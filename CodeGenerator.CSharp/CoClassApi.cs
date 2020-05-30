using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    public static class CoClassApi
    {
        private static string _fileHeader = ""
                                      + "using System;\r\n"
                                      + "using NetRuntimeSystem = System;\r\n"
                                      + "using System.ComponentModel;\r\n"
                                      + "using NetOffice.Attributes;\r\n"
                                      + "\r\n"
                                      + "namespace %namespace%\r\n"
                                      + "{\r\n";

        private static string _delegates = "\t#region Delegates\r\n\r\n" +
                                            "\t#pragma warning disable\r\n" +
                                            "%delegates%" +
                                            "\t#pragma warning restore\r\n" +
                                            "\r\n\t#endregion\r\n\r\n";

        private static string _disposeOverride;

        private static string _classDesc = "\t/// <summary>\r\n\t/// CoClass %name% %RefLibs%\r\n\t/// </summary>\r\n";
        private static string _classRemarks = "\t/// <remarks> MSDN Online: %docLink% </remarks>\r\n";

        private static string _classHeader = "\t[EntityType(EntityType.IsCoClass)%moduleProviderAttribute%]\r\n" +
                                             "%EventSinkAttributes%" +
                                             "\tpublic class %name% : %inherited%%eventBindingInterface%\r\n\t{\r\n" +
                                             "\t\t#pragma warning disable\r\n\r\n";

        private static string _classConstructor;

        private static string _classEventBinding;

        internal static string ConvertCoClassesToFiles(XElement projectNode, XElement classesNode, Settings settings, string solutionFolder)
        {
            string faceFolder = System.IO.Path.Combine(solutionFolder, projectNode.Attribute("Name").Value);
            faceFolder = System.IO.Path.Combine(faceFolder, "Classes");
            if (false == System.IO.Directory.Exists(faceFolder))
                System.IO.Directory.CreateDirectory(faceFolder);

            string result = "";
            foreach (XElement faceNode in classesNode.Elements("CoClass"))
            {
                // ignore the Global coclass in a type library
                if(faceNode.Attribute("Name").Value != "Global")
                    result += ConvertCoClassToFile(projectNode, faceNode, faceFolder) + "\r\n";
            }
            return result;
        }

        private static string ConvertCoClassToFile(XElement projectNode, XElement classNode, string classFolder)
        {
            string fileName = System.IO.Path.Combine(classFolder, classNode.Attribute("Name").Value + ".cs");

            string newEnum = ConvertCoClassToString(projectNode, classNode);
            System.IO.File.AppendAllText(fileName, newEnum);

            int i = classFolder.LastIndexOf("\\");
            string result = "    <Compile Include=\"" + classFolder.Substring(i + 1) + "\\" + classNode.Attribute("Name").Value + ".cs" + "\" />";
            return result;
        }

        private static string ConvertCoClassToString(XElement projectNode, XElement classNode)
        {
            string projectName = projectNode.Attribute("Name").Value;
            string eventsNamespace = EventApi.GetEventsNamespaceName(projectName);

            string result = _fileHeader.Replace("%namespace%", projectNode.Attribute("Namespace").Value);
            string delegates = _delegates.Replace("%delegates%", GetDelegates(projectNode, classNode));
            result += delegates;

            string attributes = "\t" + CSharpGenerator.GetSupportByVersionAttribute(classNode);
            string header = _classHeader.Replace("%name%", classNode.Attribute("Name").Value);
            header = header.Replace("%inherited%", GetInherited(projectNode, classNode));

            if (ImplentsAnEventInterface(projectNode, classNode))
                header = header.Replace("%eventBindingInterface%", ", IEventBinding");
            else
                header = header.Replace("%eventBindingInterface%", "");

            string eventSinkAttributes = GetSinkHelperAttributes(projectNode, classNode, eventsNamespace);
            header = header.Replace("%EventSinkAttributes%", eventSinkAttributes);


            if (null == _classConstructor)
                _classConstructor = ResourceApi.ReadString("CoClass.Constructor.txt");
            string construct = _classConstructor.Replace("%name%", classNode.Attribute("Name").Value);
            construct = construct.Replace("%ProgId%", projectNode.Attribute("Name").Value + "." + classNode.Attribute("Name").Value);
            construct = construct.Replace("%Component%", projectNode.Attribute("Name").Value);
            construct = construct.Replace("%Class%", classNode.Attribute("Name").Value);

            if (null == _disposeOverride)
                _disposeOverride = ResourceApi.ReadString("CoClass.Dispose.txt");

            // the Application coclass represents the global module instance for a type library
            if ("Application" == classNode.Attribute("Name").Value)
            {
                string comProgId = projectName + ".Application";
                header = header.Replace("%moduleProviderAttribute%", ", ComProgId(\"" + comProgId + "\"), ModuleProvider(typeof(GlobalHelperModules.GlobalModule))");
                construct = construct.Replace("%setGlobalInstance%", "\r\n\t\t\tGlobalHelperModules.GlobalModule.Instance = this;");
                construct = construct.Replace("%disposeGlobalInstance%", _disposeOverride);
            }
            else
            {
                header = header.Replace("%moduleProviderAttribute%", "");
                construct = construct.Replace("%setGlobalInstance%", "");
                construct = construct.Replace("%disposeGlobalInstance%", "");
            }


            string sinkHelperDefine = GetSinkHelperDefine(projectNode, classNode, eventsNamespace);
            construct = construct.Replace("%sinkHelperDefine%", sinkHelperDefine);

            string sinkHelperIds = GetSinkHelperIds(projectNode, classNode, eventsNamespace);
            string sinkHelperSetActive = GetSinkHelperSetActiveSink(projectNode, classNode, eventsNamespace);

            string docLink = null;
            if (CSharpGenerator.Settings.AddDocumentationLinks)
            {
                if (CSharpGenerator.IsRootProjectName(projectName))
                {
                    XElement linkNode = (from a in CSharpGenerator.LinkFileDocument.Element("NOBuildTools.ReferenceAnalyzer").Element(projectName).Element("Types").Elements("Type")
                                         where a.Element("Name").Value.Equals(classNode.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase)
                                         select a).FirstOrDefault();
                    if (null != linkNode)
                    {
                        // TODO: Remove old code
                        //docLink = "\r\n\t" + "/// MSDN Online Documentation: " + linkNode.Element("Link").Value;
                        docLink = linkNode.Element("Link")?.Value;
                    }
                }
            }

            string classDesc = _classDesc.Replace("%name%", classNode.Attribute("Name").Value).Replace("%RefLibs%", "\r\n\t/// " + CSharpGenerator.GetSupportByVersion(classNode));
            if (!String.IsNullOrEmpty(docLink))
            {
                string classRemarks = _classRemarks.Replace("%docLink%", docLink);
                classDesc += classRemarks;
            }

            _classEventBinding = ResourceApi.ReadString("CoClass.EventHelper.txt");

            _classEventBinding = _classEventBinding.Replace("%CompareIds%", sinkHelperIds);
            _classEventBinding = _classEventBinding.Replace("%SetActiveSink%", sinkHelperSetActive);

            string sinkHelperDispose = GetSinkHelperDispose(projectNode, classNode);

            string events = GetEvents(projectNode, classNode);
            construct += events;

            if (classNode.Attribute("AutomaticQuit").Value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase))
                construct = construct.Replace("%callQuitInDispose%", "_callQuitInDispose = true;");
            else
                construct = construct.Replace("%callQuitInDispose%", "");

            result += classDesc;
            result += attributes + "\r\n";
            result += header;
            result += construct;

            if (projectNode.Attribute("Name").Value == "Word" && classNode.Attribute("Name").Value == "Application")
                _classEventBinding = _classEventBinding.Replace("= SinkHelper.GetConnectionPoint(this", "= SinkHelper.GetConnectionPoint2(this");

            result += _classEventBinding.Replace("%sinkHelperDispose%", sinkHelperDispose);
            result += "\t\t#pragma warning restore\r\n";
            result += "\t}\r\n}\r\n\r\n";
            return result;
        }

        private static bool ImplentsAnEventInterface(XElement projectNode, XElement faceNode)
        {
            return (faceNode.Element("EventInterfaces").Elements("Ref").Count() > 0);
        }

        private static string GetInherited(XElement projectNode, XElement faceNode)
        {
            if (faceNode.Element("Inherited").Elements("Ref").Count() == 0)
                return "ICOMObject";

            string retList = "";

            // select last default interface
            XElement refNode = faceNode.Element("DefaultInterfaces").Elements("Ref").Last();
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

        private static string GetSinkHelperAttributes(XElement projectNode, XElement faceNode, string eventsNamespace)
        {
            string result = "";
            var eventInterfaces = faceNode.Element("EventInterfaces").Elements("Ref");

            if (eventInterfaces.Any())
            {
                result += "\t[EventSink(";
                foreach (var item in faceNode.Element("EventInterfaces").Elements("Ref"))
                {
                    XElement inInterface = GetItemByKey(projectNode, item);
                    string type = inInterface.Attribute("Name").Value + "_SinkHelper";

                    result += "typeof(" + eventsNamespace + "." + type + "), ";
                }

                if (", " == result.Substring(result.Length - ", ".Length))
                    result = result.Substring(0, result.Length - ", ".Length);

                result += ")]\r\n";

                result += "\t[ComEventInterface(";
                foreach (var item in faceNode.Element("EventInterfaces").Elements("Ref"))
                {
                    XElement inInterface = GetItemByKey(projectNode, item);
                    string type = inInterface.Attribute("Name").Value;

                    result += "typeof(" + eventsNamespace + "." + type + "), ";
                }

                if (", " == result.Substring(result.Length - ", ".Length))
                    result = result.Substring(0, result.Length - ", ".Length);

                result += ")]\r\n";
            }

            return result;
        }

        private static string GetSinkHelperDefine(XElement projectNode, XElement faceNode, string eventsNamespace)
        {
            string result = "";
            foreach (var item in faceNode.Element("EventInterfaces").Elements("Ref"))
            {
                XElement inInterface = GetItemByKey(projectNode, item);
                string type = inInterface.Attribute("Name").Value + "_SinkHelper";
                string name = "_" + type.Substring(0, 1).ToLower() + type.Substring(1);

                result += "\t\tprivate "+ eventsNamespace +"." + type + " " + name + ";\r\n";
            }
            return result;
        }

        private static string GetSinkHelperIds(XElement projectNode, XElement faceNode, string eventsNamespace)
        {
            string result = "";
            foreach (var item in faceNode.Element("EventInterfaces").Elements("Ref"))
            {
                XElement inInterface = GetItemByKey(projectNode, item);
                string type = eventsNamespace + "." + inInterface.Attribute("Name").Value + "_SinkHelper.Id, ";
                result += type;
            }

            if ("" != result)
            {
                if (", " == result.Substring(result.Length - ", ".Length))
                    result = result.Substring(0, result.Length - ", ".Length);
            }
            else
                result = "null";

            return result;
        }

        private static string GetSinkHelperSetActiveSink(XElement projectNode, XElement faceNode, string eventsNamespace)
        {
            string result = "";
            foreach (var item in faceNode.Element("EventInterfaces").Elements("Ref"))
            {
                XElement inInterface = GetItemByKey(projectNode, item);

                string type = inInterface.Attribute("Name").Value + "_SinkHelper";
                string name = "_" + type.Substring(0, 1).ToLower() + type.Substring(1);

                string ifLine = "\r\n\t\t\t" + "if("+ eventsNamespace +"." + type + ".Id.Equals(_activeSinkId, StringComparison.InvariantCultureIgnoreCase))\r\n";
                ifLine += "\t\t\t{\r\n";
                ifLine += "\t\t\t\t" + name + " = new "+ eventsNamespace +"." + type + "(this, _connectPoint);\r\n";
                ifLine += "\t\t\t\treturn;\r\n";
                ifLine += "\t\t\t}\r\n";

                result += ifLine;
            }

            if ("" != result)
            {
                if ("\r\n" == result.Substring(result.Length - "\r\n".Length))
                    result = result.Substring(0, result.Length - "\r\n".Length);
            }

            return result;
        }

        private static string GetSinkHelperDispose(XElement projectNode, XElement faceNode)
        {
            string result = "";
            foreach (var item in faceNode.Element("EventInterfaces").Elements("Ref"))
            {
                XElement inInterface = GetItemByKey(projectNode, item);
                string type = inInterface.Attribute("Name").Value + "_SinkHelper";
                string name = "_" + type.Substring(0, 1).ToLower() + type.Substring(1);

                result += "\t\t\t" + "if( null != " + name + ")\r\n";
                result += "\t\t\t" + "{\r\n";
                result += "\t\t\t\t" + name + ".Dispose();\r\n";
                result += "\t\t\t\t" + name + " = null;\r\n";
                result += "\t\t\t" + "}\r\n";
            }
            return result;
        }

        private static string GetDelegates(XElement projectNode, XElement faceNode)
        {
            List<string> methodNames = new List<string>();
            string delegateResult ="";
            foreach (var refFace in faceNode.Element("EventInterfaces").Elements("Ref"))
            {
                XElement inInterface = GetItemByKey(projectNode, refFace);
                foreach (var itemMethod in inInterface.Element("Methods").Elements("Method"))
                {
                    bool found = false;
                    foreach (string item in methodNames)
                    {
                        if (item == itemMethod.Attribute("Name").Value)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (false == found)
                    {
                        delegateResult += "\t" + GetDelegateSignature(faceNode.Attribute("Name").Value, itemMethod) + "\r\n";
                        methodNames.Add(itemMethod.Attribute("Name").Value);
                    }
                }
            }
            return delegateResult;
        }

        public static string GetDelegateSignature(string className, XElement methodNode)
        {
            string result = "public delegate void " + className + "_" + methodNode.Attribute("Name").Value + "EventHandler" + "(";
            foreach (XElement itemParam in methodNode.Element("Parameters").Elements("Parameter"))
            {
                string isRef = "";
                if ("true" == itemParam.Attribute("IsRef").Value)
                    isRef = "ref ";

                string parameterName = itemParam.Attribute("Name").Value;
                if (parameterName.StartsWith("xml", StringComparison.OrdinalIgnoreCase))
                {
                    parameterName = "xml" + parameterName.Substring(3);
                }
                else if (parameterName.StartsWith("msg", StringComparison.OrdinalIgnoreCase))
                {
                    parameterName = "msg" + parameterName.Substring(3);
                }
                else if (parameterName.StartsWith("url", StringComparison.OrdinalIgnoreCase))
                {
                    parameterName = "url" + parameterName.Substring(3);
                }

                parameterName = ParameterApi.ValidateParamName(parameterName);

                string par = "";
                if(EventApi.IsStruct(itemParam))
                {
                    par = isRef + "object" + " " + parameterName;
                }
                else if (("true" == itemParam.Attribute("IsComProxy").Value) && ("object" == itemParam.Attribute("Type").Value))
                {
                    par = isRef + "ICOMObject" + " " + parameterName;
                }
                else
                {
                    par = isRef + CSharpGenerator.GetQualifiedType(itemParam) + " " + parameterName;
                }

                result += par + ", ";
            }
            if (", " == result.Substring(result.Length - 2))
                result = result.Substring(0, result.Length - 2);

            return result + ");";
        }

        private static XDocument GetImplementEvents(XElement projectNode, XElement faceNode)
        {
            XDocument doc = new XDocument();
            doc.Add(new XElement("Methods"));

            foreach (var item in faceNode.Element("EventInterfaces").Elements("Ref"))
            {
                XElement inInterface = GetItemByKey(projectNode, item);
                string type = inInterface.Attribute("Name").Value;
                foreach (var itemMethod in inInterface.Elements("Methods").Elements("Method"))
                {
                    string methodName = itemMethod.Attribute("Name").Value;

                    var methodNode = (from a in doc.Element("Methods").Elements("Method")
                                      where a.Attribute("Name").Value.Equals(methodName, StringComparison.InvariantCultureIgnoreCase)
                                      select a).FirstOrDefault();
                    if (null == methodNode)
                    {
                        methodNode = new XElement("Method", new XAttribute("Name", methodName));
                        doc.Element("Methods").Add(methodNode);
                    }

                    string[] versions = CSharpGenerator.GetSupportByVersionArray(itemMethod);
                    foreach (string version in versions)
                    {
                        XElement attribute = (from a in methodNode.Elements("Version")
                                              where a.Attribute("Name").Value.Equals(version, StringComparison.InvariantCultureIgnoreCase)
                                              select a).FirstOrDefault();
                        if (null == attribute)
                        {
                            attribute = new XElement("Version", new XAttribute("Name", version));
                            methodNode.Add(attribute);
                        }
                    }

                }

            }

            return doc;
        }

        private static XElement GetTypeNode(XElement methodNode)
        {
            XElement parentNode = methodNode;
            while (parentNode.Name != "CoClass")
                parentNode = parentNode.Parent;

            return parentNode;
        }

        private static XElement GetProjectNode(XElement methodNode)
        {
            XElement parentNode = methodNode;
            while (parentNode.Name != "CoClass")
                parentNode = parentNode.Parent;

            return parentNode;
        }

        private static string GetEvents(XElement projectNode, XElement faceNode)
        {
            string result = "\r\n\r\n\t\t#region Events\r\n\r\n";
            XDocument doc = GetImplementEvents(projectNode, faceNode);
            string line = "";
            foreach (var itemNode in doc.Element("Methods").Elements("Method"))
            {
                string versionAttributeString = "";
                foreach (var itemAttribute in itemNode.Elements("Version"))
                {
                    versionAttributeString += "\"" + itemAttribute.Attribute("Name").Value + "\"";
                    versionAttributeString += ",";
                }
                versionAttributeString = versionAttributeString.Substring(0, versionAttributeString.Length - 1);

                line += "\t\t/// <summary>\r\n" + "\t\t/// SupportByVersion " + projectNode.Attribute("Name").Value + ", " + versionAttributeString.Replace("\"", "") + "\r\n" + "\t\t/// </summary>\r\n";
                line += "\t\tprivate event " + faceNode.Attribute("Name").Value + "_" + itemNode.Attribute("Name").Value +
                   "EventHandler _" + itemNode.Attribute("Name").Value + "Event;\r\n\r\n";

                line += "\t\t/// <summary>\r\n" + "\t\t/// SupportByVersion " + projectNode.Attribute("Name").Value + " " + versionAttributeString.Replace(",", " ").Replace("\"", "").Replace("15 16", "15,16") +
                    "\r\n" + "\t\t/// </summary>\r\n";

                if (CSharpGenerator.Settings.AddDocumentationLinks)
                {
                    string projectName = projectNode.Attribute("Name").Value;
                    if (null != projectName && CSharpGenerator.IsRootProjectName(projectName))
                    {
                        XElement typeDocNode = (from a in CSharpGenerator.LinkFileDocument.Element("NOBuildTools.ReferenceAnalyzer").Element(projectName).Element("Types").Elements("Type")
                                                where a.Element("Name").Value.Equals(faceNode.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase)
                                                select a).FirstOrDefault();

                        if (null != typeDocNode)
                        {
                            XElement linkNode = (from a in typeDocNode.Element("Events").Elements("Event")
                                                 where a.Element("Name").Value.Equals(itemNode.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase)
                                                 select a).FirstOrDefault();

                            if (null != linkNode)
                                line += "\t\t" + "///<remarks> MSDN Online Documentation: " + linkNode.Element("Link").Value + " </remarks>\r\n";
                        }
                    }
                }


                line += "\t\t[SupportByVersion(" + "\"" + projectNode.Attribute("Name").Value + "\"" + ", " + versionAttributeString.Replace("\"", "") + ")]\r\n";

                string field = itemNode.Attribute("Name").Value + "Event";

                line += "\t\tpublic event " + faceNode.Attribute("Name").Value + "_" + itemNode.Attribute("Name").Value +
                    "EventHandler " + itemNode.Attribute("Name").Value + "Event\r\n" + "\t\t{\r\n" +
                    "\t\t\tadd\r\n\t\t\t{\r\n" + "\t\t\t\tCreateEventBridge();\r\n" + "\t\t\t\t" + "_" + field + " += value;" + "\r\n" + "\t\t\t}\r\n" + "\t\t\tremove\r\n" +
                    "\t\t\t{\r\n\t\t\t\t" + "_" + field + " -= value;" + "\r\n\t\t\t}\r\n" +
                    "\t\t}\r\n\r\n";
            }

            result += line;
            result += "\t\t#endregion\r\n";
            return result;
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
