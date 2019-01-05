using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class ModuleApi
    {
        public static readonly string FolderName = "Modules";

        private static string _fileHeader = "using System;\r\n"
                                          + "using NetRuntimeSystem = System;\r\n"
                                          + "using System.ComponentModel;\r\n"
                                          + "using NetOffice.Attributes;\r\n"
                                          + "\r\n"
                                          + "namespace %namespace%.GlobalHelperModules\r\n"
                                          + "{\r\n";

        private static string _classDesc = "    ///<summary>\r\n"
                                         + "    /// Module %name%\r\n"
                                         + "    /// %supportedByVersion%\r\n"
                                         + "    ///</summary>\r\n";

        private static string _classHeader =
            "\t[EntityType(EntityType.IsModule), ModuleBaseType(typeof(%projectName%Api.Application))]\r\n" +
            "\tpublic class %name%\r\n\t{\r\n%instanceType%\r\n";

        private static string _instanceType;

        internal static string ConvertModulesToFiles(XElement projectNode, XElement facesNode, Settings settings, string solutionFolder)
        {
            string projectName = projectNode.Attribute("Name").Value;
            string modulesFolder = Path.Combine(solutionFolder, projectName, FolderName);
            DirectoryEx.EnsureDirectory(modulesFolder);

            string result = "";
            foreach (XElement moduleNode in facesNode.Elements("Module"))
                result += ConvertModuleToFile(settings, projectNode, moduleNode, modulesFolder) + "\r\n";

            foreach (XElement item in projectNode.Element("CoClasses").Elements("CoClass"))
            {
                if (item.Attribute("IsAppObject").Value == "true")
                {
                    XElement face = CSharpGenerator.GetInterfaceOrClassFromKey((item.Element("Inherited").FirstNode as XElement).Attribute("Key").Value);
                    result += ConvertGlobalModuleToFile(settings, projectNode, face, modulesFolder) + "\r\n";
                    break;
                }
            }

            return result;
        }

        private static string ConvertGlobalModuleToFile(Settings settings, XElement projectNode, XElement faceNode, string faceFolder)
        {
            string fileName = Path.Combine(faceFolder, "Global.cs");

            string newEnum = ConvertGlobalModuleToString(settings, projectNode, faceNode);
            File.WriteAllText(fileName, newEnum, Constants.UTF8WithBOM);

            int i = faceFolder.LastIndexOf("\\");
            string result = "    <Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "Global.cs" + "\" />";
            return result;
        }

        private static string ConvertGlobalModuleToString(Settings settings, XElement projectNode, XElement moduleNode)
        {
            if (null == _instanceType)
                _instanceType = RessourceApi.ReadString("Module.Module.txt");

            string namespaceName = projectNode.Attribute("Namespace").Value;
            string projectName = projectNode.Attribute("Name").Value;

            string result = _fileHeader.Replace("%namespace%", namespaceName).Replace("%projectName%", projectName);
            string attributes = "    " + CSharpGenerator.GetSupportByVersionAttribute(moduleNode);
            string header = _classHeader.Replace("%name%", "GlobalModule").Replace("%projectName%", projectName);
            string classDesc = _classDesc.Replace("%name%", "GlobalModule").Replace("%supportedByVersion%", CSharpGenerator.GetSupportByVersion(moduleNode));

            string properties = PropertyApi.ConvertPropertiesLateBindToString(settings, moduleNode.Element("Properties"));
            string methods = MethodApi.ConvertMethodsLateBindToString(settings, moduleNode.Element("Methods"));

            result += classDesc;
            result += attributes + "\r\n";
            result += header;
            result += properties;
            result += methods;
            result += "\t}\r\n}";

            result = result.Replace("%instanceType%", _instanceType);
            result = result.Replace("public", "public static");
            result = result.Replace("(this", "(_instance");

            return result;
        }


        private static string ConvertModuleToFile(Settings settings, XElement projectNode, XElement faceNode, string modulesFolder)
        {
            string fileName = System.IO.Path.Combine(modulesFolder, faceNode.Attribute("Name").Value + ".cs");

            string newEnum = ConvertModuleToString(settings, projectNode, faceNode);
            System.IO.File.AppendAllText(fileName, newEnum);

            int i = modulesFolder.LastIndexOf("\\");
            string result = "    <Compile Include=\"" + modulesFolder.Substring(i + 1) + "\\" + faceNode.Attribute("Name").Value + ".cs" + "\" />";
            return result;
        }

        private static string ConvertModuleToString(Settings settings, XElement projectNode, XElement moduleNode)
        {
            string result = _fileHeader.Replace("%namespace%", projectNode.Attribute("Namespace").Value);
            string attributes = "\t" + CSharpGenerator.GetSupportByVersionAttribute(moduleNode);
            string header = _classHeader.Replace("%name%", moduleNode.Attribute("Name").Value);
            string classDesc = _classDesc.Replace("%name%", moduleNode.Attribute("Name").Value);
            string methods = MethodApi.ConvertMethodsLateBindToString(settings, moduleNode.Element("Methods"));

            result += classDesc;
            result += attributes + "\r\n";
            result += header;
            result += methods;
            result += "\t}\r\n}";
            return result;
        }
    }
}
