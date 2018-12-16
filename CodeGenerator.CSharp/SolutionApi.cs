using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class SolutionApi
    {
        internal static readonly string _projectLine = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"%Name%Api\", \"%Name%\\%Name%Api.csproj\", \"{%Key%}\"\r\n%Depend%EndProject\r\n";

        internal static readonly string _buildConfig = "\t\t{%Key%}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\r\n"
                                                     + "\t\t{%Key%}.Debug|Any CPU.Build.0 = Debug|Any CPU\r\n"
                                                     + "\t\t{%Key%}.Release|Any CPU.ActiveCfg = Release|Any CPU\r\n"
                                                     + "\t\t{%Key%}.Release|Any CPU.Build.0 = Release|Any CPU\r\n";

        internal static string ReplaceSolutionAttributes(Settings settings, string solutionFile, XElement solution)
        {
            string projects = "";
            string configs = "";

            if (settings.Framework == "4.0")
            {
                solutionFile = solutionFile.Replace("%FormatVersion%", "12.00");
                solutionFile = solutionFile.Replace("%VisualStudio%", "Visual Studio 14\r\nVisualStudioVersion = 14.0.25420.1\r\nMinimumVisualStudioVersion = 10.0.40219.1");
            }
            else
            {
                solutionFile = solutionFile.Replace("%FormatVersion%", "10.00");
                solutionFile = solutionFile.Replace("%VisualStudio%", "Visual Studio 2008");
            }

            if (true == settings.AddTestApp)
            {
                string testProjectLine = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"%Name%\", \"%Name%\\%Name%.csproj\", \"{%Key%}\"\r\n%Depend%EndProject\r\n";
                string newProjectLine = testProjectLine.Replace("%Name%", "ClientApplication");
                newProjectLine = newProjectLine.Replace("%Key%", "DF73F99F-DFC0-42D1-9EDF-AD7D890C53D5");

                string depends = "\tProjectSection(ProjectDependencies) = postProject\r\n";
                foreach (var item in solution.Element("Projects").Elements("Project"))
                {
                    string line = "\t\t" + "{%Key%} = {%Key%}" + "\r\n";
                    depends += line.Replace("%Key%", CSharpGenerator.ValidateGuid(item.Attribute("Key").Value));
                }
                depends += "\tEndProjectSection\r\n";
                newProjectLine = newProjectLine.Replace("%Depend%", depends);
                projects += newProjectLine;

                string newConfig = _buildConfig.Replace("%Key%", "DF73F99F-DFC0-42D1-9EDF-AD7D890C53D5");
                configs += newConfig;
            }

            foreach (var project in solution.Element("Projects").Elements("Project"))
            {
                if ("true" == project.Attribute("Ignore").Value)
                    continue;

                string projectName = project.Attribute("Name").Value;
                string newProjectLine = _projectLine.Replace("%Name%", projectName);
                newProjectLine = newProjectLine.Replace("%Key%", CSharpGenerator.ValidateGuid(project.Attribute("Key").Value));
                string depends = "";
                foreach (var item in project.Element("RefProjects").Elements("RefProject"))
                {
                    string projKey = item.Attribute("Key").Value;
                    XElement projNode = (from a in solution.Element("Projects").Elements("Project")
                                        where a.Attribute("Key").Value.Equals(projKey)
                                        select a).FirstOrDefault();

                    // skip referenced project which are ignored by generator
                    if ("true" == projNode.Attribute("Ignore").Value)
                        continue;

                    string line = "\t\t" + "{%Key%} = {%Key%}" + "\r\n";
                    string refProjectGuid = CSharpGenerator.ValidateGuid(projNode.Attribute("Key").Value);
                    depends += line.Replace("%Key%", refProjectGuid);
                }

                if (!String.IsNullOrEmpty(depends))
                {
                    depends = "\tProjectSection(ProjectDependencies) = postProject\r\n"
                            + depends
                            + "\tEndProjectSection\r\n";
                }

                newProjectLine = newProjectLine.Replace("%Depend%", depends);
                projects += newProjectLine;

                string newConfig = _buildConfig.Replace("%Key%", CSharpGenerator.ValidateGuid(project.Attribute("Key").Value));
                configs += newConfig;

                // HACK: special hack to order NetOffice project after VBIDEApi to match the official solution file
                if (projectName == "VBIDE")
                {
                    string newProjectLine2 = _projectLine.Replace("Api", "").Replace("%Name%", "NetOffice").Replace("%Key%", "65442327-D01F-4ECB-8C39-6D5C7622A80F").Replace("%Depend%", "");
                    projects += newProjectLine2;

                    string newConfig2 = _buildConfig.Replace("%Key%", "65442327-D01F-4ECB-8C39-6D5C7622A80F");
                    configs += newConfig2;
                }
            }

            projects = projects.TrimEnd();
            configs = configs.TrimEnd();
            solutionFile = solutionFile.Replace("%Projects%", projects);
            solutionFile = solutionFile.Replace("%Config%", configs);
            return solutionFile;
        }

        internal static void SaveSolutionFile(Settings settings, string path, string solutionFile, XElement solution)
        {
            if (true == settings.AddTestApp)
                SaveTestClient(settings, solution, path);

            string solutionName = solution.Attribute("Name").Value;
            PathApi.CreateFolder(path);
            string solutionFilePath = System.IO.Path.Combine(path, solutionName + ".sln");
            System.IO.File.WriteAllText(solutionFilePath, solutionFile, Encoding.UTF8);
        }

        internal static void CopyNetOfficeProject(Settings settings, string sourceProjectPath, string targetPath)
        {
            targetPath = Path.Combine(targetPath, "NetOffice");
            var sourceDir = new DirectoryInfo(sourceProjectPath);

            sourceDir.CopyTo(targetPath);

            var binDir = Path.Combine(targetPath, "bin");
            var objDir = Path.Combine(targetPath, "obj");

            try
            {
                Directory.Delete(binDir, true);
                Directory.Delete(objDir, true);
            }
            catch (Exception)
            {
            }
        }

        internal static void SaveTestClient(Settings settings, XElement solution, string path)
        {
            string projectFile  = RessourceApi.ReadString("TestClient.ClientApplication.csproj");
            string programFile  = RessourceApi.ReadString("TestClient.Program.cs");
            string formFile     = RessourceApi.ReadString("TestClient.Form1.cs");

            string projectRef = "    <ProjectReference Include=\"..\\%Name%\\%Name%Api.csproj\">\r\n"
                                                   + "      <Project>{%Key%}</Project>\r\n"
                                                   + "      <Name>%Name%Api</Name>\r\n"
                                                   + "    </ProjectReference>\r\n";

            string formUsings = "";
            string projectInclude = "";
            foreach (var item in solution.Element("Projects").Elements("Project"))
            {
                if ("true" == item.Attribute("Ignore").Value)
                    continue;

                string newRefProject = projectRef.Replace("%Key%", CSharpGenerator.ValidateGuid(item.Attribute("Key").Value));
                newRefProject = newRefProject.Replace("%Name%", item.Attribute("Name").Value);
                projectInclude += newRefProject;

                string newUsing = "using " + item.Attribute("Name").Value + " = " + item.Attribute("Namespace").Value + ";\r\n";
                formUsings += newUsing;
            }

            string newRefProject2 = projectRef.Replace("Api", "").Replace("%Key%", "65442327-D01F-4ECB-8C39-6D5C7622A80F");
            newRefProject2 = newRefProject2.Replace("%Name%", "NetOffice");
            projectInclude += newRefProject2;
            projectFile = projectFile.Replace("%ApiRefDll%", "");

            formFile = formFile.Replace("using xyz;", formUsings);

            string refProjectInclude = "  <ItemGroup>\r\n" + projectInclude + "  </ItemGroup>";
            projectFile = projectFile.Replace("%ProjectRefInclude%", refProjectInclude);

            if ("4.0" == settings.Framework)
                projectFile = projectFile.Replace("%ToolsVersion%", "4.0");
            else
                projectFile = projectFile.Replace("%ToolsVersion%", "3.5");

            projectFile = projectFile.Replace("%Framework%", "v" + settings.Framework);

            string projectPath = System.IO.Path.Combine(path, "ClientApplication");
            PathApi.CreateFolder(projectPath);
            string projectFilePath = System.IO.Path.Combine(projectPath, "ClientApplication.csproj");
            System.IO.File.WriteAllText(projectFilePath, projectFile, Encoding.UTF8);

            string programFilePath = System.IO.Path.Combine(projectPath, "Program.cs");
            System.IO.File.WriteAllText(programFilePath, programFile, Encoding.UTF8);

            string formFilePath = System.IO.Path.Combine(projectPath, "Form1.cs");
            System.IO.File.WriteAllText(formFilePath, formFile, Encoding.UTF8);
        }
    }
}
