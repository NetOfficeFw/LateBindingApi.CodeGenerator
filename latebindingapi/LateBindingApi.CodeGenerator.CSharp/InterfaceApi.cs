﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class InterfaceApi
    {
        private static string _fileHeader = "//Generated by LateBindingApi.CodeGenerator\r\n"
                                       + "using System;\r\n"
                                       + "%enumerableSpace%"
                                       + "using LateBindingApi.Core;\r\n"
                                       + "namespace %namespace%\r\n"
                                       + "{\r\n";
        
        private static string _classDesc = "\t///<summary>\r\n\t/// Interface %name%\r\n\t///</summary>\r\n";

        private static string _classHeader = "\tpublic class %name% : %inherited%%enumerable%\r\n\t{\r\n";

        private static string _classConstructor = "\t\t#region Construction\r\n\r\n"
                                                + "\t\t///<param name=\"parentObject\">object there has created the proxy</param>\r\n\t\t///<param name=\"comProxy\">inner wrapped COM proxy</param>\r\n"
                                                + "\t\tpublic %name%(COMObject parentObject, object comProxy) : base(parentObject, comProxy)\r\n\t\t{\r\n\t\t}\r\n\r\n\t\t#endregion\r\n";

        
        internal static string ConvertInterfacesToFiles(XElement projectNode, XElement facesNode, Settings settings, string solutionFolder)
        {
            string faceFolder = System.IO.Path.Combine(solutionFolder, projectNode.Attribute("Name").Value);
            faceFolder = System.IO.Path.Combine(faceFolder, "Interfaces");
            if (false == System.IO.Directory.Exists(faceFolder))
                System.IO.Directory.CreateDirectory(faceFolder);

            string result = "";
            foreach (XElement faceNode in facesNode.Elements("Interface"))
                result += ConvertInterfaceToFile(projectNode, faceNode, faceFolder) + "\r\n";

            return result;
        }

        private static string ConvertInterfaceToFile(XElement projectNode, XElement faceNode, string faceFolder)
        {
            string fileName = System.IO.Path.Combine(faceFolder, faceNode.Attribute("Name").Value + ".cs");

            string newEnum = ConvertInterfaceToString(projectNode, faceNode);
            System.IO.File.AppendAllText(fileName, newEnum);

            int i = faceFolder.LastIndexOf("\\");
            string result = "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + faceNode.Attribute("Name").Value + ".cs" + "\" />";
            return result;
        }

        private static string ConvertInterfaceToString(XElement projectNode, XElement faceNode)
        {
            string result = _fileHeader.Replace("%namespace%", projectNode.Attribute("Namespace").Value);
            string attributes = "\t" + CSharpGenerator.GetSupportByLibraryAttribute(faceNode);
            string header = _classHeader.Replace("%name%", faceNode.Attribute("Name").Value);
            header = header.Replace("%inherited%", GetInherited(projectNode, faceNode));
            string construct = _classConstructor.Replace("%name%", faceNode.Attribute("Name").Value);
            string classDesc = _classDesc.Replace("%name%", faceNode.Attribute("Name").Value);

            result += classDesc;
            result += attributes + "\r\n";
            result += header;
            result += construct;

            ScanEnumerable(faceNode, ref result);

            result += "\t}\r\n}";
            return result;
        }

        private static void ScanEnumerable(XElement faceNode, ref string content)
        {
            bool hasEnumerator = EnumerableApi.HasEnumerator(faceNode);
            if (true == hasEnumerator)
                EnumerableApi.AddEnumerator(faceNode,ref content);
            else
                EnumerableApi.RemoveEnumeratorMarker(ref content);
        }

        private static string GetInherited(XElement projectNode, XElement faceNode)
        {
            if (faceNode.Element("Inherited").Elements("Ref").Count() == 0)
                return "COMObject";
            
            string retList ="";
            foreach (var item in faceNode.Element("Inherited").Elements("Ref"))
 	        {
                if (retList != "")
                    retList += ", ";

                XElement inInterface = GetItemByKey(projectNode, item);
                if (inInterface.Parent.Parent == faceNode.Parent.Parent)
                {
                    // same project               
                    retList = inInterface.Attribute("Name").Value;
                }
                else
                {
                    // extern project
                    retList = inInterface.Parent.Parent.Attribute("Namespace").Value + "." + inInterface.Attribute("Name").Value;
                }
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

            throw (new ArgumentException("refEntity not found."));
        }
    }
}
