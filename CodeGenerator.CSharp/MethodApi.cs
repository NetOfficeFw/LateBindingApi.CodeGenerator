using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    public static class MethodApi
    {
        /// <summary>
        /// convert all methods to code as string
        /// </summary>
        /// <param name="methodsNode"></param>
        /// <returns></returns>
        internal static string ConvertConflictMethodsLateBindToString(Settings settings, XElement methodsNode)
        {
            bool interfaceHasEnumerator = EnumerableApi.HasEnumerator(methodsNode.Parent);
            bool hasDefaultItem = EnumerableApi.HasDefaultItem(methodsNode.Parent);

            ParameterApi.ValidateItems(methodsNode, "Method", settings);

            string result = "\r\n\t\t#region Methods\r\n\r\n";
            foreach (XElement methodNode in methodsNode.Elements("Method"))
            {
                if ("this" == methodNode.Attribute("Name").Value)
                    continue;

                if ("_NewEnum" == methodNode.Attribute("Name").Value)
                    continue;

                if (("Item" == methodNode.Attribute("Name").Value) && HasThis(methodsNode.Parent))
                    continue;

                string method = ConvertConflictMethodLateBindToString(settings, methodNode, interfaceHasEnumerator, hasDefaultItem);
                result += method;
               
            }

            result += "\t\t#endregion\r\n";
            return result;
        }

        /// <summary>
        /// convert all methods to code as string
        /// </summary>
        /// <param name="methodsNode"></param>
        /// <returns></returns>
        internal static string ConvertMethodsLateBindToString(Settings settings, XElement methodsNode)
        {
            bool interfaceHasEnumerator = EnumerableApi.HasEnumerator(methodsNode.Parent);
            bool hasDefaultItem = EnumerableApi.HasDefaultItem(methodsNode.Parent);

            ParameterApi.ValidateItems(methodsNode, "Method", settings);

            string result = "\r\n\t\t#region Methods\r\n\r\n";
            foreach (XElement methodNode in methodsNode.Elements("Method"))
            {
                if("_NewEnum" == methodNode.Attribute("Name").Value)
                    continue;

                if (("Item" == methodNode.Attribute("Name").Value) && HasThis(methodsNode.Parent))
                    continue;

                bool isNameConflict = false;
                foreach (XAttribute item in methodNode.Attributes())
                {
                    if (item.Name == "IsNameConflict" && item.Value == "true")
                    {
                        isNameConflict = true;
                        break;
                    }

                }

                bool isOptionalConflict = false;
                foreach (XAttribute item in methodNode.Attributes())
                {
                    if (item.Name == "IsOptionalConflict" && item.Value == "true")
                    {
                        isOptionalConflict = true;
                        break;
                    }
                }

                if (!isNameConflict && !isOptionalConflict)
                {
                    string method = ConvertMethodLateBindToString(settings, methodNode, interfaceHasEnumerator, hasDefaultItem);
                    result += method;
                }
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
        internal static string ConvertConflictMethodLateBindToString(Settings settings, XElement methodNode, bool interfaceHasEnumerator, bool hasDefaultItem)
        {
            string result = "";
            string name = methodNode.Attribute("Name").Value;
            bool analyzeReturn = Convert.ToBoolean(methodNode.Attribute("AnalyzeReturn").Value);
            foreach (XElement itemParams in methodNode.Elements("Parameters"))
            {
                if (("this" == name) && itemParams.HasAttributes)
                    continue;

                bool isOptionalConflict = false;
                if (!CSharpGenerator.Settings.VBOptimization)
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

                string inParam = "(";
                string outParam = ")";

                if ("this" == name)
                {
                    inParam = "[";
                    outParam = "]";
                }

                XElement returnValue = itemParams.Element("ReturnValue");

                string[] supportDocuArray = CSharpGenerator.GetSupportByVersionArray(itemParams);

                // gibt es andere überladungen mit mehr parametern als dieser überladung und sind die überzählen alle optional?
                // dann füge deren supportbylibray überladungen an
                List<XElement> otherOverloads = GetOverloadsWithMoreParameters(itemParams, methodNode.Elements("Parameters"));
                foreach (XElement other in otherOverloads)
                    supportDocuArray = DocumentationApi.AddParameterDocumentation(supportDocuArray, other);
                
                string docLink = "";
                if (CSharpGenerator.Settings.AddDocumentationLinks)
                {
                    XElement typeNode = GetTypeNode(methodNode);
                    XElement projectNode = GetProjectNode(methodNode);

                    string projectName = projectNode.Attribute("Name").Value;
                    if (null != projectName && CSharpGenerator.IsRootProjectName(projectName))
                    {
                        XElement typeDocNode = (from a in CSharpGenerator.LinkFileDocument.Element("NOBuildTools.ReferenceAnalyzer").Element(projectName).Element("Types").Elements("Type")
                                                where a.Element("Name").Value.Equals(typeNode.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase)
                                                select a).FirstOrDefault();
                        
                        if (null == typeDocNode && typeNode.Attribute("Name").Value.StartsWith("_"))
                        {
                            string typeName = typeNode.Attribute("Name").Value.Substring(1);
                            typeDocNode = (from a in CSharpGenerator.LinkFileDocument.Element("NOBuildTools.ReferenceAnalyzer").Element(projectName).Element("Types").Elements("Type")
                                           where a.Element("Name").Value.Equals(typeName, StringComparison.InvariantCultureIgnoreCase)
                                           select a).FirstOrDefault();
                        }

                        if (null != typeDocNode)
                        {
                            XElement linkNode = (from a in typeDocNode.Element("Methods").Elements("Method")
                                                 where a.Element("Name").Value.Equals(methodNode.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase)
                                                 select a).FirstOrDefault();

                            if (null != linkNode)
                                docLink = "MSDN Online Documentation: " + linkNode.Element("Link").Value;
                        }
                    }
                }

                string supportDocu = DocumentationApi.CreateParameterDocumentationForMethod(2, supportDocuArray, itemParams, docLink);
               
                string supportAttribute = CSharpGenerator.GetSupportByVersionAttribute(supportDocuArray, itemParams);

                string method = "";
                if (true == settings.CreateXmlDocumentation)
                    method = supportDocu;

                int paramsCountWithOptionals = ParameterApi.GetParamsCount(itemParams, true);

                if (methodNode.Attribute("Hidden").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    method += "\t\t" + "[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]" + "\r\n";

                if (HasCustomAttribute(itemParams))
                    method += "\t\t" + "[CustomMethodAttribute]" + "\r\n";
                method += "\t\t" + supportAttribute + "\r\n";
                if ("this" == name)
                    method += "\t\t" + "[NetRuntimeSystem.Runtime.CompilerServices.IndexerName(\"Item\")]" + "\r\n";

                string valueReturn = CSharpGenerator.GetQualifiedType(returnValue);
                if (valueReturn == "COMObject")
                {
                    valueReturn = "object";
                }

                if ("true" == returnValue.Attribute("IsArray").Value)
                    valueReturn += "[]";
                if (!analyzeReturn)
                    valueReturn = "object";

                method += "\t\tpublic " + valueReturn + " " + name + inParam + "%params%" + outParam + "\r\n\t\t{\r\n%methodbody%\t\t}\r\n";
                string parameters = ParameterApi.CreateParametersPrototypeString(settings, itemParams, true, true);
                method = method.Replace("%params%", parameters);

                string methodBody = "";
                if ("this" == name)
                    methodBody = "\t\t\tget\r\n\t\t\t{\r\n" + CreateLateBindMethodBody(settings, 4, itemParams, analyzeReturn) + "\t\t\t}\r\n";
                else
                    methodBody = CreateLateBindMethodBody(settings, 3, itemParams, analyzeReturn);

                method = method.Replace("%methodbody%", methodBody);
                result += method + "\r\n";
            }

            return result;
        }

        private static XElement GetTypeNode(XElement methodNode)
        {
            XElement parentNode = methodNode;
            while (parentNode.Name != "Interface")
                parentNode = parentNode.Parent;

            return parentNode;
        }

        private static XElement GetProjectNode(XElement methodNode)
        {
            XElement parentNode = methodNode;
            while (parentNode.Name != "Project")
                parentNode = parentNode.Parent;

            return parentNode;
        }

        /// <summary>
        /// convert method to code as string
        /// </summary>
        /// <param name="methodNode"></param>
        /// <returns></returns>
        internal static string ConvertMethodLateBindToString(Settings settings, XElement methodNode, bool interfaceHasEnumerator, bool hasDefaultItem)
        {
            string result = "";
            string name = methodNode.Attribute("Name").Value;
            bool analyzeReturn = Convert.ToBoolean(methodNode.Attribute("AnalyzeReturn").Value);
            foreach (XElement itemParams in methodNode.Elements("Parameters"))
            {
                if (("this" == name) && itemParams.HasAttributes)
                    continue;

                string inParam = "(";
                string outParam = ")";

                if("this" == name)
                {
                    inParam = "[";
                    outParam = "]";
                }
                
                XElement returnValue = itemParams.Element("ReturnValue");

                string[] supportDocuArray = CSharpGenerator.GetSupportByVersionArray(itemParams);

                // gibt es andere überladungen mit mehr parametern als dieser überladung und sind die überzählen alle optional?
                // dann füge deren supportbylibray überladungen an
                List<XElement> otherOverloads = GetOverloadsWithMoreParameters(itemParams, methodNode.Elements("Parameters"));
                foreach (XElement other in otherOverloads)
                    supportDocuArray = DocumentationApi.AddParameterDocumentation(supportDocuArray, other);

                string docLink = "";
                if (CSharpGenerator.Settings.AddDocumentationLinks)
                {
                    XElement typeNode = GetTypeNode(methodNode);
                    XElement projectNode = GetProjectNode(methodNode);
                    string projectName = projectNode.Attribute("Name").Value;
                    if (null != projectName && CSharpGenerator.IsRootProjectName(projectName))
                    {
                        XElement typeDocNode = (from a in CSharpGenerator.LinkFileDocument.Element("NOBuildTools.ReferenceAnalyzer").Element(projectName).Element("Types").Elements("Type")
                                                where a.Element("Name").Value.Equals(typeNode.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase)
                                                select a).FirstOrDefault();
                        if (null == typeDocNode && typeNode.Attribute("Name").Value.StartsWith("_"))
                        {
                            string typeName = typeNode.Attribute("Name").Value.Substring(1);
                            typeDocNode = (from a in CSharpGenerator.LinkFileDocument.Element("NOBuildTools.ReferenceAnalyzer").Element(projectName).Element("Types").Elements("Type")
                                           where a.Element("Name").Value.Equals(typeName, StringComparison.InvariantCultureIgnoreCase)
                                           select a).FirstOrDefault();
                        }

                        if (null != typeDocNode)
                        {
                            XElement linkNode = (from a in typeDocNode.Element("Methods").Elements("Method")
                                                 where a.Element("Name").Value.Equals(methodNode.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase)
                                                 select a).FirstOrDefault();

                            if (null != linkNode)
                                docLink = "MSDN Online Documentation: " + linkNode.Element("Link").Value;
                        }
                    }
                }
               

                string supportDocu = DocumentationApi.CreateParameterDocumentationForMethod(2, supportDocuArray, itemParams, docLink);
                string supportAttribute = CSharpGenerator.GetSupportByVersionAttribute(supportDocuArray, itemParams);
                 
                string method = "";
                if (true == settings.CreateXmlDocumentation)
                    method = supportDocu;

                int paramsCountWithOptionals = ParameterApi.GetParamsCount(itemParams, true);

                if(methodNode.Attribute("Hidden").Value.Equals("true",StringComparison.InvariantCultureIgnoreCase))
                    method += "\t\t" + "[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]" + "\r\n";
               
                if (HasCustomAttribute(itemParams))
                    method += "\t\t" + "[CustomMethodAttribute]" + "\r\n";
                method += "\t\t" + supportAttribute + "\r\n";
                if ("this" == name)
                    method += "\t\t" + "[NetRuntimeSystem.Runtime.CompilerServices.IndexerName(\"Item\")]" + "\r\n";

                string valueReturn = CSharpGenerator.GetQualifiedType(returnValue);
                if (valueReturn == "COMObject")
                    valueReturn = "object";

                if ("true" == returnValue.Attribute("IsArray").Value)
                    valueReturn += "[]";
                if (!analyzeReturn)
                    valueReturn = "object";

                method += "\t\tpublic " + valueReturn + " " + name + inParam + "%params%" + outParam + "\r\n\t\t{\r\n%methodbody%\t\t}\r\n";
                string parameters = ParameterApi.CreateParametersPrototypeString(settings, itemParams, true, true);
                method = method.Replace("%params%", parameters);

                string methodBody ="";
                if ("this" == name)
                    methodBody = "\t\t\tget\r\n\t\t\t{\r\n" + CreateLateBindMethodBody(settings, 4, itemParams, analyzeReturn) + "\t\t\t}\r\n";
                else
                    methodBody = CreateLateBindMethodBody(settings, 3, itemParams, analyzeReturn);
                
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
            string tabSpace      = CSharpGenerator.TabSpace(numberOfRootTabs);
            string methodBody    = ParameterApi.CreateParametersSetArrayString(settings, numberOfRootTabs, parametersNode, true);
            XElement returnValue = parametersNode.Element("ReturnValue");
            string methodName    = parametersNode.Parent.Attribute("Name").Value;
            string typeName      = returnValue.Attribute("Type").Value;
 
            string fullTypeName = CSharpGenerator.GetQualifiedType(returnValue);

            if ("this" == methodName)
                methodName = parametersNode.Parent.Attribute("Underlying").Value;

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

                methodBody += tabSpace + "object returnItem = Invoker.MethodReturn(this, \"" + invokeTarget + "\", paramsArray" + modifiers + ");\r\n";
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

                    methodBody += tabSpace + "object returnItem = Invoker.MethodReturn(this, \"" + invokeTarget + "\", paramsArray" + modifiers + ");\r\n";
                    if (typeName == "COMObject")
                    {
                        methodBody += tabSpace + "object" + arrayField + " newObject = Factory.CreateObject" + arrayName + "FromComProxy(this," + objectArrayField + "returnItem);\r\n";
                        methodBody += "%modifiers%";
                        methodBody += tabSpace + "return newObject;\r\n";
                    }
                    else if (typeName == "COMVariant")
                    {
                        methodBody += tabSpace + "if((null != returnItem) && (returnItem is MarshalByRefObject))\r\n" + tabSpace + "{\r\n";
                        if("" == objectArrayField)
                            methodBody += tabSpace + "\tICOMObject" + arrayField + " newObject = Factory.CreateObject" + arrayName + "FromComProxy(this, " + objectArrayField + "returnItem);\r\n";
                        else
                            methodBody += tabSpace + "\tICOMObject" + arrayField + " newObject = Factory.CreateObject" + arrayName + "FromComProxy(this, " + objectArrayField + "returnItem);\r\n";
                        methodBody += "\t%modifiers%";
                        methodBody += tabSpace + "return newObject;\r\n";
                        methodBody += tabSpace + "}\r\n";
                        methodBody += tabSpace + "else" + "\r\n" + tabSpace +"{\r\n";
                        methodBody += "\t%modifiers%";
                        methodBody += tabSpace + "return " + objectArrayField + " returnItem;\r\n";
                        methodBody += tabSpace + "}\r\n";
                    }
                    else
                    {                        
                        // library type
                        if ("true" == returnValue.Attribute("IsArray").Value)
                        {
                            methodBody += tabSpace + "ICOMObject[] newObject = Factory.CreateObjectArrayFromComProxy(this, " + objectArrayField + "returnItem);\r\n";
                             methodBody += tabSpace + fullTypeName + " returnArray = new " + CSharpGenerator.GetQualifiedType(returnValue) + "[newObject.Length];\r\n";
                             methodBody += tabSpace + "for (int i = 0; i < newObject.Length; i++)\r\n";
                             methodBody += tabSpace + "\treturnArray[i] = newObject[i] as " + CSharpGenerator.GetQualifiedType(returnValue) + ";\r\n";
                             methodBody += "%modifiers%";
                             methodBody += tabSpace + "return returnArray;\r\n";                            
                        }
                        else
                        {       
                            bool isFromIgnoreProject = CSharpGenerator.IsFromIgnoreProject(returnValue);
                            bool isDuplicated = CSharpGenerator.IsDuplicatedReturnValue(returnValue);
                            bool isDerived = CSharpGenerator.IsDerivedReturnValue(returnValue);
                            if ((true == isFromIgnoreProject) && (false == isDuplicated))
                            {
                                methodBody += tabSpace + fullTypeName + " newObject = Factory.CreateObjectFromComProxy(this, " + objectArrayField + "returnItem) as " + fullTypeName + ";\r\n";
                                methodBody += "%modifiers%";
                                methodBody += tabSpace + "return newObject;\r\n";
                            }
                            else
                            {
                                if ((isDerived) && (!isDuplicated))
                                {
                                    methodBody += tabSpace + fullTypeName + " newObject = Factory.CreateObjectFromComProxy(this," + objectArrayField + "returnItem) as " + fullTypeName + ";\r\n";
                                    methodBody += "%modifiers%";
                                    methodBody += tabSpace + "return newObject;\r\n";  
                                }
                                else
                                {
                                    string knownType = fullTypeName + ".LateBindingApiWrapperType";
                                    methodBody += tabSpace + fullTypeName + " newObject = Factory.CreateKnownObjectFromComProxy(this, " + objectArrayField + "returnItem," + knownType + ") as " + fullTypeName + ";\r\n";
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

                    methodBody += tabSpace + "object" + " returnItem = " + objectString + "Invoker.MethodReturn" + "(this, \"" + invokeTarget + "\", paramsArray);\r\n";
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
                string invokeTarget = "";
                if ("this" == parametersNode.Parent.Attribute("Name").Value)
                    invokeTarget = parametersNode.Parent.Attribute("Underlying").Value;
                else
                    invokeTarget = methodName;

                methodBody += tabSpace + "Invoker.Method(this, \"" + invokeTarget + "\", paramsArray" + modifiers + ");\r\n";
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
            return methodBody;
        }
          
        internal static string ConvertMethodEarlyBindToString(Settings settings, XElement methodNode)
        {
            string result = "";
            string name = methodNode.Attribute("Name").Value;
            foreach (XElement itemParams in methodNode.Elements("Parameters"))
            {
                XElement returnValue = itemParams.Element("ReturnValue");

                string method = "";
                method += "\t\t" + CSharpGenerator.GetSupportByVersionAttribute(itemParams) + "\r\n";

                string marshalReturnAs = returnValue.Attribute("MarshalAs").Value;
                if ("" != marshalReturnAs)
                    method += "\t\t[return: MarshalAs(" + marshalReturnAs + ")]\r\n";

                method += "\t\t" + "[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime), DispId(" + itemParams.Parent.Element("DispIds").Element("DispId").Attribute("Id").Value + ")]\r\n";
                method += "\t\t" + GetEarlyBindMethodSignatur(itemParams) + ";\r\n\r\n";
                 
                result += method;
            }

            return result;
        }

        internal static string GetEarlyBindMethodSignatur(XElement paramsNode)
        {
            string result = paramsNode.Element("ReturnValue").Attribute("Type").Value + " " + paramsNode.Parent.Attribute("Name").Value + "(";
            foreach (XElement itemParam in paramsNode.Elements("Parameter"))
            {
                string isRef = "";
                if ("true" == itemParam.Attribute("IsRef").Value)
                    isRef = "ref ";

                string par = "[In";
                string marshalAs = itemParam.Attribute("MarshalAs").Value;
                if ("" != marshalAs)
                    marshalAs = ", MarshalAs(" + marshalAs + ")]";

                if (("true" == itemParam.Attribute("IsComProxy").Value) || ("COMObject" == itemParam.Attribute("Type").Value) ||
                    ("COMObject" == itemParam.Attribute("Type").Value) || ("object" == itemParam.Attribute("Type").Value))
                {
                    par += marshalAs + " object " + itemParam.Attribute("Name").Value;
                }
                else
                {
                    par += marshalAs + isRef + itemParam.Attribute("Type").Value + " " + itemParam.Attribute("Name").Value;
                }

                result += par + ", ";
            }
            if (", " == result.Substring(result.Length - 2))
                result = result.Substring(0, result.Length - 2);

            return result + ")";
        }

        internal static string ConvertMethodsEarlyBindToString(Settings settings, XElement methodsNode)
        {
            string result = "\t\t#region Methods\r\n\r\n";
            foreach (XElement methodNode in methodsNode.Elements("Method"))
            {
                if ("_NewEnum" == methodNode.Attribute("Name").Value)
                    continue;

                string method = ConvertMethodEarlyBindToString(settings, methodNode);
                result += method;
            }
            result += "\t\t#endregion\r\n";
            return result;
        }
    }
}
