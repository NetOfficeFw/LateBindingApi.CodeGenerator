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
        internal static string ConvertPropertiesLateBindToString(Settings settings, XElement propertiesNode, string instance)
        {
            bool interfaceHasEnumerator = EnumerableApi.HasEnumerator(propertiesNode.Parent);
            bool hasDefaultItem = EnumerableApi.HasDefaultItem(propertiesNode.Parent);
       
            ParameterApi.ValidateItems(propertiesNode, "Property", settings);

            string result = "\r\n\t\t#Region \"Properties\"\r\n\r\n";

            foreach (XElement propertyNode in propertiesNode.Elements("Property"))
            {
                if ("_NewEnum" == propertyNode.Attribute("Name").Value)
                    continue;

                string method = ConvertPropertyLateBindToString(settings, propertyNode, interfaceHasEnumerator, hasDefaultItem, instance);
                result += method;
            }

            result += "\t\t#End Region\r\n";
            return result;
        }
        
        internal static string ConvertPropertiesEarlyBindToString(Settings settings, XElement propertiesNode)
        {
            ParameterApi.ValidateItems(propertiesNode, "Property", settings);

            string result = "\r\n\t\t#Region \"Properties\"\r\n\r\n";

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
        /// returns overloads with the 
        /// </summary>
        /// <returns></returns>
        public static List<XElement> GetOverloadsWithMoreParameters(XElement parametersNode, IEnumerable<XElement> parameters)
        {
            int paramCount = parametersNode.Elements("Parameter").Count();
            List<XElement> listMethods = new List<XElement>();

            foreach (XElement item in parameters)
            {
                IEnumerable<XElement> otherParameters = item.Elements("Parameter");
                List<XElement> listOtherParameters = new List<XElement>();
                foreach (XElement itemOther in otherParameters)
                    listOtherParameters.Add(itemOther);

                int otherParametersCount = otherParameters.Count();
                if (otherParametersCount > paramCount)
                {
                    bool allOptionals = true;
                    for (int i = paramCount; i < otherParametersCount; i++)
                    {
                        XElement other = listOtherParameters[i];
                        if (other.Attribute("IsOptional").Value == "false")
                        {
                            allOptionals = false;
                            break;
                        }
                    }

                    if (allOptionals)
                        listMethods.Add(item);
                }
            }

            return listMethods;
        }

        /// <summary>
        /// convert property to code as string
        /// </summary>
        /// <param name="propertyNode"></param>
        /// <returns></returns>
        internal static string ConvertPropertyLateBindToString(Settings settings, XElement propertyNode, bool interfaceHasEnumerator, bool hasDefaultItem, string instance)
        {
            string result = "";
            string name = ParameterApi.ValidateNameWithoutVarType(propertyNode.Attribute("Name").Value);
            bool analyzeReturn = Convert.ToBoolean(propertyNode.Attribute("AnalyzeReturn").Value);
            foreach (XElement itemParams in propertyNode.Elements("Parameters"))
            {
                string interfaceName = itemParams.Parent.Parent.Parent.Attribute("Name").Value;
         
                string[] supportDocuArray = VBGenerator.GetSupportByVersionArray(itemParams);

                // gibt es andere überladungen mit mehr parametern als dieser überladung und sind die überzählen alle optional?
                // dann füge deren supportbylibray überladungen an
                List<XElement> otherOverloads = GetOverloadsWithMoreParameters(itemParams, propertyNode.Elements("Parameters"));
                foreach (XElement other in otherOverloads)
                    supportDocuArray = DocumentationApi.AddParameterDocumentation(supportDocuArray, other);

                string paramDoku = DocumentationApi.CreateParameterDocumentationForMethod(2, supportDocuArray, itemParams);
                string paramAttrib = VBGenerator.GetSupportByVersionAttribute(supportDocuArray, itemParams);

                XElement returnValue = itemParams.Element("ReturnValue");

                string method = "";

                string valueReturn = VBGenerator.GetQualifiedType(returnValue);
                if ("true" == returnValue.Attribute("IsArray").Value)
                    valueReturn += "()";

                if ("COMObject" == valueReturn)
                    valueReturn = "Object";

                string protoype = CreatePropertyLateBindPrototypeString(settings, itemParams, interfaceHasEnumerator, hasDefaultItem);
                paramDoku = DocumentationApi.CreateParameterDocumentation(2, itemParams).Substring(2);
                paramAttrib = VBGenerator.GetSupportByVersionAttribute(itemParams);
                bool hiddenIsSet = false;
                if (propertyNode.Attribute("Hidden").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                { 
                    paramAttrib += "\r\n\t\t" + "<EditorBrowsable(EditorBrowsableState.Never), Browsable(false)> _" + "";
                    hiddenIsSet = true;
                }

                if (hiddenIsSet)
                    protoype = protoype.Replace("%specialHiddenAttribute%", "");
                else
                    protoype = protoype.Replace("%specialHiddenAttribute%", "\r\n\t\t" + "<EditorBrowsable(EditorBrowsableState.Never), Browsable(false)> _" + "\r\n");

                protoype = protoype.Replace("%paramDocu%", paramDoku);

                protoype = protoype.Replace("%setAttribute%", paramAttrib);
                if(analyzeReturn)
                    protoype = protoype.Replace("%valueReturn%", valueReturn);
                else
                    protoype = protoype.Replace("%valueReturn%", "object");
                
                method += protoype;
 
                int paramsCount = ParameterApi.GetParamsCount(itemParams, true);
                bool hasForbiddenName = IsKeyword(propertyNode.Attribute("Name").Value as string);
          
                string methodGetBody = CreatePropertyGetBody(settings, 3, itemParams, analyzeReturn, instance);
                string methodSetBody = CreatePropertySetBody(settings, 3, itemParams, instance );

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
                string method = "\t\t" + VBGenerator.GetSupportByVersionAttribute(itemParams) + "\r\n";
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
        
     
        private static bool IsDefaultItem(XElement itemParams)
        {
            XElement itemMethod = itemParams.Parent;

            XElement node = (from a in itemMethod.Element("DispIds").Elements("DispId")
                    where a.Attribute("Id").Value.Equals("0", StringComparison.InvariantCultureIgnoreCase)
                    select a).FirstOrDefault();
            if (itemParams.Elements("Parameter").Count() > 0)
                return (node != null);
            else
                return false;
        }

        private static bool HasCustomAttribute(XElement paramsNode)
        {
            return (paramsNode.Attribute("IsCustom") != null);
        }

        private static string CreatePropertyLateBindPrototypeString(Settings settings, XElement itemParams, bool interfaceHasEnumerator, bool hasDefaultItem)
        {
            string name = ParameterApi.ValidateNameWithoutVarType(itemParams.Parent.Attribute("Name").Value);
            string faceName = itemParams.Parent.Parent.Parent.Attribute("Name").Value;

            string result = "\r\n\t\t%paramDocu%";
            result += "\t\t%setAttribute%\r\n";
            string parameters = ParameterApi.CreateParametersPrototypeString(settings, itemParams, true, false,true);
            if (!string.IsNullOrEmpty(parameters))
                parameters = "(" + parameters + ")";
            string parametersCall = ParameterApi.CreateParametersCallString(settings, itemParams, true, false);
            int paramsCountWithOptionals = ParameterApi.GetParamsCount(itemParams, true);
            int paramsCountWithOutOptionals = ParameterApi.GetParamsCount(itemParams, false);
            bool hasForbiddenName = IsKeyword(name);
            string retValueType = VBGenerator.GetQualifiedType(itemParams.Element("ReturnValue"));

            string defaultPrefix = "";
            if (IsDefaultItem(itemParams))
                defaultPrefix = "Default ";

            if (HasCustomAttribute(itemParams))
                result += "\t\t" + "<CustomMethodAttribute> _" + "\r\n";

            if (IsMappedProperty(itemParams))
            {
                result += "\t\t" + defaultPrefix + "Public Function " + name + parameters + " As " + "%valueReturn%" + "\r\n";
                result += "\t\t\r\n\r\n%propertyGetBody%\r\n\r\n";
                result += "\t\tEnd Function\r\n";

                result += "\r\n\t\t%paramDocu%\r\n";
                result += "%specialHiddenAttribute%\t\t%setAttribute%\r\n";
                result += "\t\t" + defaultPrefix + "Public Function get_" + name + parameters + " As " + "%valueReturn%" + "\r\n";
                result += "\t\t\r\n\r\n%propertyGetBody%\r\n\r\n";
                result += "\t\tEnd Function\r\n";
            }
            else
            {
                if ((("INVOKE_PROPERTYGET" != itemParams.Parent.Attribute("InvokeKind").Value)))
                {
                    result += "\t\t" + defaultPrefix + "Public " + GetReadOnly(itemParams.Parent) + "Property " + name + parameters + " As " + "%valueReturn%" + "\r\n";
                    result += "\t\t\r\n\t\t\tGet\r\n\r\n%propertyGetBody%\r\n\t\t\tEnd Get\r\n\t\t\r\n";
                    result += "\t\t\tSet\r\n\r\n%propertySetBody%\r\n\t\t\tEnd Set\r\n\t\t\r\n";
                }
                else
                {
                    result += "\t\t" + defaultPrefix + "Public " + GetReadOnly(itemParams.Parent) + "Property " + name + parameters + " As " + "%valueReturn%" + "\r\n";
                    result += "\t\t\r\n\r\n\t\t\tGet\r\n\r\n%propertyGetBody%\r\n\t\t\tEnd Get\r\n\t\t\r\n\r\n";
                }
                result += "\t\tEnd Property\r\n";
            }
           
           
            return result;
        }

        private static bool IsMappedProperty(XElement itemParams)
        {
            if ("INVOKE_PROPERTYGET" == itemParams.Parent.Attribute("InvokeKind").Value)
            {
                if (!IsDefaultItem(itemParams))
                {
                    if (ParameterApi.MethodHasOptionalParams(itemParams.Parent))
                    {
                        if (ParameterApi.MethodHasNonOptionalParams(itemParams.Parent))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
 
        /// <summary>
        /// convert parametersNode to complete property body code
        /// </summary>
        /// <param name="numberOfRootTabs"></param>
        /// <param name="parametersNode"></param>
        /// <returns></returns>
        private static string CreatePropertyGetBody(Settings settings, int numberOfRootTabs, XElement parametersNode, bool analyzeMethod, string instance)
        {
            string result = CreatePropertyGetBodyCode(settings, 4, parametersNode, analyzeMethod, instance);
            return result;           
        }

        private static string CreatePropertySetBody(Settings settings, int numberOfRootTabs, XElement parametersNode, string instance)
        {
            string result = CreatePropertySetBodyCode(settings, 4, parametersNode, instance);
            return result;  
        }

        internal static string CreatePropertySetBodyCode(Settings settings, int numberOfRootTabs, XElement parametersNode, string instance)
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

                result += tabSpace + "Invoker.PropertySet(" + instance + ", \"" + invokeTarget + "\", paramsArray" + modifiers + ")\r\n";
            }
            else
            {
                string invokeTarget = "";
                if ("this" == parametersNode.Parent.Attribute("Name").Value)
                    invokeTarget = parametersNode.Parent.Attribute("Underlying").Value;
                else
                    invokeTarget = parametersNode.Parent.Attribute("Name").Value;

                result = ParameterApi.CreateParametersSetArrayString(settings, numberOfRootTabs, parametersNode, true);
                result += tabSpace + "Invoker.PropertySet(" + instance + ", \"" + invokeTarget + "\", paramsArray, value" + modifiers + ")\r\n";
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
        internal static string CreatePropertyGetBodyCode(Settings settings, int numberOfRootTabs, XElement parametersNode, bool analyzeReturn, string instance)
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

                methodBody += tabSpace + "Dim returnItem As Object = Invoker.PropertyGet(" + instance + ", \"" + invokeTarget + "\", paramsArray" + modifiers + ")\r\n";
                methodBody += tabSpace + "return returnItem\r\n\r\n";
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

                    methodBody += tabSpace + "Dim returnItem As Object = Invoker.PropertyGet(" + instance + ", \"" + invokeTarget + "\", paramsArray" + modifiers + ")\r\n";
                    if (typeName == "COMObject")
                    {
                        methodBody += tabSpace + "Dim" + " newObject " + arrayField + " As COMObject = NetOffice.Factory.CreateObject" + arrayName + "FromComProxy(" + instance + "," + objectArrayField + "returnItem)\r\n";
                        methodBody += "%modifiers%";
                        methodBody += tabSpace + "return newObject\r\n";
                    }
                    else if (typeName == "COMVariant")
                    {
                        methodBody += tabSpace + "If (Not NetOffice.Utils.IsNothing(returnItem)) And (TypeOf returnItem Is MarshalByRefObject)\r\n" + tabSpace + "\r\n";
                        if ("" == objectArrayField)
                            methodBody += tabSpace + "\tDim newObject" + arrayField + " As COMObject = NetOffice.Factory.CreateObject" + arrayName + "FromComProxy(" + instance + ", " + objectArrayField + "returnItem)\r\n";
                        else
                            methodBody += tabSpace + "\ttDim newObject" + arrayField + " As COMObject = NetOffice.Factory.CreateObject" + arrayName + "FromComProxy(" + instance + ", " + objectArrayField + "returnItem)\r\n";
                        methodBody += "\t%modifiers%";
                        methodBody += tabSpace + "return newObject\r\n";
                        methodBody += tabSpace + "\r\n";
                        methodBody += tabSpace + "Else" + "\r\n" + tabSpace + "\r\n";
                        methodBody += "\t%modifiers%";
                        methodBody += tabSpace + "return " + objectArrayField + " returnItem\r\n\r\n";
                        methodBody += tabSpace + "End If\r\n";
                    }
                    else
                    {
                        // library type
                        if ("true" == returnValue.Attribute("IsArray").Value)
                        {
                            methodBody += tabSpace + "Dim newObject() COMObject As = NetOffice.Factory.CreateObjectArrayFromComProxy(" + instance + "," + objectArrayField + "returnItem)\r\n";
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
                                methodBody += tabSpace + "Dim newObject As " + fullTypeName + " = NetOffice.Factory.CreateObjectFromComProxy(" + instance + "," + objectArrayField + "returnItem)\r\n";
                                methodBody += "%modifiers%";
                                methodBody += tabSpace + "return newObject\r\n";
                            }
                            else
                            {
                                if( (isDerived) && (!isDuplicated))
                                {
                                    methodBody += tabSpace + "Dim newObject As " + fullTypeName + " = NetOffice.Factory.CreateObjectFromComProxy(" + instance + "," + objectArrayField + "returnItem)\r\n";
                                    methodBody += "%modifiers%";
                                    methodBody += tabSpace + "return newObject\r\n";  
                                }
                                else
                                {
                                    string knownType = fullTypeName + ".LateBindingApiWrapperType";
                                    methodBody += tabSpace + "Dim newObject As " + fullTypeName + " = NetOffice.Factory.CreateKnownObjectFromComProxy("+ instance + "," + objectArrayField + "returnItem," + knownType + ")\r\n";
                                    methodBody += "%modifiers%";
                                    methodBody += tabSpace + "return newObject\r\n";
                                }
                            }
                        }
                    }
                }
                else
                {
                   
                    string invokeTarget = "";
                    if ("this" == parametersNode.Parent.Attribute("Name").Value)
                        invokeTarget = parametersNode.Parent.Attribute("Underlying").Value;
                    else
                        invokeTarget = methodName;

                    methodBody += tabSpace + "Dim returnItem As Object = " + "Invoker.PropertyGet" + "(" + instance + ", \"" + invokeTarget + "\", paramsArray)\r\n";
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

                methodBody += tabSpace + "Invoker.PropertyGet(" + instance + ", \"" + invokeTarget + "\", paramsArray" + modifiers + ")\r\n";
                methodBody += "";
            }

            return methodBody;
        }
    }
}
