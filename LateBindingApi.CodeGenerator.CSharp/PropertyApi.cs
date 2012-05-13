using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class PropertyApi
    {
        private static string[] _Keywords;

        internal static string ConvertConflictPropertiesLateBindToString(Settings settings, XElement propertiesNode)
        {
            bool interfaceHasEnumerator = EnumerableApi.HasEnumerator(propertiesNode.Parent);
            bool hasDefaultItem = EnumerableApi.HasDefaultItem(propertiesNode.Parent);

            ParameterApi.ValidateItems(propertiesNode, "Property", settings);

            string result = "\r\n\t\t#region Properties\r\n\r\n";

            foreach (XElement propertyNode in propertiesNode.Elements("Property"))
            {
                if ("this" == propertyNode.Attribute("Name").Value)
                    continue;

                if ("_NewEnum" == propertyNode.Attribute("Name").Value)
                    continue;

                if (("Item" == propertyNode.Attribute("Name").Value) && HasThis(propertiesNode.Parent))
                    continue;

                string method = ConvertConflictPropertyLateBindToString(settings, propertyNode, interfaceHasEnumerator, hasDefaultItem);
                result += method;
            }

            result += "\t\t#endregion\r\n";
            return result;
        }

        /// <summary>
        /// convert all properties to code as string
        /// </summary>
        /// <param name="propertiesNode"></param>
        /// <returns></returns>
        internal static string ConvertPropertiesLateBindToString(Settings settings, XElement propertiesNode)
        {
            bool interfaceHasEnumerator = EnumerableApi.HasEnumerator(propertiesNode.Parent);
            bool hasDefaultItem = EnumerableApi.HasDefaultItem(propertiesNode.Parent);

            ParameterApi.ValidateItems(propertiesNode, "Property", settings);

            string result = "\r\n\t\t#region Properties\r\n\r\n";

            foreach (XElement propertyNode in propertiesNode.Elements("Property"))
            {
                if ("_NewEnum" == propertyNode.Attribute("Name").Value)
                    continue;

                if (("Item" == propertyNode.Attribute("Name").Value) && HasThis(propertiesNode.Parent))
                    continue;

     
               string method = ConvertPropertyLateBindToString(settings, propertyNode, interfaceHasEnumerator, hasDefaultItem);
               result += method;
                          
            }

            result += "\t\t#endregion\r\n";
            return result;
        }

        internal static bool HasThis(XElement entityNode)
        {

            XElement node = (from a in entityNode.Element("Properties").Elements("Property")
                             where a.Attribute("Name").Value.Equals("this", StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();
            if (node != null)
                return true;

            node = (from a in entityNode.Element("Methods").Elements("Method")
                    where a.Attribute("Name").Value.Equals("this", StringComparison.InvariantCultureIgnoreCase)
                    select a).FirstOrDefault();
            if (node != null)
                return true;

            return false;
        }

        internal static string ConvertPropertiesEarlyBindToString(Settings settings, XElement propertiesNode)
        {
            ParameterApi.ValidateItems(propertiesNode, "Property", settings);

            string result = "\r\n\t\t#region Properties\r\n\r\n";

            foreach (XElement propertyNode in propertiesNode.Elements("Property"))
            {
                if ("_NewEnum" == propertyNode.Attribute("Name").Value)
                    continue;

                string method = ConvertPropertyEarlyBindToString(settings, propertyNode);
                result += method;
            }

            result += "\t\t#endregion\r\n";
            return result;
        }

        private static bool IsKeyword(string text)
        {
            if (null == _Keywords)
            {
                string res = RessourceApi.ReadString("Keywords.txt");
                _Keywords = res.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }

            foreach (string item in _Keywords)
            {
                if (item == text)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// convert property to code as string
        /// </summary>
        /// <param name="propertyNode"></param>
        /// <returns></returns>
        internal static string ConvertConflictPropertyLateBindToString(Settings settings, XElement propertyNode, bool interfaceHasEnumerator, bool hasDefaultItem)
        {
            bool isUnkownProxy = false;
            string result = "";
            string name = propertyNode.Attribute("Name").Value;

            if( (name == "Name") && (propertyNode.Parent.Parent.Attribute("Name").Value == "Name"))
            { 
            
            }

            bool analyzeReturn = Convert.ToBoolean(propertyNode.Attribute("AnalyzeReturn").Value);
            foreach (XElement itemParams in propertyNode.Elements("Parameters"))
            {
                string interfaceName = itemParams.Parent.Parent.Parent.Attribute("Name").Value;
                if (("this" == name) && itemParams.Elements("Parameter").Count() == 0)
                    continue;
  
                bool isOptionalConflict = false;
                foreach (XAttribute item in itemParams.Attributes())
                {
                    if (item.Name == "IsOptionalConflict" && item.Value == "true")
                    {
                        isOptionalConflict = true;
                        break;
                    }
                }

                bool isNameConflict = false;
                foreach (XAttribute item in itemParams.Attributes())
                {
                    if (item.Name == "IsNameConflict" && item.Value == "true")
                    {
                        isNameConflict = true;
                        break;
                    }
                }

                if (!isOptionalConflict && !isNameConflict)
                    continue;

                XElement returnValue = itemParams.Element("ReturnValue");

                string method = "";
                if (true == settings.CreateXmlDocumentation)
                    method += DocumentationApi.CreateParameterDocumentation(2, itemParams);

                method += "\t\t" + CSharpGenerator.GetSupportByVersionAttribute(itemParams) + "\r\n";

                int paramsCountWithOptionals = ParameterApi.GetParamsCount(itemParams, true);

                if (propertyNode.Attribute("Hidden").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    method += "\t\t" + "[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]" + "\r\n";
             

                string valueReturn = CSharpGenerator.GetQualifiedType(returnValue);
                if (valueReturn == "COMObject")
                {
                    valueReturn = "object";
                    isUnkownProxy = true;
                }
                else
                    isUnkownProxy = false;

                if ("true" == returnValue.Attribute("IsArray").Value)
                    valueReturn += "[]";

                string protoype = CreatePropertyLateBindPrototypeString(settings, itemParams, interfaceHasEnumerator, hasDefaultItem, true);
                string paramDoku = DocumentationApi.CreateParameterDocumentation(2, itemParams).Substring(2);
                string paramAttrib = "\t\t" + CSharpGenerator.GetSupportByVersionAttribute(itemParams) + "\r\n";
                protoype = protoype.Replace("%paramDocu%", DocumentationApi.CreateParameterDocumentation(2, itemParams, false, "\t\t/// Alias for get_" + name + "\r\n").Substring(2) + "\t\t" + CSharpGenerator.GetSupportByVersionAttribute(itemParams));

                if (true == settings.CreateXmlDocumentation)
                    paramAttrib = paramDoku + paramAttrib;
                protoype = protoype.Replace("%setAttribute%", "\t\t" + paramAttrib);
                if (analyzeReturn)
                    protoype = protoype.Replace("%valueReturn%", valueReturn);
                else
                    protoype = protoype.Replace("%valueReturn%", "object");

                method += protoype;

                int paramsCount = ParameterApi.GetParamsCount(itemParams, true);
                bool hasForbiddenName = IsKeyword(propertyNode.Attribute("Name").Value as string);
                bool convertToMethod = ((paramsCount > 0) || (true == hasForbiddenName));
                string methodGetBody = CreatePropertyGetBody(settings, 3, itemParams, convertToMethod, analyzeReturn);
                string methodSetBody = CreatePropertySetBody(settings, 3, itemParams, convertToMethod);

                method = method.Replace("%propertyGetBody%", methodGetBody);
                method = method.Replace("%propertySetBody%", methodSetBody);

                result += method;
            }
            return result;
        }

        /// <summary>
        /// convert property to code as string
        /// </summary>
        /// <param name="propertyNode"></param>
        /// <returns></returns>
        internal static string ConvertPropertyLateBindToString(Settings settings, XElement propertyNode, bool interfaceHasEnumerator, bool hasDefaultItem)
        {
            string result = "";
            string name = propertyNode.Attribute("Name").Value;
            bool analyzeReturn = Convert.ToBoolean(propertyNode.Attribute("AnalyzeReturn").Value);
            foreach (XElement itemParams in propertyNode.Elements("Parameters"))
            {
                string interfaceName = itemParams.Parent.Parent.Parent.Attribute("Name").Value;
                if (("this" == name) && itemParams.Elements("Parameter").Count() == 0)
                    continue;

                bool isOptionalConflict = false;
                if ("this" != name)
                { 
                    foreach (XAttribute item in itemParams.Attributes())
                    {
                        if (item.Name == "IsOptionalConflict" && item.Value == "true")
                        {
                            isOptionalConflict = true;
                            break;
                        }
                    }
                }

                bool isNameConflict = false;
                if ("this" != name)
                { 
                    foreach (XAttribute item in itemParams.Attributes())
                    {
                        if (item.Name == "IsNameConflict" && item.Value == "true")
                        {
                            isNameConflict = true;
                            break;
                        }
                    }
                }

                if (isOptionalConflict || isNameConflict)
                    continue;

                XElement returnValue = itemParams.Element("ReturnValue");

                string method = "";
                if (true == settings.CreateXmlDocumentation)
                    method += DocumentationApi.CreateParameterDocumentation(2, itemParams);

                method += "\t\t" + CSharpGenerator.GetSupportByVersionAttribute(itemParams) + "\r\n";

                int paramsCountWithOptionals = ParameterApi.GetParamsCount(itemParams, true);

                if (propertyNode.Attribute("Hidden").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    method += "\t\t" + "[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]" + "\r\n";
                else if( (paramsCountWithOptionals > 0) && (name != "this"))
                    method += "\t\t" + "[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]" + "\r\n";

                if("this" == name)
                    method += "\t\t" + "[NetRuntimeSystem.Runtime.CompilerServices.IndexerName(\"Item\")]" + "\r\n";

                string valueReturn = CSharpGenerator.GetQualifiedType(returnValue);
                if (valueReturn == "COMObject")
                {
                    valueReturn = "object";
                }
                if ("true" == returnValue.Attribute("IsArray").Value)
                    valueReturn += "[]";

                string protoype = CreatePropertyLateBindPrototypeString(settings, itemParams, interfaceHasEnumerator, hasDefaultItem, false);
                string paramDoku = DocumentationApi.CreateParameterDocumentation(2, itemParams).Substring(2);
                string paramAttrib = "\t\t" + CSharpGenerator.GetSupportByVersionAttribute(itemParams)+ "\r\n";
                protoype = protoype.Replace("%paramDocu%", DocumentationApi.CreateParameterDocumentation(2, itemParams, false, "\t\t/// Alias for get_" + name + "\r\n").Substring(2) + "\t\t" + CSharpGenerator.GetSupportByVersionAttribute(itemParams));

                if (true == settings.CreateXmlDocumentation)
                    paramAttrib = paramDoku + paramAttrib;
                protoype = protoype.Replace("%setAttribute%", "\t\t" + paramAttrib);
                if(analyzeReturn)
                    protoype = protoype.Replace("%valueReturn%", valueReturn);
                else
                    protoype = protoype.Replace("%valueReturn%", "object");
                
                method += protoype;
 
                int paramsCount = ParameterApi.GetParamsCount(itemParams, true);
                bool hasForbiddenName = IsKeyword(propertyNode.Attribute("Name").Value as string);
                bool convertToMethod = ((paramsCount > 0) || (true ==hasForbiddenName));
                string methodGetBody = CreatePropertyGetBody(settings, 3, itemParams, convertToMethod, analyzeReturn);
                string methodSetBody = CreatePropertySetBody(settings, 3, itemParams, convertToMethod);

                method = method.Replace("%propertyGetBody%", methodGetBody);
                method = method.Replace("%propertySetBody%", methodSetBody);

                result += method;
            }
            return result;
        }

        internal static string ConvertPropertyEarlyBindToString(Settings settings, XElement propertyNode)
        {
            string result = "";
            string name = propertyNode.Attribute("Name").Value;
            foreach (XElement itemParams in propertyNode.Elements("Parameters"))
            {
                string method = "\t\t" + CSharpGenerator.GetSupportByVersionAttribute(itemParams) + "\r\n";
                method += "\t\t" + "[DispId(" + itemParams.Parent.Element("DispIds").Element("DispId").Attribute("Id").Value + ")]\r\n";

                XElement returnValue = itemParams.Element("ReturnValue");
                method += "\t\t" + GetEarlyBindPropertySignatur(itemParams) + "\r\n\r\n";


                result += method;
            }
            return result;

        }

        internal static string cValidatePropertyName(string name)
        {
            if (null == _Keywords)
            {
                string res = RessourceApi.ReadString("Keywords.txt");
                _Keywords = res.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }

            bool isInList = false;
            foreach (string item in _Keywords)
            {
                if (item == name)
                {
                    isInList = true;
                    break;
                }
            }
            if (true == isInList)
                name = "get_" + name;

            return name;
        }

        internal static string GetEarlyBindPropertySignatur(XElement paramsNode)
        {
            string dispId = paramsNode.Parent.Element("DispIds").Element("DispId").Attribute("Id").Value;
            string marshalAs = paramsNode.Element("ReturnValue").Attribute("MarshalAs").Value;
            if ("" != marshalAs)
                marshalAs = "[return: MarshalAs(" + marshalAs + ")] ";

            string result = paramsNode.Element("ReturnValue").Attribute("Type").Value + " " + paramsNode.Parent.Attribute("Name").Value + "{";

            result += marshalAs + "[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(" + dispId + ")] get;";

            return result + "}";
        }
        
        private static string CreatePropertyEarlyBindPrototypeString(Settings settings, XElement itemParams)
        {
            throw new NotSupportedException("CreatePropertyEarlyBindPrototypeString");
        }

        private static bool IsCustom(XElement itemParams)
        {
            foreach (XAttribute item in itemParams.Attributes())
            {
                if (item.Name == "IsCustom" && item.Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        private static string CreatePropertyLateBindPrototypeString(Settings settings, XElement itemParams, bool interfaceHasEnumerator, bool hasDefaultItem, bool isFromConflicted )
        {
            string getter = "get_";
            string inParam = "(";
            string outParam = ")";
            string name = itemParams.Parent.Attribute("Name").Value;
            string faceName = itemParams.Parent.Parent.Parent.Attribute("Name").Value;

            if("this" == name)
            {
                name = "this";
                inParam = "[";
                outParam = "]";
                getter = "";
            }

            string result = "";
            string parameters = ParameterApi.CreateParametersPrototypeString(settings, itemParams, true, false);
            string parametersCall = ParameterApi.CreateParametersCallString(settings, itemParams, true, false);
            int paramsCountWithOptionals = ParameterApi.GetParamsCount(itemParams, true);
            int paramsCountWithOutOptionals = ParameterApi.GetParamsCount(itemParams, false);
            bool hasForbiddenName = IsKeyword(name);
            IEnumerable<XElement> xParams = ParameterApi.GetParameter(itemParams, true);
            if ((xParams.Count() > 0) || hasForbiddenName) // || (faceName == name) 
            {
                result = "\t\tpublic " + "%valueReturn% " + getter + name + inParam + parameters + outParam + "\r\n";
                if (name != "this")
                    result += "\t\t{\t\t\r\n%propertyGetBody%\t\t}\r\n\r\n";
                else
                    result += "\t\t{\r\n\t\t\tget\r\n{\t\t\t\r\n%propertyGetBody%\t\t\t}\r\n\t\t}\r\n\r\n";

                if ((("INVOKE_PROPERTYGET" != itemParams.Parent.Attribute("InvokeKind").Value) && ("this" != name)))
                {
                    string retValueType = CSharpGenerator.GetQualifiedType(itemParams.Element("ReturnValue"));
                    if (retValueType == "COMObject")
                    { 
                       
                    }
                    if ("" != parameters)
                        retValueType = ", " + retValueType;
                    
                    result += "%setAttribute%";
                    result += "\t\tpublic " + "void set_" + name + inParam + parameters + retValueType + " value" + outParam + "\r\n";
                    result += "\t\t{\r\n%propertySetBody%\t\t}\r\n\r\n";                    
                }
                else if((("INVOKE_PROPERTYGET" != itemParams.Parent.Attribute("InvokeKind").Value) && ("this" == name)))
                {
                    result = "\t\tpublic " + "%valueReturn% " + name + inParam + parameters + outParam + "\r\n";
                    result += "\t\t{\r\n\t\t\tget\r\n\t\t\t{\r\n%propertyGetBody%\t\t\t}\r\n%%\t\t}\r\n\r\n";
                    result = result.Replace("%%", "\t\t\tset\r\n\t\t\t{\r\n%propertySetBody%\t\t\t}\r\n");
                }
            }
            else
            {
                result = "\t\tpublic " + "%valueReturn% " + name + "\r\n";
                result += "\t\t{\r\n\t\t\tget\r\n\t\t\t{\r\n%propertyGetBody%\t\t\t}\r\n%%\t\t}\r\n\r\n";
                if ("INVOKE_PROPERTYGET" != itemParams.Parent.Attribute("InvokeKind").Value)
                {
                    result = result.Replace("%%", "\t\t\tset\r\n\t\t\t{\r\n%propertySetBody%\t\t\t}\r\n");
                }
                else
                    result = result.Replace("%%", "");
            }

            
            if (isFromConflicted)
            {
                if ((paramsCountWithOptionals > 0) && ("this" != name) && ("_Default" != name))
                {
                    result += "\t\t%paramDocu%\r\n" + "\t\tpublic %valueReturn% " + name + "(" + parameters + ")" + "\r\n\t\t{\r\n\t\t\t" +
                        "return get_" + name + "(" + parametersCall + ");" +
                        "\r\n\t\t}\r\n\r\n";
                }
            }
            else
            {
                if ((paramsCountWithOptionals > 0) && ("this" != name) && ("_Default" != name) && (!faceName.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    result += "\t\t%paramDocu%\r\n" + "\t\tpublic %valueReturn% " + name + "(" + parameters + ")" + "\r\n\t\t{\r\n\t\t\t" +
                        "return get_" + name + "(" + parametersCall + ");" +
                        "\r\n\t\t}\r\n\r\n";
                }
            }
            
            return result;
        }

 
        /// <summary>
        /// convert parametersNode to complete property body code
        /// </summary>
        /// <param name="numberOfRootTabs"></param>
        /// <param name="parametersNode"></param>
        /// <returns></returns>
        private static string CreatePropertyGetBody(Settings settings, int numberOfRootTabs, XElement parametersNode, bool convertToMethod, bool analyzeMethod)
        {
            string result = "";
            if (true == convertToMethod)
                result = CreatePropertyGetBodyCode(settings, 3, parametersNode, analyzeMethod);
            else
                result = CreatePropertyGetBodyCode(settings, 4, parametersNode, analyzeMethod);
            
            return result;           
        }

        private static string CreatePropertySetBody(Settings settings, int numberOfRootTabs, XElement parametersNode, bool convertToMethod)
        {
            string result = "";
            if (true == convertToMethod)
                result = CreatePropertySetBodyCode(settings, 3, parametersNode);
            else
                result = CreatePropertySetBodyCode(settings, 4, parametersNode);

            return result;  
        }

        internal static string CreatePropertySetBodyCode(Settings settings, int numberOfRootTabs, XElement parametersNode)
        {
            string modifiers = "";
            if (true == ParameterApi.HasRefOrOutParamsParams(parametersNode, true))
                modifiers = ", modifiers";

            string result = "";
            string tabSpace = CSharpGenerator.TabSpace(numberOfRootTabs);
            int countOfParams = ParameterApi.GetParamsCount(parametersNode, true);
            if (0 == countOfParams)
            {
                result = tabSpace + "object[] paramsArray = Invoker.ValidateParamsArray(value);\r\n";

                string invokeTarget = "";
                if ("this" == parametersNode.Parent.Attribute("Name").Value)
                    invokeTarget = parametersNode.Parent.Attribute("Underlying").Value;
                else
                    invokeTarget = parametersNode.Parent.Attribute("Name").Value;

                result += tabSpace + "Invoker.PropertySet(this, \"" + invokeTarget + "\", paramsArray" + modifiers + ");\r\n";
            }
            else
            {
                string invokeTarget = "";
                if ("this" == parametersNode.Parent.Attribute("Name").Value)
                    invokeTarget = parametersNode.Parent.Attribute("Underlying").Value;
                else
                    invokeTarget = parametersNode.Parent.Attribute("Name").Value;

                result = ParameterApi.CreateParametersSetArrayString(settings, numberOfRootTabs, parametersNode, true);
                result += tabSpace + "Invoker.PropertySet(this, \"" + invokeTarget + "\", paramsArray, value" + modifiers + ");\r\n";
                if (true == ParameterApi.HasRefOrOutParamsParams(parametersNode, true))
                    result += ParameterApi.CreateParametersToRefUpdateString(settings, numberOfRootTabs,parametersNode,true);
            }
                        
            return result; 
        }

        /// <summary>
        /// convert parametersNode to complete property body code
        /// </summary>
        /// <param name="numberOfRootTabs"></param>
        /// <param name="parametersNode"></param>
        /// <returns></returns>
        internal static string CreatePropertyGetBodyCode(Settings settings, int numberOfRootTabs, XElement parametersNode, bool analyzeReturn)
        {
            string tabSpace = CSharpGenerator.TabSpace(numberOfRootTabs);           
            string methodBody = ParameterApi.CreateParametersSetArrayString(settings, numberOfRootTabs, parametersNode, true);
            XElement returnValue = parametersNode.Element("ReturnValue");
            string methodName = parametersNode.Parent.Attribute("Name").Value;
            string typeName = returnValue.Attribute("Type").Value;
            if("this" == methodName)
                methodName = parametersNode.Parent.Attribute("Underlying").Value;

            string fullTypeName = CSharpGenerator.GetQualifiedType(returnValue);
           
            string objectArrayField = "";
            string arrayField = "";
            string arrayName = "";
            if ("true" == returnValue.Attribute("IsArray").Value)
            {
                arrayField = "[]";
                arrayName = "Array";
                fullTypeName += "[]";
                objectArrayField = "(object[])";
            }

            string modifiers = "";
            if (true == ParameterApi.HasRefOrOutParamsParams(parametersNode, true))
                modifiers = ", modifiers";

            if (!analyzeReturn)
            {
                string invokeTarget = methodName;

                methodBody += tabSpace + "object returnItem = Invoker.PropertyGet(this, \"" + invokeTarget + "\", paramsArray" + modifiers + ");\r\n";
                methodBody += tabSpace + "return returnItem;\r\n";
            }
            else if (typeName != "void")
            {
                if ("true" == returnValue.Attribute("IsComProxy").Value)
                {
                    string invokeTarget  = methodName;
                   
                    methodBody += tabSpace + "object returnItem = Invoker.PropertyGet(this, \"" + invokeTarget + "\", paramsArray" + modifiers + ");\r\n";
                    if (typeName == "COMObject")
                    {
                        methodBody += tabSpace + "COMObject" + arrayField + " newObject = NetOffice.Factory.CreateObject" + arrayName + "FromComProxy(this," + objectArrayField + "returnItem);\r\n";
                        methodBody += "%modifiers%";
                        methodBody += tabSpace + "return newObject;\r\n";
                    }
                    else if (typeName == "COMVariant")
                    {
                        methodBody += tabSpace + "if((null != returnItem) && (returnItem is MarshalByRefObject))\r\n" + tabSpace + "{\r\n";
                        if ("" == objectArrayField)
                            methodBody += tabSpace + "\tCOMObject" + arrayField + " newObject = NetOffice.Factory.CreateObject" + arrayName + "FromComProxy(this, " + objectArrayField + "returnItem);\r\n";
                        else
                            methodBody += tabSpace + "\tCOMObject" + arrayField + " newObject = NetOffice.Factory.CreateObject" + arrayName + "FromComProxy(this, " + objectArrayField + "returnItem);\r\n";
                        methodBody += "\t%modifiers%";
                        methodBody += tabSpace + "return newObject;\r\n";
                        methodBody += tabSpace + "}\r\n";
                        methodBody += tabSpace + "else" + "\r\n" + tabSpace + "{\r\n";
                        methodBody += "\t%modifiers%";
                        methodBody += tabSpace + "return " + objectArrayField + " returnItem;\r\n";
                        methodBody += tabSpace + "}\r\n";
                    }
                    else
                    {
                        // library type
                        if ("true" == returnValue.Attribute("IsArray").Value)
                        {
                            methodBody += tabSpace + "COMObject[] newObject = NetOffice.Factory.CreateObjectArrayFromComProxy(this," + objectArrayField + "returnItem);\r\n";
                            methodBody += tabSpace + fullTypeName + " returnArray = new " + CSharpGenerator.GetQualifiedType(returnValue) + "[newObject.Length];\r\n";
                            methodBody += tabSpace + "for (int i = 0; i < newObject.Length; i++)\r\n";
                            methodBody += tabSpace + "\treturnArray[i] = newObject[i] as " + CSharpGenerator.GetQualifiedType(returnValue) + ";\r\n";
                            methodBody += "%modifiers%";
                            methodBody += tabSpace + "return returnArray;\r\n";
                        }
                        else
                        {
                            bool isFromIgnoreProject = CSharpGenerator.IsFromIgnoreProject(returnValue); 
                            bool isDuplicated =  CSharpGenerator.IsDuplicatedReturnValue(returnValue);
                            bool isDerived = CSharpGenerator.IsDerivedReturnValue(returnValue);  
                            if((true == isFromIgnoreProject) && (false == isDuplicated) )
                            {
                                methodBody += tabSpace + fullTypeName + " newObject = NetOffice.Factory.CreateObjectFromComProxy(this," + objectArrayField + "returnItem) as " + fullTypeName + ";\r\n";
                                methodBody += "%modifiers%";
                                methodBody += tabSpace + "return newObject;\r\n";
                            }
                            else
                            {
                                if( (isDerived) && (!isDuplicated))
                                {
                                    methodBody += tabSpace + fullTypeName + " newObject = NetOffice.Factory.CreateObjectFromComProxy(this," + objectArrayField + "returnItem) as " + fullTypeName + ";\r\n";
                                    methodBody += "%modifiers%";
                                    methodBody += tabSpace + "return newObject;\r\n";  

                                }
                                else
                                {
                                    string knownType = fullTypeName + ".LateBindingApiWrapperType";
                                    methodBody += tabSpace + fullTypeName + " newObject = NetOffice.Factory.CreateKnownObjectFromComProxy(this," + objectArrayField + "returnItem," + knownType + ") as " + fullTypeName + ";\r\n";
                                    methodBody += "%modifiers%";
                                    methodBody += tabSpace + "return newObject;\r\n";

                                }
                            }
                        }
                    }
                }
                else
                {
                    // native type
                    string objectString = "";
                    if ("true" == returnValue.Attribute("IsArray").Value)
                        objectString = "(object)";

                    string invokeTarget  = methodName;
                   
                    methodBody += tabSpace + "object" + " returnItem = " + objectString + "Invoker.PropertyGet" + "(this, \"" + invokeTarget + "\", paramsArray);\r\n";
                    methodBody += "%modifiers%";

                    if (returnValue.Attribute("TypeKind").Value.Equals("TKIND_RECORD", StringComparison.InvariantCultureIgnoreCase) || fullTypeName == "UIntPtr")
                    {
                        methodBody += tabSpace + "return (" + fullTypeName + ")returnItem;\r\n";
                    }
                    else if (returnValue.Attribute("IsEnum").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase) )
                    {
                        methodBody += tabSpace + "int intReturnItem = NetRuntimeSystem.Convert.ToInt32(returnItem);\r\n";
                        methodBody += tabSpace + "return (" + fullTypeName + ")intReturnItem;\r\n";
                    }
                    else if (returnValue.Attribute("IsArray").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase)
                                    || fullTypeName.Equals("object", StringComparison.InvariantCultureIgnoreCase))
                    {
                        methodBody += tabSpace + "return (" + fullTypeName + ")returnItem;\r\n";
                    }
                    else if (returnValue.Attribute("IsExternal").Value.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                        methodBody += tabSpace + "return NetRuntimeSystem.Convert.To" + CSharpGenerator.ConvertTypeToConvertCall(fullTypeName) + "(returnItem);\r\n";

                    else
                        methodBody += tabSpace + "return NetRuntimeSystem.Convert.To" + CSharpGenerator.ConvertTypeToConvertCall(fullTypeName) + "(returnItem);\r\n";
                }

                if (true == ParameterApi.HasRefOrOutParamsParams(parametersNode, true))
                {
                    string modRefs = ParameterApi.CreateParametersToRefUpdateString(settings, numberOfRootTabs, parametersNode, true);
                    methodBody = methodBody.Replace("%modifiers%", modRefs);
                }
                else
                {
                    methodBody = methodBody.Replace("%modifiers%", "");
                }
            }
            else
            {
                string invokeTarget = methodName;
                methodBody += tabSpace + "Invoker.PropertyGet(this, \"" + invokeTarget + "\", paramsArray" + modifiers + ");\r\n";
                methodBody += "";
            }

            return methodBody;
        }
    }
}
