using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.VB
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

            string result = "\r\n\t\t#region \"Properties\"\r\n\r\n";

            foreach (XElement propertyNode in propertiesNode.Elements("Property"))
            {
                if ("_NewEnum" == propertyNode.Attribute("Name").Value)
                    continue;

                string method = ConvertPropertyLateBindToString(settings, propertyNode, interfaceHasEnumerator, hasDefaultItem, hasItem);
                result += method;
            }

            result += "\t\t#End Region\r\n";
            return result;
        }
        
        internal static string ConvertPropertiesEarlyBindToString(Settings settings, XElement propertiesNode)
        {
            ParameterApi.ValidateItems(propertiesNode, "Property", settings);

            string result = "\r\n\t\t#region \"Properties\"\r\n\r\n";

            foreach (XElement propertyNode in propertiesNode.Elements("Property"))
            {
                if ("_NewEnum" == propertyNode.Attribute("Name").Value)
                    continue;

                string method = ConvertPropertyEarlyBindToString(settings, propertyNode);
                result += method;
            }

            result += "\t\t#End Region\r\n";
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

                method += "\t\t" + VBGenerator.GetSupportByLibraryAttribute(itemParams) + "\r\n";

                if (propertyNode.Attribute("Hidden").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    method += "\t\t" + "<EditorBrowsable(EditorBrowsableState.Never), Browsable(false)> _" + "\r\n";

                if("this" == name)
                    method += "\t\t" + "<NetRuntimeSystem.Runtime.CompilerServices.IndexerName(\"Item\")> _" + "\r\n";

                string valueReturn = VBGenerator.GetQualifiedType(returnValue);
                if ("true" == returnValue.Attribute("IsArray").Value)
                    valueReturn += "()";

                string protoype = CreatePropertyLateBindPrototypeString(settings, itemParams, interfaceHasEnumerator, hasDefaultItem, hasItem);
                string paramDoku = DocumentationApi.CreateParameterDocumentation(2, itemParams).Substring(2);
                string paramAttrib = "\t\t" + VBGenerator.GetSupportByLibraryAttribute(itemParams) + "\r\n";
                protoype = protoype.Replace("%paramDocu%", DocumentationApi.CreateParameterDocumentation(2, itemParams, false, "\t\t/// Alias for get_" + name + "\r\n").Substring(2) + "\t\t" + VBGenerator.GetSupportByLibraryAttribute(itemParams));

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

        internal static string GetEarlyBindPropertySignatur(XElement paramsNode)
        {
            string dispId = paramsNode.Parent.Element("DispIds").Element("DispId").Attribute("Id").Value;
            dispId = "\t\t" + "<DispId(" + dispId + ")> _\r\n";
            return dispId;
        }

        private static string GetReadOnly(XElement propertyNode)
        {
            if (propertyNode.Attribute("InvokeKind").Value == "INVOKE_PROPERTYGET")
                return "ReadOnly ";
            else
                return "";
        }

        internal static string ConvertPropertyEarlyBindToString(Settings settings, XElement propertyNode)
        {
            string result = "";
            string name = propertyNode.Attribute("Name").Value;
            foreach (XElement itemParams in propertyNode.Elements("Parameters"))
            {
                string method = "\t\t" + VBGenerator.GetSupportByLibraryAttribute(itemParams) + "\r\n";
                method += GetEarlyBindPropertySignatur(itemParams);

                string marshalAs = itemParams.Element("ReturnValue").Attribute("MarshalAs").Value;
                if ("" != marshalAs)
                    marshalAs = "<MarshalAs(" + marshalAs + ")> ";

                XElement returnValue = itemParams.Element("ReturnValue");
                string returnType = returnValue.Attribute("Type").Value;
                if (returnType == "COMObject")
                    returnType = "object";
                method += "\t\t" + GetReadOnly(propertyNode) + "Property " + ParameterApi.ValidateName(propertyNode.Attribute("Name").Value) + "() As " + returnType + "\r\n\r\n";
                
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
        
        private static string CreatePropertyEarlyBindPrototypeString(Settings settings, XElement itemParams)
        {
            return "";
        }

        private static string CreatePropertyLateBindPrototypeString(Settings settings, XElement itemParams, bool interfaceHasEnumerator, bool hasDefaultItem,  bool hasItem)
        {
            string inParam = "(";
            string outParam = ")";
            string name = ParameterApi.ValidateNameWithoutVarType(itemParams.Parent.Attribute("Name").Value);
            string faceName = itemParams.Parent.Parent.Parent.Attribute("Name").Value;
            
          
            string result = "";
            string parameters = ParameterApi.CreateParametersPrototypeString(settings, itemParams, true, false,true);
            string parametersCall = ParameterApi.CreateParametersCallString(settings, itemParams, true, false);
            int paramsCountWithOptionals = ParameterApi.GetParamsCount(itemParams, true);
            int paramsCountWithOutOptionals = ParameterApi.GetParamsCount(itemParams, false);
            bool hasForbiddenName = IsKeyword(name);
            if ((paramsCountWithOptionals > 0) || (faceName == name) || hasForbiddenName)
            {
                result = "\t\tPublic " + GetReadOnly(itemParams.Parent) + "Property " + name +  inParam + parameters + outParam + " As " + "%valueReturn%" + "\r\n";
                result += "\t\t\r\n\t\t\tGet\r\n\t\t\t\r\n%propertyGetBody%\t\t\tEnd Get\r\n\t\tEnd Property\r\n\r\n";

                if ((("INVOKE_PROPERTYGET" != itemParams.Parent.Attribute("InvokeKind").Value) && ("this" != name)))
                {
                    string retValueType = VBGenerator.GetQualifiedType(itemParams.Element("ReturnValue"));
                    if ("" != parameters)
                        retValueType = ", " + retValueType;
                }
            }
            else
            {
                result = "\t\tPublic " + GetReadOnly(itemParams.Parent) + "Property " + name + " As " + "%valueReturn%" + "\r\n";
                result += "\t\t\r\n\r\n\t\t\tGet\r\n%propertyGetBody%\t\t\tEnd Get\r\n%%\t\tEnd Property\r\n\r\n";
                if ("INVOKE_PROPERTYGET" != itemParams.Parent.Attribute("InvokeKind").Value)
                {
                    result = result.Replace("%%", "\t\t\tSet\r\n\t\t\t\r\n%propertySetBody%\t\t\tEnd Set\r\n");
                }
                else
                    result = result.Replace("%%", "");
            }
            /*
            if ((paramsCountWithOptionals > 0) && (paramsCountWithOutOptionals > 0) && ("this" != name) && ("_Default" != name) && (!faceName.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
            {
                result += "\t\t%paramDocu%\r\n" + "\t\tpublic %valueReturn% " + name + "(" + parameters + ")" + "\r\n\t\t{\r\n\t\t\t" +
                    "return get_" + name + "(" + parametersCall + ");" +
                    "\r\n\t\t}\r\n\r\n";
            }
            */
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
            string tabSpace = VBGenerator.TabSpace(numberOfRootTabs);
            int countOfParams = ParameterApi.GetParamsCount(parametersNode, true);
            if (0 == countOfParams)
            {
                result = tabSpace + "Dim paramsArray() As Object = Invoker.ValidateParamsArray(value)\r\n";

                string invokeTarget = "";
                if ("this" == parametersNode.Parent.Attribute("Name").Value)
                    invokeTarget = parametersNode.Parent.Attribute("Underlying").Value;
                else
                    invokeTarget = parametersNode.Parent.Attribute("Name").Value;

                result += tabSpace + "Invoker.PropertySet(Me, \"" + invokeTarget + "\", paramsArray" + modifiers + ")\r\n";
            }
            else
            {
                string invokeTarget = "";
                if ("this" == parametersNode.Parent.Attribute("Name").Value)
                    invokeTarget = parametersNode.Parent.Attribute("Underlying").Value;
                else
                    invokeTarget = parametersNode.Parent.Attribute("Name").Value;

                result = ParameterApi.CreateParametersSetArrayString(settings, numberOfRootTabs, parametersNode, true);
                result += tabSpace + "Invoker.PropertySet(Me, \"" + invokeTarget + "\", paramsArray, value" + modifiers + ")\r\n";
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
            string tabSpace = VBGenerator.TabSpace(numberOfRootTabs);           
            string methodBody = ParameterApi.CreateParametersSetArrayString(settings, numberOfRootTabs, parametersNode, true);
            XElement returnValue = parametersNode.Element("ReturnValue");
            string methodName = parametersNode.Parent.Attribute("Name").Value;
            string typeName = returnValue.Attribute("Type").Value;
            string fullTypeName = VBGenerator.GetQualifiedType(returnValue);
           
            string objectArrayField = "";
            string arrayField = "";
            string arrayName = "";
            if ("true" == returnValue.Attribute("IsArray").Value)
            {
                arrayField = "()";
                arrayName = "Array";
                fullTypeName += "()";
                objectArrayField = "";
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

                methodBody += tabSpace + "Dim returnItem As Object = Invoker.PropertyGet(Me, \"" + invokeTarget + "\", paramsArray" + modifiers + ")\r\n";
                methodBody += tabSpace + "return returnItem\r\n";
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

                    methodBody += tabSpace + "Dim returnItem As Object = Invoker.PropertyGet(Me, \"" + invokeTarget + "\", paramsArray" + modifiers + ")\r\n";
                    if (typeName == "COMObject")
                    {
                        methodBody += tabSpace + "Dim" + " newObject " + arrayField + " As COMObject = LateBindingApi.Core.Factory.CreateObject" + arrayName + "FromComProxy(Me," + objectArrayField + "returnItem)\r\n";
                        methodBody += "%modifiers%";
                        methodBody += tabSpace + "return newObject\r\n";
                    }
                    else if (typeName == "COMVariant")
                    {
                        methodBody += tabSpace + "If (Not LateBindingApi.Core.Utils.IsNothing(returnItem)) And (TypeOf returnItem Is MarshalByRefObject)\r\n" + tabSpace + "\r\n";
                        if ("" == objectArrayField)
                            methodBody += tabSpace + "\tDim newObject" + arrayField + " As COMObject = LateBindingApi.Core.Factory.CreateObject" + arrayName + "FromComProxy(Me, " + objectArrayField + "returnItem)\r\n";
                        else
                            methodBody += tabSpace + "\ttDim newObject" + arrayField + " As COMObject = LateBindingApi.Core.Factory.CreateObject" + arrayName + "FromComProxy(Me, " + objectArrayField + "returnItem)\r\n";
                        methodBody += "\t%modifiers%";
                        methodBody += tabSpace + "return newObject\r\n";
                        methodBody += tabSpace + "\r\n";
                        methodBody += tabSpace + "Else" + "\r\n" + tabSpace + "\r\n";
                        methodBody += "\t%modifiers%";
                        methodBody += tabSpace + "return " + objectArrayField + " returnItem\r\n";
                        methodBody += tabSpace + "End If\r\n";
                    }
                    else
                    {
                        // library type
                        if ("true" == returnValue.Attribute("IsArray").Value)
                        {
                            methodBody += tabSpace + "Dim newObject() COMObject As = LateBindingApi.Core.Factory.CreateObjectArrayFromComProxy(Me," + objectArrayField + "returnItem)\r\n";
                            methodBody += tabSpace + "Dim returnArray  As " + fullTypeName + "(newObject.Length) = new " + VBGenerator.GetQualifiedType(returnValue) + "\r\n";
                            methodBody += tabSpace + "For (i As Integer = 0 To newObject.Length -1\r\n";
                            methodBody += tabSpace + "\treturnArray(i) = newObject(i) as " + VBGenerator.GetQualifiedType(returnValue) + "\r\n";
                            methodBody += tabSpace + "\tNext\r\n";
                            methodBody += "%modifiers%";
                            methodBody += tabSpace + "return returnArray\r\n";
                        }
                        else
                        {
                            bool isFromIgnoreProject = VBGenerator.IsFromIgnoreProject(returnValue);
                            bool isDuplicated = VBGenerator.IsDuplicatedReturnValue(returnValue);
                            bool isDerived = VBGenerator.IsDerivedReturnValue(returnValue);  
                            if((true == isFromIgnoreProject) && (false == isDuplicated) )
                            {
                                methodBody += tabSpace + "Dim newObject As " + fullTypeName + " = LateBindingApi.Core.Factory.CreateObjectFromComProxy(Me," + objectArrayField + "returnItem)\r\n";
                                methodBody += "%modifiers%";
                                methodBody += tabSpace + "return newObject\r\n";
                            }
                            else
                            {
                                if( (isDerived) && (!isDuplicated))
                                {
                                    methodBody += tabSpace + "Dim newObject As " + fullTypeName + " = LateBindingApi.Core.Factory.CreateObjectFromComProxy(Me," + objectArrayField + "returnItem)\r\n";
                                    methodBody += "%modifiers%";
                                    methodBody += tabSpace + "return newObject\r\n";  
                                }
                                else
                                {
                                    string knownType = fullTypeName + ".LateBindingApiWrapperType";
                                    methodBody += tabSpace + "Dim newObject As " + fullTypeName + " = LateBindingApi.Core.Factory.CreateKnownObjectFromComProxy(Me," + objectArrayField + "returnItem," + knownType + ")\r\n";
                                    methodBody += "%modifiers%";
                                    methodBody += tabSpace + "return newObject\r\n";
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

                    methodBody += tabSpace + "Dim returnItem As Object = " + "Invoker.PropertyGet" + "(Me, \"" + invokeTarget + "\", paramsArray)\r\n";
                    methodBody += "%modifiers%";
                    methodBody += tabSpace + "return " + "returnItem\r\n";
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

                methodBody += tabSpace + "Invoker.PropertyGet(Me, \"" + invokeTarget + "\", paramsArray" + modifiers + ")\r\n";
                methodBody += "";
            }

            return methodBody;
        }
    }
}
