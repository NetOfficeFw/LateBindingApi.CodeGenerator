using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    public static class EventApi
    {
        public static readonly string FolderName = "Events";

        private static readonly string SinkArgumentTemplate = "[SinkArgument(\"{0}\", {1})]";
        private static readonly string NetOfficeNamespacePrefix = "NetOffice.";

        private static string _interfaceFile;

        internal static string ConvertInterfacesToFiles(XElement projectNode, XElement dispatchfacesNode, XElement facesNode, Settings settings, string solutionFolder)
        {
            if (null == _interfaceFile)
                _interfaceFile = RessourceApi.ReadString("Event.Interface.txt");

            string eventsFolder = Path.Combine(solutionFolder, projectNode.Attribute("Name").Value, FolderName);
            DirectoryEx.EnsureDirectory(eventsFolder);

            string result = "";
            foreach (XElement faceNode in dispatchfacesNode.Elements("Interface"))
            {
                if ("true" == faceNode.Attribute("IsEventInterface").Value)
                    result += ConvertInterfaceToFile(settings, projectNode, faceNode, eventsFolder) + "\r\n";
            }

            foreach (XElement faceNode in facesNode.Elements("Interface"))
            {
                if ("true" == faceNode.Attribute("IsEventInterface").Value)
                    result += ConvertInterfaceToFile(settings, projectNode, faceNode, eventsFolder) + "\r\n";
            }

            return result;
        }

        private static string ConvertInterfaceToFile(Settings settings, XElement projectNode, XElement faceNode, string faceFolder)
        {
            string fileName = Path.Combine(faceFolder, faceNode.Attribute("Name").Value + ".cs");

            string newEnum = ConvertInterfaceToString(settings, projectNode, faceNode);
            File.WriteAllText(fileName, newEnum, Constants.UTF8WithBOM);

            int i = faceFolder.LastIndexOf("\\");
            string result = "    <Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + faceNode.Attribute("Name").Value + ".cs" + "\" />";
            return result;
        }

        private static string ConvertInterfaceToString(Settings settings, XElement projectNode, XElement faceNode)
        {
            var namespaceName = projectNode.Attribute("Namespace").Value + ".Events";
            string result = _interfaceFile.Replace("%namespace%", namespaceName);
            result = result.Replace("%supportby%", CSharpGenerator.GetSupportByVersionAttribute(faceNode));
            result = result.Replace("%name%", faceNode.Attribute("Name").Value);
            result = result.Replace("%guid%", XmlConvert.DecodeName(faceNode.Element("DispIds").Element("DispId").Attribute("Id").Value));

            var tabSpace = CSharpGenerator.TabSpace(2);
            string methodResult = "";
            string implementResult = "";
            foreach (XElement itemMethod in faceNode.Element("Methods").Elements("Method"))
            {
                methodResult += "\t\t" + CSharpGenerator.GetSupportByVersionAttribute(itemMethod) + "\r\n";
                if (ParameterApi.HasParams(itemMethod))
                {
                    string sinkArguments = GetSinkArgumentsForParameters(itemMethod);
                    methodResult += sinkArguments;
                }
                methodResult += "\t\t" + "[PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(" + itemMethod.Element("DispIds").Element("DispId").Attribute("Id").Value + ")]\r\n";
                methodResult += "\t\t" + GetEventMethodSignature(settings, itemMethod, false) + ";\r\n\r\n";
                implementResult += tabSpace + GetEventMethodSignature(settings, itemMethod, true) + "\r\n";
                implementResult += tabSpace + "{\r\n" + GetMethodImplementCode(settings, itemMethod) + tabSpace + "}\r\n\r\n";
            }
            
            if("" !=methodResult)
                methodResult = methodResult.Substring(0, methodResult.Length - "\r\n".Length);
    
            result = result.Replace("%methods%", methodResult);
            result = result.Replace("%methodsImplement%", implementResult);
            return result;
        }

        internal static string GetEventMethodSignature(Settings settings, XElement methodNode, bool withPublic)
        {
            string publicModifier ="";
            if (true == withPublic)
                publicModifier = "public ";

            string result = publicModifier + "void " + methodNode.Attribute("Name").Value + "("; 
            foreach (XElement itemParam in methodNode.Element("Parameters").Elements("Parameter"))
            {
                string isRef = "";
                if ("true" == itemParam.Attribute("IsRef").Value)
                    isRef = "ref ";

                string par = "";
                if ("true" == itemParam.Attribute("IsRef").Value)
                    par = "[In] [Out";
                else
                    par = "[In";

                //if("newVal".Equals(itemParam.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase))
                //{
                
                //}

                bool isComProxy = IsComProxy(itemParam);
                if (isComProxy)
                {
                    par += ", MarshalAs(UnmanagedType.IDispatch)] object " + ParameterApi.ValidateParamName(itemParam.Attribute("Name").Value);
                }
                else
                {
                    par += "] " + isRef + "object " + ParameterApi.ValidateParamName(itemParam.Attribute("Name").Value);
                }

                result += par +", ";
            }
            if (", " == result.Substring(result.Length - 2))
                result = result.Substring(0, result.Length - 2);

            return result+ ")";
        }

        private static string GetSinkArgumentsForParameters(XElement itemMethod)
        {
            var tabSpace = CSharpGenerator.TabSpace(2);
            var attributes = new StringBuilder(512);

            var parametersNode = itemMethod.Element("Parameters");
            foreach (XElement itemParam in ParameterApi.GetParameter(parametersNode, true))
            {
                string parameterName = ParameterApi.ValidateParamName(itemParam.Attribute("Name").Value);
                string parameterType = "";

                if (IsComProxy(itemParam))
                {
                    if (itemParam.Attribute("Type").Value == "object")
                    {
                        parameterType = "SinkArgumentType.UnknownProxy";
                    }
                    else
                    {
                        string qualifiedType = CSharpGenerator.GetQualifiedType(itemParam);
                        if (qualifiedType.StartsWith(NetOfficeNamespacePrefix))
                        {
                            qualifiedType = qualifiedType.Substring(NetOfficeNamespacePrefix.Length);
                        }

                        parameterType = "typeof(" + qualifiedType + ")";
                    }
                }
                else
                {
                    string typeName = itemParam.Attribute("Type").Value;
                    if ("true" == itemParam.Attribute("IsEnum").Value)
                    {
                        string qualifiedType = CSharpGenerator.GetQualifiedType(itemParam);
                        if (qualifiedType.StartsWith(NetOfficeNamespacePrefix))
                        {
                            qualifiedType = qualifiedType.Substring(NetOfficeNamespacePrefix.Length);
                        }

                        parameterType = "SinkArgumentType.Enum, typeof(" + qualifiedType + ")";
                    }
                    else
                    {
                        switch (typeName)
                        {
                            case "bool":
                                parameterType = "SinkArgumentType.Bool";
                                break;
                            case "Int16":
                                parameterType = "SinkArgumentType.Int16";
                                break;
                            case "Int32":
                                parameterType = "SinkArgumentType.Int32";
                                break;
                            case "Single":
                                parameterType = "SinkArgumentType.Single";
                                break;
                            case "string":
                                parameterType = "SinkArgumentType.String";
                                break;
                            default:
                                parameterType = "SinkArgumentType.UnknownProxy";
                                break;
                        }
                    }
                }

                string attribute = string.Format(SinkArgumentTemplate, parameterName, parameterType);
                attributes.Append(tabSpace);
                attributes.AppendLine(attribute);
            }

            return attributes.ToString();
        }

        internal static bool IsStruct(XElement itemParam)
        {
            if ("VT_VARIANT" == itemParam.Attribute("VarType").Value && itemParam.Attribute("MarshalAs").Value == "UnmanagedType.Struct")
                return true;
            else
                return false;
        }

        internal static bool IsEarlyBindInterface(XElement itemParam)
        {
            XElement node = CSharpGenerator.GetInterfaceOrClassFromKeyAndName(itemParam);
            if (node == null || null == node.Attribute("IsEarlyBind"))
                return false;

            if (node.Attribute("IsEarlyBind").Value == "true")
                 return true;
            else
                return false;
        }

        internal static bool IsComProxy(XElement itemParam)
        {
            if ("VT_VARIANT" == itemParam.Attribute("VarType").Value && itemParam.Attribute("MarshalAs").Value == "UnmanagedType.Struct")
                return false;

            if (("true" == itemParam.Attribute("IsComProxy").Value) || ("COMObject" == itemParam.Attribute("Type").Value) || ("object" == itemParam.Attribute("Type").Value))
                return true;
            else
                return false;
        }

        private static string GetMethodImplementCode(Settings settings, XElement methodNode)
        {
            bool hasRefParams = ParameterApi.HasRefOrOutParamsParams(methodNode.Element("Parameters"), true);
            bool hasParams = ParameterApi.HasParams(methodNode);
            var tabSpace = CSharpGenerator.TabSpace(3);
            var methodName = methodNode.Attribute("Name").Value;

            string result = "";
            ////result += "\t\t\tDelegate[] recipients = _eventBinding.GetEventRecipients(\"" + methodNode.Attribute("Name").Value + "\");\r\n";
            ////result += "\t\t\t" + "if( (true == _eventClass.IsCurrentlyDisposing) || (recipients.Length == 0) )\r\n";
            ////result += "\t\t\t{\r\n";
            ////result += "\t\t\t\tInvoker.ReleaseParamsArray(" + CreateParametersCallString(settings, methodNode.Element("Parameters"), "") + ");\r\n";
            ////result += "\t\t\t\treturn;\r\n";
            ////result += "\t\t\t}\r\n\r\n";
            result += tabSpace + "if (!Validate(\"" + methodName + "\"))\r\n";
            result += tabSpace + "{\r\n";
            if (hasParams)
            {
                result += tabSpace + "    Invoker.ReleaseParamsArray(" + CreateParametersCallString(settings, methodNode.Element("Parameters"), "") + ");\r\n";
            }

            result += tabSpace + "    return;\r\n";
            result += tabSpace + "}\r\n\r\n";

            result += CreateConversionString(settings, 3, methodNode.Element("Parameters"));
            result += CreateSetArrayString(settings, 3, methodNode.Element("Parameters"));

            result += tabSpace + "EventBinding.RaiseCustomEvent(\"" +  methodNode.Attribute("Name").Value + "\", ref paramsArray);" +"\r\n";

            string modRefs = ParameterApi.CreateParametersToRefUpdateString(settings, 3, methodNode.Element("Parameters"), true);
            if (modRefs != "")
                modRefs = "\r\n" + modRefs;

            result += modRefs;

            return result;
        }

        private static string CreateParametersCallString(Settings settings, XElement parametersNode, string prefix)
        {
            string result = "";
            int paramsCount = ParameterApi.GetParamsCount(parametersNode, true);
            int i = 1;
            foreach (XElement itemParam in ParameterApi.GetParameter(parametersNode, true))
            {               
                result += prefix + ParameterApi.ValidateParamName(itemParam.Attribute("Name").Value);
                if(i<paramsCount)
                    result += ", ";
                i++;
            }
            return result;
        }
        
        private static string CreateModifiersString(int tabCount, XElement parametersNode)
        {
            string tabSpace = CSharpGenerator.TabSpace(tabCount);
            string result = tabSpace + "bool[] modifiers = new bool[]{%values%};\r\n";
            string arrays = "";
            foreach (XElement itemParam in ParameterApi.GetParameter(parametersNode, true))
            {
                if("true" == itemParam.Attribute("IsRef").Value)
                    arrays += "true, ";
                else
                   arrays += "false, ";
            }
            if (", " == arrays.Substring(arrays.Length - 2))
                arrays = arrays.Substring(0, arrays.Length - 2);

            result = result.Replace("%values%", arrays);
            return result;
        }

        private static string CreateSetArrayString(Settings settings, int tabCount, XElement parametersNode)
        {
            string tabSpace = CSharpGenerator.TabSpace(tabCount);
            int paramsCount = ParameterApi.GetParamsCount(parametersNode, true);

            string result = tabSpace + "object[] paramsArray = new object[" + paramsCount.ToString() + "];\r\n";
            
            int i = 0;
            foreach (XElement itemParam in ParameterApi.GetParameter(parametersNode, true))
            {
                if ("true" == itemParam.Attribute("IsRef").Value)
                {
                    result += tabSpace + "paramsArray.SetValue(" + ParameterApi.ValidateParamName(itemParam.Attribute("Name").Value) + ", " + i.ToString() + ");\r\n";
                }
                else
                {
                    result += tabSpace + "paramsArray[" + i.ToString() + "] = " + "new" + itemParam.Attribute("Name").Value + ";\r\n";
                }
                i++;
            }
            return result;
        }

        private static string CreateConversionString(Settings settings, int tabCount, XElement parametersNode)
        {
            string tabSpace = CSharpGenerator.TabSpace(tabCount); 
            string result = "";
            foreach (XElement itemParam in ParameterApi.GetParameter(parametersNode, true))
            {
                if ("true" == itemParam.Attribute("IsRef").Value)
                    continue;

                if (IsComProxy(itemParam)) // if ("true" == itemParam.Attribute("IsComProxy").Value)                
                {
                    if (IsEarlyBindInterface(itemParam))
                    {
                        string qualifiedType = CSharpGenerator.GetQualifiedType(itemParam);
                        result += tabSpace + qualifiedType + " new" + itemParam.Attribute("Name").Value + " = " +  ParameterApi.ValidateParamName(itemParam.Attribute("Name").Value) + " as " + qualifiedType + ";\r\n";
                    }
                    else
                    {
                        string qualifiedType = CSharpGenerator.GetQualifiedType(itemParam);
                        string parameterName = ParameterApi.ValidateParamName(itemParam.Attribute("Name").Value);
                        string factory = "";
                        if (qualifiedType == "object")
                        {
                            factory = "Factory.CreateEventArgumentObjectFromComProxy(EventClass, " + parameterName + ") as object";
                        }
                        else
                        {
                            factory = "Factory.CreateKnownObjectFromComProxy<" + qualifiedType + ">(EventClass, " + parameterName + ", " + qualifiedType + ".LateBindingApiWrapperType)";
                        }

                        result += tabSpace + qualifiedType + " new" + itemParam.Attribute("Name").Value +" = " + factory + ";\r\n";
                    }
                }
                else
                {
                    if ("true" == itemParam.Attribute("IsEnum").Value)
                    {
                        string qualifiedType = CSharpGenerator.GetQualifiedType(itemParam);
                        result += tabSpace + qualifiedType + " new" + itemParam.Attribute("Name").Value +
                           " = (" + qualifiedType + ")" + ParameterApi.ValidateParamName(itemParam.Attribute("Name").Value) + ";\r\n";
                    }
                    else
                    {
                        if (itemParam.Attribute("Type").Value.Equals("object", StringComparison.InvariantCultureIgnoreCase))
                        {
                            result += tabSpace + itemParam.Attribute("Type").Value + " new" + itemParam.Attribute("Name").Value +
                            " = (" + CSharpGenerator.ConvertTypeToConvertCall(itemParam.Attribute("Type").Value) + ")" + ParameterApi.ValidateParamName(itemParam.Attribute("Name").Value) + ";\r\n";
                        }
                        else
                        {
                            result += tabSpace + itemParam.Attribute("Type").Value + " new" + itemParam.Attribute("Name").Value +
                            " = To" + CSharpGenerator.ConvertTypeToConvertCall(itemParam.Attribute("Type").Value) + "(" + ParameterApi.ValidateParamName(itemParam.Attribute("Name").Value) + ");\r\n";
                        }
                    }
                }
            } 
            return result;
        }
    }
}