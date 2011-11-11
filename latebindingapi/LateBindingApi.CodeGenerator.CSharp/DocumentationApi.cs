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
            string[] otherSupport = CSharpGenerator.GetSupportByLibraryArray(other);
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
        /// SupportByLibraryArray 
        /// </summary>
        /// <param name="numberOfTabSpace"></param>
        /// <param name="parametersNode"></param>
        /// <returns></returns>
        internal static string CreateParameterDocumentationForMethod(int numberOfTabSpace, string[] supportByLibrary, XElement parametersNode)
        {
            XElement parentNode = parametersNode;
            while (parentNode.Name != "Project")
                parentNode = parentNode.Parent;

            string result = "";
            string tabSpace = CSharpGenerator.TabSpace(numberOfTabSpace);

            string libs = "/// SupportByLibrary " + parentNode.Attribute("Name").Value + " ";
            foreach (string lib in supportByLibrary)
            {
                libs += lib + ", ";
            }
            libs = libs.Substring(0, libs.Length - 2);

            string summary = tabSpace + "/// <summary>\r\n" + tabSpace + libs + "\r\n";
            summary += tabSpace + "/// </summary>\r\n";

            result += summary;

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

                string line = tabSpace + "/// <param name=\"" + itemParameter.Attribute("Name").Value + "\">" + typeName + "</param>\r\n";
                result += line;
            }
            return result;
        }

        /// <summary>
        /// SupportByLibraryArray 
        /// </summary>
        /// <param name="numberOfTabSpace"></param>
        /// <param name="parametersNode"></param>
        /// <returns></returns>
        internal static string CreateParameterDocumentation(int numberOfTabSpace, XElement parametersNode)
        {
            XElement parentNode = parametersNode;
            while (parentNode.Name != "Project")
                parentNode = parentNode.Parent;

            string result = "";
            string tabSpace = CSharpGenerator.TabSpace(numberOfTabSpace);

            string[] supportByLibrary = CSharpGenerator.GetSupportByLibraryArray(parametersNode);
            string libs = "/// SupportByLibrary " + parentNode.Attribute("Name").Value + " ";
            foreach (string lib in supportByLibrary)
            {
                libs += lib + ", ";
            }
            libs = libs.Substring(0, libs.Length - 2);

            string summary = tabSpace + "/// <summary>\r\n" + tabSpace + libs + "\r\n";
            if ("Property" == parametersNode.Parent.Name)
            {
                if ("INVOKE_PROPERTYGET" == parametersNode.Parent.Attribute("InvokeKind").Value)
                    summary += tabSpace + "/// Get\r\n";
                else
                    summary += tabSpace + "/// Get/Set\r\n";

                summary += tabSpace + "/// </summary>\r\n";
            }
            else
                summary += tabSpace + "/// </summary>\r\n";

            result += summary;
            
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

                string line = tabSpace + "/// <param name=\"" + itemParameter.Attribute("Name").Value + "\">" + typeName + "</param>\r\n";
                result += line;
            }
            return result;
        }
    }
}
