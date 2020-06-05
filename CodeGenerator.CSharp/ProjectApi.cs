using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class ProjectApi
    {
        private static readonly string _projectRef = "    <ProjectReference Include=\"..\\%Name%\\%Name%Api.csproj\">\r\n"
                                                   + "      <Project>{%Key%}</Project>\r\n"
                                                   + "      <Name>%Name%Api</Name>\r\n"
                                                   + "    </ProjectReference>\r\n";

        private static readonly string[] DefaultSystemLibsImport = { "System", "System.Xml" };

        private static readonly Dictionary<string, string[]> AdditionalLibsImport = new Dictionary<string, string[]>
        {
            { "Office", new [] { "System.Data", "System.Drawing", "System.Windows.Forms" } },
            { "Excel", new [] { "System.Data", "System.Drawing", "System.Windows.Forms" } },
            { "PowerPoint", new [] { "System.Windows.Forms" } },
            { "Word", new [] { "System.Drawing" } }
        };

        private static XElement GetLibraryNode(XElement project)
        { 
            string name = project.Attribute("Name").Value;
  
            return (from a in project.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Libraries").Elements("Library") 
                    where a.Attribute("Name").Value.Equals(name,StringComparison.InvariantCultureIgnoreCase) select a).FirstOrDefault();
        }

        internal static string ReplaceAssemblyAttributes(Settings settings, string solutionPath, string assemblyInfo, XElement project, string typeDefsInclude)
        {
            assemblyInfo = assemblyInfo.Replace("%Title%", project.Attribute("Name").Value);
            assemblyInfo = assemblyInfo.Replace("%Description%", project.Attribute("Description").Value);
            assemblyInfo = assemblyInfo.Replace("%Configuration%", project.Attribute("Configuration").Value);
            assemblyInfo = assemblyInfo.Replace("%Company%", project.Attribute("Company").Value);
            assemblyInfo = assemblyInfo.Replace("%Product%", project.Attribute("Product").Value);
            assemblyInfo = assemblyInfo.Replace("%Copyright%", project.Attribute("Copyright").Value);
            assemblyInfo = assemblyInfo.Replace("%Trademark%", project.Attribute("Trademark").Value);
            assemblyInfo = assemblyInfo.Replace("%Culture%", project.Attribute("Culture").Value);
            assemblyInfo = assemblyInfo.Replace("%Version%", project.Attribute("Version").Value);
            assemblyInfo = assemblyInfo.Replace("%FileVersion%", project.Attribute("FileVersion").Value);
            assemblyInfo = assemblyInfo.Replace("%ImportedTypeLibName%", project.Attribute("Name").Value);
            assemblyInfo = assemblyInfo.Replace("%ImportedTypeLibGuid%", CSharpGenerator.ValidateGuid(GetLibraryNode(project).Attribute("GUID").Value));
     
            assemblyInfo = assemblyInfo.Replace("%AliasInclude%", typeDefsInclude);

            string listAssemblies = "\tName - Description - SupportByVersion\r\n";
            foreach (XElement item in project.Element("RefLibraries").Elements("Ref"))
            {
                XElement libNode = (from a in project.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Libraries").Elements("Library")
                                          where a.Attribute("Key").Value.Equals(item.Attribute("Key").Value)
                                          select a).FirstOrDefault();

                string libInfo = "\t" + libNode.Attribute("Name").Value + " - " +
                                 libNode.Attribute("Description").Value + " - " + 
                                 libNode.Attribute("Version").Value +                    
                                "\r\n";

                listAssemblies += libInfo; 
            }
            assemblyInfo = assemblyInfo.Replace("%List%", listAssemblies.TrimEnd());

            string dependentAssemblies = "";
            string[] depenents = GetRefProjects(project);
            foreach (string item in depenents)
            {
                string refName = "[assembly: Dependency(" + "\"" + item + "Api.dll\"" + ", LoadHint.Default)]" + Environment.NewLine; ;
                dependentAssemblies += refName;
            }
            string defaultRef = "[assembly: Dependency(" + "\"" + "NetOffice.dll\"" + ", LoadHint.Default)]" + Environment.NewLine; ;
            dependentAssemblies += defaultRef;

            assemblyInfo = assemblyInfo.Replace("//%DependencyInclude%", dependentAssemblies);

            return assemblyInfo;
        }
        
        internal static void RemoveRefAttributeInOptionals(XElement projectNode)
        {
            IEnumerable<XElement> paramNodes = (from a in projectNode.Descendants("Parameter")
                                                where a.Attribute("IsRef").Value.Equals("true") &&
                                                      a.Attribute("IsOptional").Value.Equals("true")
                                                select a);

            foreach (var item in paramNodes)
                item.Attribute("IsRef").Value = "false";
        }

        internal static void RemoveRefAttribute(XElement projectNode)
        {
            IEnumerable<XElement> paramNodes = (from a in projectNode.Descendants("Parameter")
                                                where a.Attribute("IsRef").Value.Equals("true")
                                                select a);

            foreach (var item in paramNodes)
            {
                XElement itemParent = item;
                while( (itemParent != null) && (itemParent.Name.LocalName != "Interface") )
                    itemParent = itemParent.Parent;

                if( (null!=itemParent) && (itemParent.Attribute("IsEventInterface").Value != "true") ) 
                    item.Attribute("IsRef").Value = "false";
            }
        }

        internal static string ReplaceProjectAttributes(string solutionPath, string projectFile, Settings settings, XElement project, string enumIncludes,
                                        string constIncludes, string faceIncludes, string dispatchIncludes, string classesIncludes, 
                                                         string eventIncludes, string modulesInclude, string recordsInclude, string toolsInclude, string factoryInclude, string referencedLibraries)
        {

            if("4.0" == settings.Framework)
                projectFile = projectFile.Replace("%ToolsVersion%", "4.0");
            else
                projectFile = projectFile.Replace("%ToolsVersion%", "3.5");

            projectFile = projectFile.Replace("%Key%", CSharpGenerator.ValidateGuid(project.Attribute("Key").Value));
            projectFile = projectFile.Replace("%Name%", project.Attribute("Name").Value + "Api");
            projectFile = projectFile.Replace("%ReferencedLibraries%", referencedLibraries);
            projectFile = projectFile.Replace("%ConstInclude%", constIncludes);
            projectFile = projectFile.Replace("%EnumInclude%", enumIncludes);
            projectFile = projectFile.Replace("%FaceInclude%", faceIncludes);
            projectFile = projectFile.Replace("%DispatchInclude%", dispatchIncludes);
            projectFile = projectFile.Replace("%ClassesInclude%", classesIncludes);
            projectFile = projectFile.Replace("%FactoryInclude%", factoryInclude);
            projectFile = projectFile.Replace("%ModulesInclude%", modulesInclude);
            projectFile = projectFile.Replace("%EventInclude%", eventIncludes);
            projectFile = projectFile.Replace("%RecordsInclude%", recordsInclude);
            projectFile = projectFile.Replace("%ToolsInclude%", toolsInclude);

            if (("3.5" == settings.Framework) || ("4.0" == settings.Framework))
            {
                projectFile = projectFile.Replace("%Framework%", "v" + settings.Framework);
                projectFile = projectFile.Replace("%Profile%", Environment.NewLine + "    <TargetFrameworkProfile>Client</TargetFrameworkProfile>");
            }
            else
            {
                projectFile = projectFile.Replace("%Framework%", "v" + settings.Framework);
                projectFile = projectFile.Replace("%Profile%", "");
            }

            string refProjectInclude = "";
            if (project.Element("RefProjects").Elements("RefProject").Count() > 0)
            {
                XElement projects = project.Parent;
                foreach (XElement item in project.Element("RefProjects").Elements("RefProject"))
                {
                    
                    XElement refProject = (from a in projects.Elements("Project")
                                           where a.Attribute("Key").Value.Equals(item.Attribute("Key").Value)
                                           select a).FirstOrDefault();

                    if ("true" == refProject.Attribute("Ignore").Value)
                        continue;

                    string newRefProject = _projectRef.Replace("%Key%", CSharpGenerator.ValidateGuid(refProject.Attribute("Key").Value));
                    newRefProject = newRefProject.Replace("%Name%", refProject.Attribute("Name").Value);
                    refProjectInclude += newRefProject;
                    
                }

                refProjectInclude += _projectRef.Replace("Api", "").Replace("%Name%", "NetOffice").Replace("%Key%", "65442327-D01F-4ECB-8C39-6D5C7622A80F");

                if("" != refProjectInclude)
                    refProjectInclude = "  <ItemGroup>\r\n" + refProjectInclude + "  </ItemGroup>";
            }

            projectFile = projectFile.Replace("%ProjectRefInclude%", refProjectInclude);

            if (true == settings.UseSigning)
            {
                string projectName = project.Attribute("Name").Value;
                var snkFilename = projectName + "Api_v" + settings.Framework + ".snk";
                string keyFile = System.IO.Path.Combine(settings.SignPath, settings.Framework + "\\" + snkFilename);
                if (System.IO.File.Exists(keyFile))
                {
                    string projectPath = System.IO.Path.Combine(solutionPath, projectName);
                    string copyKeyFile = System.IO.Path.Combine(projectPath, snkFilename);
                    PathApi.CreateFolder(projectPath);
                    System.IO.File.Copy(keyFile, copyKeyFile);
                }

                string signAssemblyItem = "  <ItemGroup>\r\n" +
                    "    <None Include=\"" + snkFilename + "\" />" + "\r\n" +
                    "  </ItemGroup>";
                projectFile = projectFile.Replace("%SignAssemblyFile%", signAssemblyItem);
            }
            else
            {
                int firstPos = projectFile.IndexOf("<SignAssembly>");
                int lastPos = projectFile.IndexOf("</AssemblyOriginatorKeyFile>");

                string firstString = projectFile.Substring(0, firstPos);
                string lastString = projectFile.Substring(lastPos + "</AssemblyOriginatorKeyFile>".Length);

                projectFile = firstString + lastString;
                projectFile = projectFile.Replace("%SignAssemblyFile%", "");
            }
            return projectFile;
        }

        internal static string RemoveProjectRef(string projectFile)
        { 
                string begin = "<Reference Include=\"NetOffice,";
                string end = "</Reference>";

                int start = projectFile.IndexOf(begin);
                int lenght = projectFile.Substring(start).IndexOf(end);

                string part1 = projectFile.Substring(0, start);
                string part2 = projectFile.Substring(start + lenght + end.Length + "\r\n".Length);

                return part1 + part2; 
        }

        internal static string[] GetRefProjects(XElement project)
        {
             List<string> refsList = new List<string>();
             XElement projects = project.Parent;
             foreach (XElement item in project.Element("RefProjects").Elements("RefProject"))
             {
                 XElement refProject = (from a in projects.Elements("Project")
                                        where a.Attribute("Key").Value.Equals(item.Attribute("Key").Value)
                                        select a).FirstOrDefault();

                 if ("true" == refProject.Attribute("Ignore").Value)
                     continue;
                 else
                     refsList.Add(refProject.Attribute("Name").Value);
             }

             return refsList.ToArray();
        }

        internal static string SaveFactoryFile(string path, XElement project)
        {
            string projectName = project.Attribute("Name").Value;
            string namespaceString = project.Attribute("Namespace").Value;
            XElement libNode = (from a in project.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Libraries").Elements("Library")
                                where a.Attribute("Key").Value.Equals(project.Element("RefLibraries").Element("Ref").Attribute("Key").Value)
                                select a).FirstOrDefault();

            string guidString = "";
            //System.Windows.Forms.MessageBox.Show("UNTESTED");
            if (libNode.Attribute("Name").Value == "DAO")
            {
                guidString = "new Guid(\"" + XmlConvert.DecodeName(libNode.Attribute("GUID").Value) + "\"), new Guid(\"4AC9E1DA-5BAD-4AC7-86E3-24F4CDCECA28\")";
            }
            else
            {
                guidString = "new Guid(\"" + XmlConvert.DecodeName(libNode.Attribute("GUID").Value) + "\")";
            }

            string dependent = "";
            string[] depenents = GetRefProjects(project);
            if (depenents.Length > 0)
            {
                dependent = "[]{";
                foreach (string item in depenents)
                    dependent += "\"" + item + "Api.dll\",";
                dependent = dependent.Substring(0, dependent.Length - 1) +"}";
            }
            else
            {
                dependent = "[0]";
            }

            string factoryFile = ResourceApi.ReadString("Project.ProjectInfo.txt");
            factoryFile = factoryFile.Replace("%AssemblyName%", projectName);
            factoryFile = factoryFile.Replace("%Namespace%", namespaceString);
            factoryFile = factoryFile.Replace("%GUID%", guidString);
            factoryFile = factoryFile.Replace("%dependencies%", dependent);

            string fileName = "ProjectInfo.cs";
            string projectPath = System.IO.Path.Combine(path, project.Attribute("Name").Value +"\\Utils");
            PathApi.CreateFolder(projectPath);
            string assemblyFilePath = System.IO.Path.Combine(projectPath, fileName);
            System.IO.File.WriteAllText(assemblyFilePath, factoryFile, Encoding.UTF8);
            int i = assemblyFilePath.LastIndexOf("\\");
            return "    <Compile Include=\"" +  "Utils\\" + fileName + "\" />";
        }

        internal static void SaveAssemblyInfoFile(string path, string assemblyFile, XElement project)
        {
            string fileName = "AssemblyInfo.cs";
            string projectPath = System.IO.Path.Combine(path, project.Attribute("Name").Value);
            PathApi.CreateFolder(projectPath);
            string assemblyFilePath = System.IO.Path.Combine(projectPath, fileName);
            System.IO.File.WriteAllText(assemblyFilePath, assemblyFile, Encoding.UTF8);
        }

        internal static void SaveProjectFile(string path, string projectFile, XElement project)
        {
            string projectName = project.Attribute("Name").Value;
            string projectPath = System.IO.Path.Combine(path, projectName);
            PathApi.CreateFolder(projectPath);
            string projectFilePath = System.IO.Path.Combine(projectPath, projectName + "Api.csproj");
            System.IO.File.WriteAllText(projectFilePath, projectFile, Encoding.UTF8);
        }

        public static string GetReferencedLibraries(XElement project, Settings settings, string solutionFolder)
        {
            var projectName = project.Attribute("Name").Value;

            var referenced = new List<string>(DefaultSystemLibsImport);
            string[] additionalReferences;
            if (AdditionalLibsImport.TryGetValue(projectName, out additionalReferences))
            {
                referenced.AddRange(additionalReferences);
                referenced.Sort(StringComparer.Ordinal);
            }
            
            var sb = new StringBuilder(1024);
            foreach (var libraryName in referenced)
            {
                sb.AppendLine($@"    <Reference Include=""{libraryName}"" />");
            }

            return sb.ToString();
        }
    }
}
