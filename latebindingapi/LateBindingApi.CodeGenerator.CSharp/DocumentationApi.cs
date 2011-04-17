using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class DocumentationApi
    {
        internal static string CreateParameterDocumentation(int numberOfTabSpace, XElement parametersNode)
        {
            string tabSpace = CSharpGenerator.TabSpace(numberOfTabSpace);
            string result = "";
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
