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

        /// <summary>
        /// convert all properties to code as string
        /// </summary>
        /// <param name="propertiesNode"></param>
        /// <returns></returns>
        internal static string ConvertPropertiesLateBindToString(Settings settings, XElement propertiesNode)
        {
            bool interfaceHasEnumerator = EnumerableApi.HasEnumerator(propertiesNode.Parent);
            bool hasDefaultItem = EnumerableApi.HasDefaultItem(propertiesNode.Parent);
            bool hasItem = EnumerableApi.HasItem(propertiesNode.Parent);

            ParameterApi.ValidateItems(propertiesNode, "Property", settings);

            string result = "\r\n\t\t#region Properties\r\n\r\n";

            foreach (XElement propertyNode in propertiesNode.Elements("Property"))
            {
                if ("_NewEnum" == propertyNode.Attribute("Name").Value)
                    continue;

                string method = ConvertPropertyLateBindToString(settings, propertyNode, interfaceHasEnumerator, hasDefaultItem, hasItem);
                result += method;
            }

            result += "\t\t#endregion\r\n";
            return result;
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
        internal static string ConvertPropertyLateBindToString(Settings settings, XElement propertyNode, bool interfaceHasEnumerator, bool hasDefaultItem, bool hasItem)
        {
            string result = "";
            string name = propertyNode.Attribute("Name").Value;
            bool analyzeReturn = Convert.ToBoolean(propertyNode.Attribute("AnalyzeReturn").Value);
            foreach (XElement itemParams in propertyNode.Elements("Parameters"))
            {
                string interfaceName = itemParams.Parent.Parent.Parent.Attribute("Name").Value;
                if (("this" == name) && itemParams.Elements("Parameter").Count() == 0)
                    continue;                

                XElement returnValue = itemParams.Element("ReturnValue");

                string method = "";
                if (true == settings.CreateXmlDocumentation)
                    method += DocumentationApi.CreateParameterDocumentation(2, itemParams);

                method += "\t\t" + CSharpGenerator.GetSupportByLibraryAttribute(itemParams) + "\r\n";

                if("this" == name)
                    method += "\t\t" + "[NetRuntimeSystem.Runtime.CompilerServices.IndexerName(\"Item\")]" + "\r\n";

                string valueReturn = CSharpGenerator.GetQualifiedType(returnValue);
                if ("true" == returnValue.Attribute("IsArray").Value)
                    valueReturn += "[]";

                string protoype = CreatePropertyLateBindPrototypeString(settings, itemParams, interfaceHasEnumerator, hasDefaultItem, hasItem);

                string paramDoku = DocumentationApi.CreateParameterDocumentation(2, itemParams).Substring(2);
                string paramAttrib = "\t\t" + CSharpGenerator.GetSupportByLibraryAttribute(itemParams)+ "\r\n";
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
                string method = "\t\t" + CSharpGenerator.GetSupportByLibraryAttribute(itemParams) + "\r\n";
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
            return "";
        }

        private static string CreatePropertyLateBindPrototypeString(Settings settings, XElement itemParams, bool interfaceHasEnumerator, bool hasDefaultItem,  bool hasItem)
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
            int paramsCountWithOptionals = ParameterApi.GetParamsCount(itemParams, true);
            bool hasForbiddenName = IsKeyword(name);
            if ((paramsCountWithOptionals > 0) || (faceName == name) || hasForbiddenName)
            {
                result = "\t\tpublic " + "%valueReturn% " + getter + name + inParam + parameters + outParam + "\r\n";
                if (name != "this")
                    result += "\t\t{\r\n%propertyGetBody%\t\t}\r\n\r\n";
                else
                    result += "\t\t{\r\n\t\t\tget\r\n{\t\t\t\r\n%propertyGetBody%\t\t\t}\r\n\t\t}\r\n\r\n";

                if ((("INVOKE_PROPERTYGET" != itemParams.Parent.Attribute("InvokeKind").Value) && ("this" != name)))
                {
                    string retValueType = CSharpGenerator.GetQualifiedType(itemParams.Element("ReturnValue"));
                    if ("" != parameters)
                        retValueType = ", " + retValueType;

                    if (name != "this")
                    { 
                        result += "%setAttribute%";
                        result += "\t\tpublic " + "void set_" + name + inParam + parameters + retValueType + " value" + outParam + "\r\n";
                        result += "\t\t{\r\n%propertySetBody%\t\t}\r\n\r\n";
                    }
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
                string invokeTarget = "";
                if ("this" == parametersNode.Parent.Attribute("Name").Value)
                    invokeTarget = parametersNode.Parent.Attribute("Underlying").Value;
                else
                    invokeTarget = methodName;

                methodBody += tabSpace + "object returnItem = Invoker.PropertyGet(this, \"" + invokeTarget + "\", paramsArray" + modifiers + ");\r\n";
                methodBody += tabSpace + "return returnItem;\r\n";
            }
            else if (typeName != "void")
            {
                if ("true" == returnValue.Attribute("IsComProxy").Value)
                {
                    string invokeTarget = "";
                    if ("this" == parametersNode.Parent.Attribute("Name").Value)
                        invokeTarget = parametersNode.Parent.Attribute("Underlying").Value;
                    else
                        invokeTarget = methodName;

                    methodBody += tabSpace + "object returnItem = Invoker.PropertyGet(this, \"" + invokeTarget + "\", paramsArray" + modifiers + ");\r\n";
                    if (typeName == "COMObject")
                    {
                        methodBody += tabSpace + "COMObject" + arrayField + " newObject = LateBindingApi.Core.Factory.CreateObject" + arrayName + "FromComProxy(this," + objectArrayField + "returnItem);\r\n";
                        methodBody += "%modifiers%";
                        methodBody += tabSpace + "return newObject;\r\n";
                    }
                    else if (typeName == "COMVariant")
                    {
                        methodBody += tabSpace + "if((null != returnItem) && (returnItem is MarshalByRefObject))\r\n" + tabSpace + "{\r\n";
                        if ("" == objectArrayField)
                            methodBody += tabSpace + "\tCOMObject" + arrayField + " newObject = LateBindingApi.Core.Factory.CreateObject" + arrayName + "FromComProxy(this, " + objectArrayField + "returnItem);\r\n";
                        else
                            methodBody += tabSpace + "\tCOMObject" + arrayField + " newObject = LateBindingApi.Core.Factory.CreateObject" + arrayName + "FromComProxy(this, " + objectArrayField + "returnItem);\r\n";
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
                            methodBody += tabSpace + "COMObject[] newObject = LateBindingApi.Core.Factory.CreateObjectArrayFromComProxy(this," + objectArrayField + "returnItem);\r\n";
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
                                methodBody += tabSpace + fullTypeName + " newObject = LateBindingApi.Core.Factory.CreateObjectFromComProxy(this," + objectArrayField + "returnItem) as " + fullTypeName + ";\r\n";
                                methodBody += "%modifiers%";
                                methodBody += tabSpace + "return newObject;\r\n";
                            }
                            else
                            {
                                if( (isDerived) && (!isDuplicated))
                                {
                                    methodBody += tabSpace + fullTypeName + " newObject = LateBindingApi.Core.Factory.CreateObjectFromComProxy(this," + objectArrayField + "returnItem) as " + fullTypeName + ";\r\n";
                                    methodBody += "%modifiers%";
                                    methodBody += tabSpace + "return newObject;\r\n";  

                                }
                                else
                                {
                                    string knownType = fullTypeName + ".LateBindingApiWrapperType";
                                    methodBody += tabSpace + fullTypeName + " newObject = LateBindingApi.Core.Factory.CreateKnownObjectFromComProxy(this," + objectArrayField + "returnItem," + knownType + ") as " + fullTypeName + ";\r\n";
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

                    string invokeTarget = "";
                    if ("this" == parametersNode.Parent.Attribute("Name").Value)
                        invokeTarget = parametersNode.Parent.Attribute("Underlying").Value;
                    else
                        invokeTarget = methodName;

                    methodBody += tabSpace + "object" + " returnItem = " + objectString + "Invoker.PropertyGet" + "(this, \"" + invokeTarget + "\", paramsArray);\r\n";
                    methodBody += "%modifiers%";
                    methodBody += tabSpace + "return (" + fullTypeName + ")returnItem;\r\n";
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
                string invokeTarget = "";
                if ("this" == parametersNode.Parent.Attribute("Name").Value)
                    invokeTarget = parametersNode.Parent.Attribute("Underlying").Value;
                else
                    invokeTarget = methodName;

                methodBody += tabSpace + "Invoker.PropertyGet(this, \"" + invokeTarget + "\", paramsArray" + modifiers + ");\r\n";
                methodBody += "";
            }

            return methodBody;
        }
    }
}
