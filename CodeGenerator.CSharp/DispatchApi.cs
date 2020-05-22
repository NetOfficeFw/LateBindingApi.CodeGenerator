using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class DispatchApi
    {
        public static readonly string FolderName = "DispatchInterfaces";

        private static string _fileHeader = "using System;\r\n"
                                          + "using NetRuntimeSystem = System;\r\n"
                                          + "using System.ComponentModel;\r\n"
                                          + "using NetOffice.Attributes;\r\n"
                                          + "%enumerableSpace%\r\n"
                                          + "namespace %namespace%\r\n"
                                          + "{%fakedClass%\r\n";

        private static string _classDesc = "\t/// <summary>\r\n\t/// DispatchInterface %name% %RefLibs%\r\n\t/// </summary>\r\n";
        private static string _classRemarks = "\t/// <remarks> MSDN Online: %docLink% </remarks>\r\n";

        private static string _classHeader = "\t[EntityType(EntityType.IsDispatchInterface), BaseType]\r\n" + "\tpublic class %name% : %inherited%%enumerable%\r\n\t{\r\n";

        private static string _classConstructor;

        private static string _fakedConstructor;

        internal static string ConvertInterfacesToFiles(XElement projectNode, XElement facesNode, Settings settings, string solutionFolder)
        {
            string projectName = projectNode.Attribute("Name").Value;
            string faceFolder = Path.Combine(solutionFolder, projectName, FolderName);
            DirectoryEx.EnsureDirectory(faceFolder);

            string result = "";
            foreach (XElement faceNode in facesNode.Elements("Interface"))
            {
                if (("false" == faceNode.Attribute("IsEventInterface").Value) && (faceNode.Attribute("Name").Value != "_Global"))
                    result += ConvertInterfaceToFile(settings, projectNode, faceNode, faceFolder) + "\r\n";
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
            if ("true" == faceNode.Attribute("IsEarlyBind").Value)
                return ConvertEarlyBindInterfaceToString(settings, projectNode, faceNode);
            else
                return ConvertLateBindInterfaceToString(settings, projectNode, faceNode);
        }

        private static bool IsNameConflicted(XElement faceNode)
        {
            foreach (XAttribute item in faceNode.Attributes())
            {
                if (item.Name == "IsNameConflict" && item.Value == "true")
                    return true;
            }
            return false;
        }

        private static bool IsOptionalConflicted(XElement faceNode)
        {
            // optional konflikte gibt es für VB nicht
            if (CSharpGenerator.Settings.VBOptimization)
                return false;

            foreach (XAttribute item in faceNode.Attributes())
            {
                if (item.Name == "IsOptionalConflict" && item.Value == "true")
                    return true;
            }
            return false;
        }

        private static string ConvertEarlyBindInterfaceToString(Settings settings, XElement projectNode, XElement faceNode)
        {
            string result = _fileHeader.Replace("%namespace%", projectNode.Attribute("Namespace").Value).Replace("%enumerableSpace%", "");
            result = result.Replace("%fakedClass%", "");
            string header = _classDesc.Replace("%name%", faceNode.Attribute("Name").Value).Replace("%RefLibs%", CSharpGenerator.GetSupportByVersionString("", faceNode));

            result += "\t#pragma warning disable\r\n\r\n";
            string version = CSharpGenerator.GetSupportByVersionAttribute(faceNode, 0);
            header += "\t" + version + "\r\n";
            string guid = XmlConvert.DecodeName(faceNode.Element("DispIds").Element("DispId").Attribute("Id").Value);
            header += "\t[ComImport, ComVisible(true), Guid(\"" + guid + "\"), TypeLibType((short) " + faceNode.Attribute("TypeLibType").Value + ")]\r\n";
            header += _classHeader.Replace("%name%", faceNode.Attribute("Name").Value);
            header =  header.Replace("class", "interface").Replace(" : %inherited%", "").Replace("%enumerable%", "");
            result += header;

            string methods = MethodApi.ConvertMethodsEarlyBindToString(settings, faceNode.Element("Methods"));
            result += methods;

            string properties = PropertyApi.ConvertPropertiesEarlyBindToString(settings, faceNode.Element("Properties"));
            result += properties;

            result += "\t}\r\n}\r\n";
            result += "\t#pragma warning restore\r\n";
            return result;
        }

        private static string CreateFakeInheritedClass(Settings settings, XElement projectNode, XElement faceNode)
        {
            if (null == _classConstructor)
                _classConstructor = RessourceApi.ReadString("Interface.Constructor.txt");

            if (null == _fakedConstructor)
                _fakedConstructor = RessourceApi.ReadString("Interface.FakedClassConstructor.txt");

            string name = faceNode.Attribute("Name").Value + "_";

            string result = "\r\n\t///<summary>\r\n\t/// " + faceNode.Attribute("Name").Value + "\r\n\t///</summary>\r\n" +
             "\tpublic class " + name + " : " + GetInherited(projectNode, faceNode) + "\r\n\t{\r\n" + "" + _fakedConstructor.Replace("%fakeName%", name) + "\t}\r\n";

             string properties = PropertyApi.ConvertConflictPropertiesLateBindToString(settings, faceNode.Element("Properties"));
             string methods = MethodApi.ConvertConflictMethodsLateBindToString(settings, faceNode.Element("Methods"));

             result = result.Replace("%InheritedFake%", properties + methods);

            return result;
        }

        private static string ConvertLateBindInterfaceToString(Settings settings, XElement projectNode, XElement faceNode)
        {
            bool isNameConflicted = IsNameConflicted(faceNode);
            bool isOptionalConflicted = IsOptionalConflicted(faceNode);

            string result = _fileHeader.Replace("%namespace%", projectNode.Attribute("Namespace").Value);
            string attributes = "\t" + CSharpGenerator.GetSupportByVersionAttribute(faceNode, 0);
            string header = _classHeader.Replace("%name%", faceNode.Attribute("Name").Value);
            header += "\t\t#pragma warning disable\r\n\r\n";

            if (isNameConflicted || isOptionalConflicted)
            {
                header = header.Replace("%inherited%", faceNode.Attribute("Name").Value + "_");
            }
            else
                header = header.Replace("%inherited%", GetInherited(projectNode, faceNode));

            if (null == _classConstructor)
                _classConstructor = RessourceApi.ReadString("Interface.Constructor.txt");
            string construct = _classConstructor.Replace("%name%", faceNode.Attribute("Name").Value);

            string docLink = "";
            if (CSharpGenerator.Settings.AddDocumentationLinks)
            {
                string projectName = projectNode.Attribute("Name").Value;
                if (null != projectName && CSharpGenerator.IsRootProjectName(projectName))
                {
                    XElement linkNode = (from a in CSharpGenerator.LinkFileDocument.Element("NOBuildTools.ReferenceAnalyzer").Element(projectName).Element("Types").Elements("Type")
                                         where a.Element("Name").Value.Equals(faceNode.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase)
                                         select a).FirstOrDefault();
                    if (null != linkNode)
                        docLink = linkNode.Element("Link")?.Value;
                }
            }

            string classDesc = _classDesc.Replace("%name%", faceNode.Attribute("Name").Value).Replace("%RefLibs%", "\r\n\t/// " + CSharpGenerator.GetSupportByVersion(faceNode));
            if (!String.IsNullOrEmpty(docLink))
            {
                string classRemarks = _classRemarks.Replace("%docLink%", docLink);
                classDesc += classRemarks;
            }

            settings.SupportByVersionSpacing = 0;
            string properties = PropertyApi.ConvertPropertiesLateBindToString(settings, faceNode.Element("Properties"));
            string methods = MethodApi.ConvertMethodsLateBindToString(settings, faceNode.Element("Methods"));
            settings.SupportByVersionSpacing = 1;

            result += classDesc;
            result += attributes + "\r\n";
            result += header;
            result += construct;
            result += properties;
            result += methods;

            if (isNameConflicted || isOptionalConflicted)
                result = result.Replace("%fakedClass%", CreateFakeInheritedClass(settings, projectNode, faceNode));
            else
                result = result.Replace("%fakedClass%", "");

            ScanEnumerable(faceNode, ref result);

            result += "\t\t#pragma warning restore\r\n";
            result += "\t}\r\n}\r\n";
            return result;
        }

        private static void ScanEnumerable(XElement faceNode, ref string content)
        {
            bool hasEnumerator = EnumerableApi.HasEnumerator(faceNode);
            if (true == hasEnumerator)
                EnumerableApi.AddEnumerator(faceNode, ref content);
            else
                EnumerableApi.RemoveEnumeratorMarker(ref content);
        }

        private static string GetInherited(XElement projectNode, XElement faceNode)
        {
            if (faceNode.Element("Inherited").Elements("Ref").Count() == 0)
                return "COMObject";

            string retList = "";

            // select last interface
            XElement refNode = faceNode.Element("Inherited").Elements("Ref").Last();
            XElement inInterface = GetItemByKey(projectNode, refNode);
            if (inInterface.Parent.Parent == faceNode.Parent.Parent)
            {
                // same project
                retList += inInterface.Attribute("Name").Value;
            }
            else
            {
                // extern project
                retList += inInterface.Parent.Parent.Attribute("Namespace").Value + "." + inInterface.Attribute("Name").Value;
            }

            return retList;
        }

        private static XElement GetItemByKey(XElement projectNode, XElement refEntity)
        {
            XElement solutionNode = projectNode.Parent.Parent;
            string key = refEntity.Attribute("Key").Value;
            foreach (var project in solutionNode.Element("Projects").Elements("Project"))
            {
                var target = (from a in project.Element("Interfaces").Elements("Interface")
                              where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                              select a).FirstOrDefault();
                if (null != target)
                    return target;

                target = (from a in project.Element("DispatchInterfaces").Elements("Interface")
                          where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                          select a).FirstOrDefault();
                if (null != target)
                    return target;

                target = (from a in project.Element("CoClasses").Elements("CoClass")
                          where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                          select a).FirstOrDefault();
                if (null != target)
                    return target;
            }

            throw (new ArgumentException($"refEntity '{key}' not found in project '{projectNode.Attribute("Name")?.Value}'."));
        }
    }
}
