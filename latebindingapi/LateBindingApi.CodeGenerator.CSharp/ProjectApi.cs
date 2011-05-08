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

        internal static string ReplaceAssemblyAttributes(string assemblyInfo, XElement project, string typeDefsInclude)
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

            assemblyInfo = assemblyInfo.Replace("%AliasInclude%", typeDefsInclude);

            string listAssemblies = "\tName - Description - SupportByLibrary\r\n";
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
            assemblyInfo = assemblyInfo.Replace("%List%", listAssemblies);

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
                item.Attribute("IsRef").Value = "false";
        }

        internal static string ReplaceProjectAttributes(string projectFile, Settings settings, XElement project, string enumIncludes,
                                        string constIncludes, string faceIncludes, string dispatchIncludes, string classesIncludes, 
                                                         string eventIncludes, string modulesInclude, string recordsInclude, string factoryInclude)
        {

            if("4.0" == settings.Framework)
                projectFile = projectFile.Replace("%ToolsVersion%", "4.0");
            else
                projectFile = projectFile.Replace("%ToolsVersion%", "3.5");

            projectFile = projectFile.Replace("%Key%", CSharpGenerator.ValidateGuid(project.Attribute("Key").Value));
            projectFile = projectFile.Replace("%Name%", project.Attribute("Name").Value + "Api");
            projectFile = projectFile.Replace("%ConstInclude%", constIncludes);
            projectFile = projectFile.Replace("%EnumInclude%", enumIncludes);
            projectFile = projectFile.Replace("%FaceInclude%", faceIncludes);
            projectFile = projectFile.Replace("%DispatchInclude%", dispatchIncludes);
            projectFile = projectFile.Replace("%ClassesInclude%", classesIncludes);
            projectFile = projectFile.Replace("%FactoryInclude%", factoryInclude);
            projectFile = projectFile.Replace("%ModulesInclude%", modulesInclude);
            projectFile = projectFile.Replace("%EventInclude%", eventIncludes);
            projectFile = projectFile.Replace("%RecordsInclude%", recordsInclude);

            projectFile = projectFile.Replace("%Framework%", "v" +  settings.Framework);

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
                if (!settings.UseApiAssembly)
                {
                    refProjectInclude += _projectRef.Replace("Api", "").Replace("%Name%", "LateBindingApi.Core").Replace("%Key%", "65442327-D01F-4ECB-8C39-6D5C7622A80F");
                    projectFile = RemoveProjectRef(projectFile);
                }
                if("" != refProjectInclude)
                    refProjectInclude = "  <ItemGroup>\r\n" + refProjectInclude + "  </ItemGroup>";
            }

            projectFile = projectFile.Replace("%ProjectRefInclude%", refProjectInclude);
            return projectFile;
        }

        internal static string RemoveProjectRef(string projectFile)
        { 
                string begin = "<Reference Include=\"LateBindingApi.Core,";
                string end = "</Reference>";

                int start = projectFile.IndexOf(begin);
                int lenght = projectFile.Substring(start).IndexOf(end);

                string part1 = projectFile.Substring(0, start);
                string part2 = projectFile.Substring(start + lenght + end.Length + "\r\n".Length);

                return part1 + part2; 
        }

        internal static string SaveFactoryFile(string path, XElement project)
        {
            string projectName = project.Attribute("Name").Value;
            string namespaceString = project.Attribute("Namespace").Value;
            XElement libNode = (from a in project.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Libraries").Elements("Library")
                                where a.Attribute("Key").Value.Equals(project.Element("RefLibraries").Element("Ref").Attribute("Key").Value)
                                select a).FirstOrDefault();
            string guidString = XmlConvert.DecodeName(libNode.Attribute("GUID").Value);
             
            string factoryFile = RessourceApi.ReadString("Project.ProjectInfo.txt");
            factoryFile = factoryFile.Replace("%AssemblyName%", projectName);
            factoryFile = factoryFile.Replace("%Namespace%", namespaceString);
            factoryFile = factoryFile.Replace("%GUID%", guidString);

            string fileName = "ProjectInfo.cs";
            string projectPath = System.IO.Path.Combine(path, project.Attribute("Name").Value +"\\Utils");
            PathApi.CreateFolder(projectPath);
            string assemblyFilePath = System.IO.Path.Combine(projectPath, fileName);
            System.IO.File.WriteAllText(assemblyFilePath, factoryFile, Encoding.UTF8);
            int i = assemblyFilePath.LastIndexOf("\\");
            return "\t\t<Compile Include=\"" +  "Utils\\" + fileName + "\" />";
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
    }
}
