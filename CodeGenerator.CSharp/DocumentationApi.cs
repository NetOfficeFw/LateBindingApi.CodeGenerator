using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    public static class DocumentationApi
    {

        public static string[] AddParameterDocumentation(string[] supportDocuArray, XElement other)
        {
            List<string> list = new List<string>();
            string[] otherSupport = CSharpGenerator.GetSupportByVersionArray(other);
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
        internal static string CreateParameterDocumentationForMethod(int numberOfTabSpace, string[] SupportByVersion, XElement parametersNode, string remarks)
        {
            XElement parentNode = parametersNode;
            while (parentNode.Name != "Project")
                parentNode = parentNode.Parent;

            string result = "";
            string tabSpace = CSharpGenerator.TabSpace(numberOfTabSpace);

            string libs = "/// SupportByVersion " + parentNode.Attribute("Name").Value + " ";
            foreach (string lib in SupportByVersion)
            {
                libs += lib + ", ";
            }
            libs = libs.Substring(0, libs.Length - 2);

            string summary = tabSpace + "/// <summary>\r\n" + tabSpace + libs + "\r\n";
            summary += tabSpace + "/// </summary>\r\n";
            if (!String.IsNullOrEmpty(remarks))
            {
                summary += tabSpace + "/// <remarks> " + remarks + " </remarks>\r\n";
            }

            result += summary;

            foreach (XElement itemParameter in parametersNode.Elements("Parameter"))
            {
                string typeName = CSharpGenerator.GetQualifiedType(itemParameter);
                string parameterName = ParameterApi.ValidateParamName(itemParameter.Attribute("Name").Value);

                if ("true" == itemParameter.Attribute("IsOptional").Value)
                    typeName = "optional " + typeName;

                if ("true" == itemParameter.Attribute("IsRef").Value)
                    typeName = "ref " + typeName;

                if ("true" == itemParameter.Attribute("IsArray").Value)
                    typeName += "[]";

                typeName += " " + parameterName;
                string defaultInfo = "";

                if (itemParameter.Attribute("HasDefaultValue").Value == "true")
                {
                    defaultInfo = " = " + itemParameter.Attribute("DefaultValue").Value;
                }
                string line = tabSpace + "/// <param name=\"" + parameterName + "\">" + typeName + defaultInfo + "</param>\r\n";
                result += line;
            }
            return result;
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
        internal static string CreateParameterDocumentation(int numberOfTabSpace, XElement parametersNode, bool generateGetSet, string additional="", string remarks = null)
        {
            List<string> listVersions = new List<string>();

            XElement parentNode = parametersNode;
            while (parentNode.Name != "Project")
                parentNode = parentNode.Parent;

            string result = "";
            string tabSpace = CSharpGenerator.TabSpace(numberOfTabSpace);
            string retValueType = CSharpGenerator.GetQualifiedType(parametersNode.Element("ReturnValue"));

            string[] SupportByVersion = CSharpGenerator.GetSupportByVersionArray(parametersNode);
            string libs = "/// SupportByVersion " + parentNode.Attribute("Name").Value + " ";
            foreach (string lib in SupportByVersion)
                listVersions.Add(lib);

            listVersions.Sort(CSharpGenerator.CompareSupportByVersion);

            foreach (string versionAttribute in listVersions)
                libs += versionAttribute + ", ";

            libs = libs.Substring(0, libs.Length - 2);

            string summary = tabSpace + "/// <summary>\r\n" + tabSpace + libs + "\r\n";
            if ("Property" == parametersNode.Parent.Name)
            {
                if (generateGetSet)
                {
                    if ("INVOKE_PROPERTYGET" == parametersNode.Parent.Attribute("InvokeKind").Value)
                        summary += tabSpace + "/// Get\r\n";
                    else
                        summary += tabSpace + "/// Get/Set\r\n";
                }
                summary += additional;
                if(retValueType == "COMObject")
                    summary += tabSpace + "/// Unknown COM Proxy\r\n";
                summary += tabSpace + "/// </summary>\r\n";
            }
            else
            {
                summary += additional;
                if (retValueType == "COMObject")
                    summary += tabSpace + "/// Unknown COM Proxy\r\n";
                summary += tabSpace + "/// </summary>\r\n";
            }
            result += summary;
            if (!String.IsNullOrEmpty(remarks))
            {
                result += tabSpace + "/// <remarks>" + remarks + "</remarks>\r\n";
            }

            foreach (XElement itemParameter in parametersNode.Elements("Parameter"))
            {
                string typeName = CSharpGenerator.GetQualifiedType(itemParameter);

                if ("true" == itemParameter.Attribute("IsOptional").Value)
                    typeName = "optional " + typeName;

                if ("true" == itemParameter.Attribute("IsRef").Value)
                    typeName = "ref " + typeName;

                if ("true" == itemParameter.Attribute("IsArray").Value)
                    typeName += "[]";

                typeName += " " + itemParameter.Attribute("Name").Value;

                string line = tabSpace + "/// <param name=\"" + ParameterApi.ValidateParamName(itemParameter.Attribute("Name").Value) + "\">" + typeName + "</param>\r\n";
                result += line;
            }
            return result;
        }

    }
}
