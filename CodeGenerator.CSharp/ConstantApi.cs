using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class ConstantApi
    {
        private static string _fileHeader = ""
                                            + "using System;\r\n"
                                            + "using NetOffice;\r\n"
                                            + "using NetOffice.Attributes;\r\n"
                                            + "namespace %namespace%\r\n"
                                            + "{\r\n";

        internal static string ConvertConstantsToFiles(XElement projectNode, XElement enumsNode, Settings settings, string solutionFolder)
        {
            string constFolder = System.IO.Path.Combine(solutionFolder, projectNode.Attribute("Name").Value);
            constFolder = System.IO.Path.Combine(constFolder, "Constants");
            if (false == System.IO.Directory.Exists(constFolder))
                System.IO.Directory.CreateDirectory(constFolder);

            string result = "";
            foreach (XElement constNode in enumsNode.Elements("Constant"))
                result += ConvertConstantToFile(settings, projectNode, constNode, constFolder) + "\r\n";

            return result;
        }

        private static string ConvertConstantToFile(Settings settings, XElement projectNode, XElement enumNode, string enumFolder)
        {
            string fileName = System.IO.Path.Combine(enumFolder, enumNode.Attribute("Name").Value + ".cs");

            string newEnum = ConvertConstantToString(settings, projectNode, enumNode);
            System.IO.File.AppendAllText(fileName, newEnum);

            int i = enumFolder.LastIndexOf("\\");
            string result = "    <Compile Include=\"" + enumFolder.Substring(i + 1) + "\\" + enumNode.Attribute("Name").Value + ".cs" + "\" />";
            return result;
        }

        private static string ConvertConstantToString(Settings settings, XElement projectNode, XElement enumNode)
        {
            string result = _fileHeader.Replace("%namespace%", projectNode.Attribute("Namespace").Value + ".Constants");
            string enumAttributes = CSharpGenerator.GetSupportByVersionAttribute(enumNode);
            
            string name = enumNode.Attribute("Name").Value;
            
            if(true == settings.CreateXmlDocumentation)
                result += CSharpGenerator.GetSupportByVersionSummary("\t", enumNode);
           
            result += "\t" + enumAttributes + Environment.NewLine;
            result += "\t[EntityType(EntityType.IsConstants)]\r\n" + "\tpublic static class " + name + Environment.NewLine + "\t{" + Environment.NewLine;

            int countOfMembers = enumNode.Element("Members").Elements("Member").Count();
            int i = 1;
            foreach (var itemMember in enumNode.Element("Members").Elements("Member"))
            {
                string memberAttribute = CSharpGenerator.GetSupportByVersionAttribute(itemMember);
               

                string memberType= itemMember.Attribute("Type").Value;
                string memberName = itemMember.Attribute("Name").Value;
                string memberValue = itemMember.Attribute("Value").Value;

                if (memberType == "string")
                {
                    memberValue = memberValue.Replace("\"", "");
                    memberValue = "\"" + memberValue + "\"";
                }

                if (true == settings.CreateXmlDocumentation)
                    result += CSharpGenerator.GetSupportByVersionSummary("\t\t", itemMember);

                result += "\t\t" + memberAttribute + "\r\n";
                result += "\t\t" + "public const " + memberType + " " + memberName + " = " + memberValue + ";";
                 
                if (i < countOfMembers)
                    result += "\r\n\r\n";
                else
                    result += "\r\n";

                i++;
            }

            result += "\t}" + Environment.NewLine;
            result += "}";
            return result;
        }

       }
}
