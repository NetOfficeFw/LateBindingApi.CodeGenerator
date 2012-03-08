using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.VB
{
    internal static class EventApi
    {
        private static string _interfaceFile;

        internal static string ConvertInterfacesToFiles(XElement projectNode, XElement dispatchfacesNode, XElement facesNode, Settings settings, string solutionFolder)
        {
            if (null == _interfaceFile)
                _interfaceFile = RessourceApi.ReadString("Event.Interface.txt");

            string faceFolder = System.IO.Path.Combine(solutionFolder, projectNode.Attribute("Name").Value);
            faceFolder = System.IO.Path.Combine(faceFolder, "EventInterfaces");
            if (false == System.IO.Directory.Exists(faceFolder))
                System.IO.Directory.CreateDirectory(faceFolder);

            string result = "";
            foreach (XElement faceNode in dispatchfacesNode.Elements("Interface"))
            {
                if ("true" == faceNode.Attribute("IsEventInterface").Value)
                    result += ConvertInterfaceToFile(settings, projectNode, faceNode, faceFolder) + "\r\n";
            }

            foreach (XElement faceNode in facesNode.Elements("Interface"))
            {
                if ("true" == faceNode.Attribute("IsEventInterface").Value)
                    result += ConvertInterfaceToFile(settings, projectNode, faceNode, faceFolder) + "\r\n";
            }

            return result;
        }

        private static string ConvertInterfaceToFile(Settings settings, XElement projectNode, XElement faceNode, string faceFolder)
        {
            string fileName = System.IO.Path.Combine(faceFolder, faceNode.Attribute("Name").Value + ".vb");

            string newEnum = ConvertInterfaceToString(settings, projectNode, faceNode);
            System.IO.File.AppendAllText(fileName, newEnum);

            int i = faceFolder.LastIndexOf("\\");
            string result = "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + faceNode.Attribute("Name").Value + ".vb" + "\" />";
            return result;
        }

        private static string ConvertInterfaceToString(Settings settings, XElement projectNode, XElement faceNode)
        {            
            string result = _interfaceFile.Replace("%namespace%", projectNode.Attribute("Namespace").Value);
            result = result.Replace("%supportby%", VBGenerator.GetSupportByLibraryAttribute(faceNode));
            result = result.Replace("%name%", faceNode.Attribute("Name").Value);
            result = result.Replace("%guid%", XmlConvert.DecodeName(faceNode.Element("DispIds").Element("DispId").Attribute("Id").Value));
            
            string methodResult = "";
            string implementResult = "";
            foreach (XElement itemMethod in faceNode.Element("Methods").Elements("Method"))
            {
                methodResult += "\t\t" + VBGenerator.GetSupportByLibraryAttribute(itemMethod) + "\r\n"; 
                methodResult += "\t\t<PreserveSig, MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime), DispId(" + itemMethod.Element("DispIds").Element("DispId").Attribute("Id").Value + ")> _\r\n";
                methodResult += "\t\t" + GetEventMethodSignatur(settings, itemMethod, false, false) + "\r\n\r\n";
                implementResult += "\t\t" + GetEventMethodSignatur(settings, itemMethod, true, true) + "\r\n" + "\r\n" + GetMethodImplementCode(settings, itemMethod) + "\t\tEnd Sub\r\n\r\n";
            }
            
            if("" !=methodResult)
                methodResult = methodResult.Substring(0, methodResult.Length - "\r\n".Length);
    
            result = result.Replace("%methods%", methodResult);
            result = result.Replace("%methodsImplement%", implementResult);
            return result;
        }

        internal static string GetEventMethodSignatur(Settings settings, XElement methodNode, bool withPublic, bool withImplementPostfix)
        {
            string publicModifier ="";
            if (true == withPublic)
                publicModifier = "Public ";
            
            string result = publicModifier + "Sub " + ParameterApi.ValidateName(methodNode.Attribute("Name").Value) + "("; 
            foreach (XElement itemParam in methodNode.Element("Parameters").Elements("Parameter"))
            {
                string isRef = "";
                if ("true" == itemParam.Attribute("IsRef").Value)
                    isRef = "ByRef ";
                else
                    isRef = "ByVal ";

                string par = "";
                if ("true" == itemParam.Attribute("IsRef").Value)
                    par = "<[In](), [Out]()";
                else
                    par = "<[In]()";
               
                if (("true" == itemParam.Attribute("IsComProxy").Value) || ("COMObject" == itemParam.Attribute("Type").Value) || ("object" == itemParam.Attribute("Type").Value))
                {

                    par += ",MarshalAs(UnmanagedType.IDispatch)> " + ParameterApi.ValidateParamName(settings, itemParam.Attribute("Name").Value) + " As Object";
                }
                else
                {
                    par += ">" +isRef + ParameterApi.ValidateParamName(settings, itemParam.Attribute("Name").Value) + " As Object";
                }
            
                result += par +", ";
            }
            if (", " == result.Substring(result.Length - 2))
                result = result.Substring(0, result.Length - 2);

            result += ")";
            if (withImplementPostfix)
                result += " Implements " + methodNode.Parent.Parent.Attribute("Name").Value + "." + methodNode.Attribute("Name").Value;

            return result;
        }

        private static string GetMethodImplementCode(Settings settings, XElement methodNode)
        {
            bool hasRefParams = ParameterApi.HasRefOrOutParamsParams(methodNode.Element("Parameters"), true);

            string result = "\t\t\tIf (_eventBinding.GetCountOfEventRecipients(\"" + methodNode.Attribute("Name").Value + "\") = 0 Or True = _eventClass.IsCurrentlyDisposing) Then\r\n";
            result += "\t\t\t\tInvoker.ReleaseParamsArray(" + CreateParametersCallString(settings, methodNode.Element("Parameters"), "") + ")\r\n";
            result += "\t\t\t\tReturn\r\n";
            result += "\t\t\tEnd If\r\n";

            result += CreateConversionString(settings, 3, methodNode.Element("Parameters"));
            result += CreateSetArrayString(settings, 3, methodNode.Element("Parameters"));

            result += "\t\t\t_eventBinding.RaiseCustomEvent(\"" + methodNode.Attribute("Name").Value + "\", paramsArray)\r\n";

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
                result += prefix + ParameterApi.ValidateParamName(settings, itemParam.Attribute("Name").Value);
                if(i<paramsCount)
                    result += ", ";                
                i++;
            }
            return result;
        }
        
        private static string CreateModifiersString(int tabCount, XElement parametersNode)
        {
            string tabSpace = VBGenerator.TabSpace(tabCount);
            string result = tabSpace + "Dim modifiers() As Boolean = new Boolean(%values%);\r\n";
            string arrays = "";
            foreach (XElement itemParam in ParameterApi.GetParameter(parametersNode, true))
            {
                if("true" == itemParam.Attribute("IsRef").Value)
                    arrays += "True, ";
                else
                   arrays += "False, ";
            }
            if (", " == arrays.Substring(arrays.Length - 2))
                arrays = arrays.Substring(0, arrays.Length - 2);

            result = result.Replace("%values%", arrays);
            return result;
        }

        private static string CreateSetArrayString(Settings settings, int tabCount, XElement parametersNode)
        {
            string tabSpace = VBGenerator.TabSpace(tabCount);
            int paramsCount = ParameterApi.GetParamsCount(parametersNode, true);
            paramsCount -= 1;

            string result = tabSpace + "Dim paramsArray("+ paramsCount.ToString() + ") As Object\r\n";
            
            int i = 0;
            foreach (XElement itemParam in ParameterApi.GetParameter(parametersNode, true))
            {
                if ("true" == itemParam.Attribute("IsRef").Value)
                {
                    result += tabSpace + "paramsArray.SetValue(" + ParameterApi.ValidateParamName(settings, itemParam.Attribute("Name").Value) + ", " + i.ToString() + ")\r\n";
                }
                else
                {
                    result += tabSpace + "paramsArray(" + i.ToString() + ") = " + "new" + itemParam.Attribute("Name").Value + "\r\n";
                }
                i++;
            }
            return result;
        }

        private static string CreateConversionString(Settings settings, int tabCount, XElement parametersNode)
        {
            string tabSpace = VBGenerator.TabSpace(tabCount); 
            string result = "";
            foreach (XElement itemParam in ParameterApi.GetParameter(parametersNode, true))
            {
                if ("true" == itemParam.Attribute("IsRef").Value)
                    continue;

                if ("true" == itemParam.Attribute("IsComProxy").Value)
                {
                    string qualifiedType = VBGenerator.GetQualifiedType(itemParam);
                    if (qualifiedType.Equals("object", StringComparison.InvariantCultureIgnoreCase))
                        qualifiedType = "COMObject";
                    result += tabSpace + "Dim new" + itemParam.Attribute("Name").Value + " As " + qualifiedType +
                            " = LateBindingApi.Core.Factory.CreateObjectFromComProxy(_eventClass, " + ParameterApi.ValidateParamName(settings,itemParam.Attribute("Name").Value) + ")" + "\r\n";
                }
                else
                {
                    if ("true" == itemParam.Attribute("IsEnum").Value)
                    {
                        string qualifiedType = VBGenerator.GetQualifiedType(itemParam);
                        result += tabSpace + "Dim new" + itemParam.Attribute("Name").Value + " As " + qualifiedType +
                           " = " + ParameterApi.ValidateParamName(settings, itemParam.Attribute("Name").Value) + "\r\n";

                    }
                    else
                    {
                            result += tabSpace + "Dim new" + itemParam.Attribute("Name").Value + " As " +  ParameterApi.ValidateVarTypeVB(itemParam.Attribute("Type").Value) +
                            " = " + ParameterApi.ValidateParamName(settings, itemParam.Attribute("Name").Value) + "\r\n";
                    }
                }
            } 
            return result;
        }
    }
}