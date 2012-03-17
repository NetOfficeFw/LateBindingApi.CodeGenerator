using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.VB
{
    public static class DocumentationApi
    {

        public static string[] AddParameterDocumentation(string[] supportDocuArray, XElement other)
        {
            List<string> list = new List<string>();
            string[] otherSupport = VBGenerator.GetSupportByVersionArray(other);
            foreach (string item in supportDocuArray)
                list.Add(item);

            foreach (string item in otherSupport)
            {
                bool found = false;
                foreach (string otherItem in list)
                {
                    if (item == otherItem)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    list.Add(item);
            }

            return list.ToArray();
        }

        /// <summary>
        /// SupportByVersionArray 
        /// </summary>
        /// <param name="numberOfTabSpace"></param>
        /// <param name="parametersNode"></param>
        /// <returns></returns>
        internal static string CreateParameterDocumentationForMethod(int numberOfTabSpace, string[] SupportByVersion, XElement parametersNode)
        {
            XElement parentNode = parametersNode;
            while (parentNode.Name != "Project")
                parentNode = parentNode.Parent;

            string result = "";
            string tabSpace = VBGenerator.TabSpace(numberOfTabSpace);

            string libs = "''' SupportByVersion " + parentNode.Attribute("Name").Value + " ";
            foreach (string lib in SupportByVersion)
            {
                libs += lib + ", ";
            }
            libs = libs.Substring(0, libs.Length - 2);

            string summary = tabSpace + "''' <summary>\r\n" + tabSpace + libs + "\r\n";
            summary += tabSpace + "''' </summary>\r\n";

            result += summary;

            foreach (XElement itemParameter in parametersNode.Elements("Parameter"))
            {
                string typeName = VBGenerator.GetQualifiedType(itemParameter);
                typeName = ValidateParamName(typeName);

                if ("true" == itemParameter.Attribute("IsOptional").Value)
                    typeName = "optional " + typeName;

                if ("true" == itemParameter.Attribute("IsRef").Value)
                    typeName = "ByRef " + typeName;
                
                if ("true" == itemParameter.Attribute("IsArray").Value)
                    typeName += "()";

                typeName = itemParameter.Attribute("Name").Value + " As " + typeName;
                string defaultInfo = "";
                
                if (itemParameter.Attribute("HasDefaultValue").Value == "true")
                {
                    defaultInfo = " = " + itemParameter.Attribute("DefaultValue").Value;
                }

                string parName = ValidateParamName(itemParameter.Attribute("Name").Value);
                if (parName.Equals(parametersNode.Parent.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase))
                    parName += "_";
                 

                string line = tabSpace + "''' <param name=\"" + parName + "\">" + typeName + defaultInfo + "</param>\r\n";
                result += line;
            }
            if (parametersNode.Element("ReturnValue").Attribute("Type").Value == "COMObject")
                summary += tabSpace + "''' <returns>COMObject</returns>\r\n";
            return result;
        }

        private static string ValidateParamName(string name)
        {
            return name.Substring(0, 1).ToLower() + name.Substring(1);
        }

        /// <summary>
        /// SupportByVersionArray 
        /// </summary>
        /// <param name="numberOfTabSpace"></param>
        /// <param name="parametersNode"></param>
        /// <returns></returns>
        internal static string CreateParameterDocumentation(int numberOfTabSpace, XElement parametersNode)
        {
            return CreateParameterDocumentation(numberOfTabSpace, parametersNode, true, "");
        }

        /// <summary>
        /// SupportByVersionArray 
        /// </summary>
        /// <param name="numberOfTabSpace"></param>
        /// <param name="parametersNode"></param>
        /// <returns></returns>
        internal static string CreateParameterDocumentation(int numberOfTabSpace, XElement parametersNode, bool generateGetSet, string additional)
        {
            XElement parentNode = parametersNode;
            while (parentNode.Name != "Project")
                parentNode = parentNode.Parent;

            string result = "";
            string tabSpace = VBGenerator.TabSpace(numberOfTabSpace);

            string[] SupportByVersion = VBGenerator.GetSupportByVersionArray(parametersNode);
            string libs = "''' SupportByVersion " + parentNode.Attribute("Name").Value + " ";
            foreach (string lib in SupportByVersion)
            {
                libs += lib + ", ";
            }
            libs = libs.Substring(0, libs.Length - 2);

            string summary = tabSpace + "''' <summary>\r\n" + tabSpace + libs + "\r\n";
            if ("Property" == parametersNode.Parent.Name)
            {
                if (generateGetSet)
                {
                    if ("INVOKE_PROPERTYGET" == parametersNode.Parent.Attribute("InvokeKind").Value)
                        summary += tabSpace + "''' Get\r\n";
                    else
                        summary += tabSpace + "''' Get/Set\r\n";
                }
                summary += additional;
                summary += tabSpace + "''' </summary>\r\n";

                if (parametersNode.Element("ReturnValue").Attribute("Type").Value == "COMObject")
                    summary += tabSpace + "''' <returns>COMObject</returns>\r\n";
            }
            else
            {
                summary += additional;

                summary += tabSpace + "''' </summary>\r\n";
                if (parametersNode.Element("ReturnValue").Attribute("Type").Value == "COMObject")
                    summary += tabSpace + "''' <returns>COMObject</returns>\r\n";
            }
            result += summary;

            foreach (XElement itemParameter in parametersNode.Elements("Parameter"))
            {
                string typeName = VBGenerator.GetQualifiedType(itemParameter);

                if ("true" == itemParameter.Attribute("IsOptional").Value)
                    typeName = "optional " + typeName;

                if ("true" == itemParameter.Attribute("IsRef").Value)
                    typeName = "ref " + typeName;

                if ("true" == itemParameter.Attribute("IsArray").Value)
                    typeName += "[]";

                string parName = ValidateParamName(itemParameter.Attribute("Name").Value);
                if (parametersNode.Parent.Attribute("Name").Value.Equals(itemParameter.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase))
                    parName += "_";

                typeName = typeName += " " + parName;

                string line = tabSpace + "''' <param name=\"" + parName + "\">" + typeName + "</param>\r\n";
                result += line;
            }
            return result;
        }

    }
}
