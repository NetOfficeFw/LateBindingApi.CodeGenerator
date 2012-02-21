using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.VB
{
    public static class MethodApi
    {
        /// <summary>
        /// convert all methods to code as string
        /// </summary>
        /// <param name="methodsNode"></param>
        /// <returns></returns>
        internal static string ConvertMethodsLateBindToString(Settings settings, XElement methodsNode)
        {
            bool interfaceHasEnumerator = EnumerableApi.HasEnumerator(methodsNode.Parent);
            bool hasDefaultItem = EnumerableApi.HasDefaultItem(methodsNode.Parent);
            bool hasItem = EnumerableApi.HasItem(methodsNode.Parent);

            ParameterApi.ValidateItems(methodsNode, "Method", settings);

            string result = "\r\n\t\t#Region \"Methods\"\r\n\r\n";
            foreach (XElement methodNode in methodsNode.Elements("Method"))
            {
                if("_NewEnum" == methodNode.Attribute("Name").Value)
                    continue;

                string method = ConvertMethodLateBindToString(settings, methodNode, interfaceHasEnumerator, hasDefaultItem, hasItem);
                result += method;
            }
            result += "\t\t#End Region\r\n";
            return result;
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

        private static bool HasCustomAttribute(XElement paramsNode)
        {
            return (paramsNode.Attribute("IsCustom") != null);
        }

        /// <summary>
        /// convert method to code as string
        /// </summary>
        /// <param name="methodNode"></param>
        /// <returns></returns>
        internal static string ConvertMethodLateBindToString(Settings settings, XElement methodNode, bool interfaceHasEnumerator, bool hasDefaultItem, bool hasItem)
        {
            string result = "";
            string name = ParameterApi.ValidateName(methodNode.Attribute("Name").Value);
            bool analyzeReturn = Convert.ToBoolean(methodNode.Attribute("AnalyzeReturn").Value);
            foreach (XElement itemParams in methodNode.Elements("Parameters"))
            {
                XElement returnValue = itemParams.Element("ReturnValue");

                string[] supportDocuArray = VBGenerator.GetSupportByLibraryArray(itemParams);

                // gibt es andere überladungen mit mehr parametern als dieser überladung und sind die überzählen alle optional?
                // dann füge deren supportbylibray überladungen an
                List<XElement> otherOverloads = GetOverloadsWithMoreParameters(itemParams, methodNode.Elements("Parameters"));
                foreach (XElement other in otherOverloads)
                    supportDocuArray = DocumentationApi.AddParameterDocumentation(supportDocuArray, other);
                
                string supportDocu = DocumentationApi.CreateParameterDocumentationForMethod(2, supportDocuArray, itemParams);
                string supportAttribute = VBGenerator.GetSupportByLibraryAttribute(supportDocuArray, itemParams);
                 
                string method = "";
                if (true == settings.CreateXmlDocumentation)
                    method = supportDocu;

                if(methodNode.Attribute("Hidden").Value.Equals("true",StringComparison.InvariantCultureIgnoreCase))
                    method += "\t\t" + "<EditorBrowsable(EditorBrowsableState.Never), Browsable(false)> _" + "\r\n";

                if (HasCustomAttribute(itemParams))
                    method += "\t\t" + "<CustomMethodAttribute> _" + "\r\n";
                method += "\t\t" + supportAttribute + "\r\n";
                if ("this" == name)
                    method += "\t\t" + "<NetRuntimeSystem.Runtime.CompilerServices.IndexerName(\"Item\")> _" + "\r\n";

                string valueReturn = VBGenerator.GetQualifiedType(returnValue);
                if ("true" == returnValue.Attribute("IsArray").Value)
                    valueReturn += "()";
                if (!analyzeReturn)
                    valueReturn = "object";

                string methodType = "Function";
                if ("void" == valueReturn)
                    methodType = "Sub";

                string valReturnEnd = "";
                if ("void" != valueReturn)
                    valReturnEnd = " As " + ParameterApi.ValidateVarTypeVB(valueReturn);

                string thisPrefix = "";
                if(name =="this")
                {
                    thisPrefix = "Default Public ReadOnly Property";
                    methodType = "Property";
                }
                else
                    thisPrefix = "Public " + methodType;

                method += "\t\t" + thisPrefix + " " + name + "(" + "%params%" + ")" + valReturnEnd + "\r\n\t\t\r\n%methodbody%\t\tEnd " + methodType + "\r\n";

               // method += "\t\tPublic " + valueReturn + " " + name + inParam + "%params%" + outParam + "\r\n\t\t{\r\n%methodbody%\t\t}\r\n";
                string parameters = ParameterApi.CreateParametersPrototypeString(settings, itemParams, true, true,false);
                method = method.Replace("%params%", parameters);

                string methodBody = CreateLateBindMethodBody(settings, 3, itemParams, analyzeReturn);
                 
                method = method.Replace("%methodbody%", methodBody);
                result += method + "\r\n";
            }

            return result;
        }

        /// <summary>
        /// convert parametersNode to complete method body code
        /// </summary>
        /// <param name="numberOfRootTabs"></param>
        /// <param name="parametersNode"></param>
        /// <returns></returns>
        internal static string CreateLateBindMethodBody(Settings settings, int numberOfRootTabs, XElement parametersNode, bool analyzeReturn)
        {
            string tabSpace = VBGenerator.TabSpace(numberOfRootTabs);
            string methodBody    = ParameterApi.CreateParametersSetArrayString(settings, numberOfRootTabs, parametersNode, true);
            XElement returnValue = parametersNode.Element("ReturnValue");
            string methodName    = parametersNode.Parent.Attribute("Name").Value;            
            string typeName      = returnValue.Attribute("Type").Value;
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

            if (methodName == "this")
                methodBody = "Get\r\n" + methodBody;

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

                methodBody += tabSpace + "Dim returnItem As Object = Invoker.MethodReturn(Me, \"" + invokeTarget + "\", paramsArray" + modifiers + ")\r\n";
                methodBody += tabSpace + "Return returnItem\r\n";
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

                    methodBody += tabSpace + "Dim returnItem As Object = Invoker.MethodReturn(Me, \"" + invokeTarget + "\", paramsArray" + modifiers + ")\r\n";
                    if (typeName == "COMObject")
                    {
                        methodBody += tabSpace + "Dim newObject" + arrayField + " As COMObject = LateBindingApi.Core.Factory.CreateObject" + arrayName + "FromComProxy(Me," + objectArrayField + "returnItem)\r\n";
                        methodBody += "%modifiers%";
                        methodBody += tabSpace + "Return newObject\r\n";
                    }
                    else if (typeName == "COMVariant")
                    {

                        methodBody += tabSpace + "If (Not LateBindingApi.Core.Utils.IsNothing(returnItem)) And (TypeOf returnItem Is MarshalByRefObject)\r\n" + tabSpace + "\r\n";
                        if("" == objectArrayField)
                            methodBody += tabSpace + "\tDim newObject" + arrayField + " As COMObject = LateBindingApi.Core.Factory.CreateObject" + arrayName + "FromComProxy(Me, " + objectArrayField + "returnItem)\r\n";
                        else
                            methodBody += tabSpace + "\ttDim newObject" + arrayField + " As COMObject = LateBindingApi.Core.Factory.CreateObject" + arrayName + "FromComProxy(Me, " + objectArrayField + "returnItem)\r\n";
                        methodBody += "\t%modifiers%";
                        methodBody += tabSpace + "Return newObject\r\n";
                        methodBody += tabSpace + "\r\n";
                        methodBody += tabSpace + "Else" + "\r\n" + tabSpace +"\r\n";
                        methodBody += "\t%modifiers%";
                        methodBody += tabSpace + "Return " + objectArrayField + " returnItem\r\n";
                        methodBody += tabSpace + " End If\r\n";
                    }
                    else
                    {                        
                        // library type
                        if ("true" == returnValue.Attribute("IsArray").Value)
                        {
                             methodBody += tabSpace + "Dim newObject() As COMObject = LateBindingApi.Core.Factory.CreateObjectArrayFromComProxy(Me, " + objectArrayField + "returnItem)\r\n";
                             methodBody += tabSpace + fullTypeName + " returnArray = new " + VBGenerator.GetQualifiedType(returnValue) + "(newObject.Length)\r\n";
                             methodBody += tabSpace + "For i As Integer = 0 To newObject.Length\r\n";
                             methodBody += tabSpace + "\treturnArray(i) = newObject(i) as " + VBGenerator.GetQualifiedType(returnValue) + "\r\n";
                             methodBody += tabSpace + "Next" + "\r\n";
                             methodBody += "%modifiers%";
                             methodBody += tabSpace + "Return returnArray\r\n";                            
                        }
                        else
                        {
                            bool isFromIgnoreProject = VBGenerator.IsFromIgnoreProject(returnValue);
                            bool isDuplicated = VBGenerator.IsDuplicatedReturnValue(returnValue);
                            bool isDerived = VBGenerator.IsDerivedReturnValue(returnValue);
                            if ((true == isFromIgnoreProject) && (false == isDuplicated))
                            {
                                methodBody += tabSpace + "Dim newObject As " + fullTypeName + " = LateBindingApi.Core.Factory.CreateObjectFromComProxy(Me, " + objectArrayField + "returnItem)\r\n";
                                methodBody += "%modifiers%";
                                methodBody += tabSpace + "Return newObject\r\n";
                            }
                            else
                            {
                                if ((isDerived) && (!isDuplicated))
                                {
                                    methodBody += tabSpace + "Dim newObject As " + fullTypeName + " = LateBindingApi.Core.Factory.CreateObjectFromComProxy(Me," + objectArrayField + "returnItem)" + "\r\n";
                                    methodBody += "%modifiers%";
                                    methodBody += tabSpace + "Return newObject\r\n";  
                                }
                                else
                                {
                                    string knownType = fullTypeName + ".LateBindingApiWrapperType";
                                    methodBody += tabSpace + "Dim newObject As " + fullTypeName + " = LateBindingApi.Core.Factory.CreateKnownObjectFromComProxy(Me, " + objectArrayField + "returnItem," + knownType + ")" + "\r\n";
                                    methodBody += "%modifiers%";
                                    methodBody += tabSpace + "Return newObject\r\n";
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

                    methodBody += tabSpace +  "Dim returnItem As Object = " + "Invoker.MethodReturn" + "(Me, \"" + invokeTarget + "\", paramsArray)\r\n";
                    methodBody += "%modifiers%";
                    methodBody += tabSpace + "return returnItem\r\n";
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

                methodBody += tabSpace + "Invoker.Method(Me, \"" + invokeTarget + "\", paramsArray" + modifiers + ")\r\n";
                methodBody += "%modifiers%";

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
            if (methodName == "this")
                methodBody += "End Get\r\n";
            return methodBody;
        }

        internal static string ConvertMethodEarlyBindToString(Settings settings, XElement methodNode)
        {
            string result = "";
           // string name = "Function " + methodNode.Attribute("Name").Value;
            foreach (XElement itemParams in methodNode.Elements("Parameters"))
            {
                XElement returnValue = itemParams.Element("ReturnValue");

                string method = "";
                method += "\t\t" + VBGenerator.GetSupportByLibraryAttribute(itemParams) + "\r\n";
                /*
                string marshalReturnAs = returnValue.Attribute("MarshalAs").Value;
                if ("" != marshalReturnAs)
                    method += "\t\t[return: MarshalAs(" + marshalReturnAs + ")]\r\n";
                */
                method += "\t\t" + "<MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime), DispId(" + itemParams.Parent.Element("DispIds").Element("DispId").Attribute("Id").Value + ")> _\r\n";
                method += "\t\t" + GetEarlyBindMethodSignatur(itemParams) + "\r\n\r\n";
                 
                result += method;
            }
          

            return result;
        }

        internal static string GetEarlyBindMethodSignatur(XElement paramsNode)
        {
            string methodType = "Sub ";
            if(paramsNode.Element("ReturnValue").Attribute("Type").Value != "void")
                methodType = "Function ";
            
            string result = methodType + paramsNode.Parent.Attribute("Name").Value + "(";
            foreach (XElement itemParam in paramsNode.Elements("Parameter"))
            {
                string isRef = "";
                if ("true" == itemParam.Attribute("IsRef").Value)
                    isRef = "ByRef ";
                else
                    isRef = "ByVal ";

                string par = "<[In]()";
                string marshalAs = itemParam.Attribute("MarshalAs").Value;
                if ("" != marshalAs)
                    marshalAs = ", MarshalAs(" + marshalAs + ")>";

                if (("true" == itemParam.Attribute("IsComProxy").Value) || ("COMObject" == itemParam.Attribute("Type").Value) ||
                    ("COMObject" == itemParam.Attribute("Type").Value) || ("object" == itemParam.Attribute("Type").Value))
                {
                    par += marshalAs + " object " + itemParam.Attribute("Name").Value;
                }
                else
                {
                    par += marshalAs + isRef + ParameterApi.ValidateName(itemParam.Attribute("Name").Value) + " As " + itemParam.Attribute("Type").Value;
                }

                result += par + ", ";
            }
            if (", " == result.Substring(result.Length - 2))
                result = result.Substring(0, result.Length - 2);

            result += ")";

            if (paramsNode.Element("ReturnValue").Attribute("Type").Value != "void")
                result += " As " + paramsNode.Element("ReturnValue").Attribute("Type").Value;

            return result;
        }

        internal static string ConvertMethodsEarlyBindToString(Settings settings, XElement methodsNode)
        {
            string result = "\t\t#region \"Methods\"\r\n\r\n";
            foreach (XElement methodNode in methodsNode.Elements("Method"))
            {
                if ("_NewEnum" == methodNode.Attribute("Name").Value)
                    continue;

                string method = ConvertMethodEarlyBindToString(settings, methodNode);
                result += method;
            }
            result += "\t\t#End Region\r\n";
            return result;
        }
    }
}
