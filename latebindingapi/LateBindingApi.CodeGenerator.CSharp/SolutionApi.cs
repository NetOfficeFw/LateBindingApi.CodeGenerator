using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class SolutionApi
    {
        internal static readonly string _projectLine = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"%Name%Api\",\"%Name%\\%Name%Api.csproj\", \"{%Key%}\"\r\n%Depend%EndProject\r\n";

        internal static readonly string _buildConfig = "\t\t{%Key%}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\r\n"
                                                     + "\t\t{%Key%}.Debug|Any CPU.Build.0 = Debug|Any CPU\r\n"
                                                     + "\t\t{%Key%}.Release|Any CPU.ActiveCfg = Release|Any CPU\r\n";

        internal static string ReplaceSolutionAttributes(Settings settings, string solutionFile, XElement solution)
        {
            string projects = "";
            string configs = "";

            if (settings.Framework == "4.0")
            {
                solutionFile = solutionFile.Replace("%FormatVersion%", "11.00");
                solutionFile = solutionFile.Replace("%VisualStudio%", "Visual C# Express 2010");
            }
            else
            {
                solutionFile = solutionFile.Replace("%FormatVersion%", "10.00");
                solutionFile = solutionFile.Replace("%VisualStudio%", "Visual Studio 2008");
            }

            if (true == settings.AddTestApp)
            {
                string testProjectLine = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"%Name%\",\"%Name%\\%Name%.csproj\", \"{%Key%}\"\r\n%Depend%EndProject\r\n";
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

                string newProjectLine = _projectLine.Replace("%Name%", project.Attribute("Name").Value);
                newProjectLine = newProjectLine.Replace("%Key%", CSharpGenerator.ValidateGuid(project.Attribute("Key").Value));
                string depends = "";
                if(project.Element("RefProjects").Elements("RefProject").Count() > 0)
                    depends += "\tProjectSection(ProjectDependencies) = postProject\r\n";

                foreach (var item in project.Element("RefProjects").Elements("RefProject"))
                {
                    string projKey = item.Attribute("Key").Value;
                    XElement projNode = (from a in solution.Element("Projects").Elements("Project")
                                        where a.Attribute("Key").Value.Equals(projKey)
                                        select a).FirstOrDefault();
                    string line = "\t\t" + "{%Key%} = {%Key%}" + "\r\n";
                    depends += line.Replace("%Key%", CSharpGenerator.ValidateGuid(projNode.Attribute("Key").Value));               
                }
                 
                if (project.Element("RefProjects").Elements("RefProject").Count() > 0)
                    depends += "\tEndProjectSection\r\n";

                newProjectLine = newProjectLine.Replace("%Depend%", depends);
                projects += newProjectLine;
               
                string newConfig = _buildConfig.Replace("%Key%", CSharpGenerator.ValidateGuid(project.Attribute("Key").Value));
                configs += newConfig; 
            }

            if (!settings.UseApiAssembly)
            {
                string newProjectLine = _projectLine.Replace("Api", "").Replace("%Name%", "LateBindingApi.Core").Replace("%Key%", "65442327-D01F-4ECB-8C39-6D5C7622A80F").Replace("%Depend%", "");
                projects += newProjectLine;

                string newConfig = _buildConfig.Replace("%Key%", "65442327-D01F-4ECB-8C39-6D5C7622A80F");
                configs += newConfig; 
            }

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

        internal static void SaveApiProject(Settings settings, string projectApiPath, string path)
        {
            path = System.IO.Path.Combine(path, "LateBindingApi.Core");
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            string[] files = System.IO.Directory.GetFiles(projectApiPath);
            foreach (string file in files)
            {
                string fileName = System.IO.Path.GetFileName(file);
                string newFilePath = System.IO.Path.Combine(path, fileName);
                System.IO.File.Copy(file, newFilePath);
            }

            if (!System.IO.Directory.Exists(path + "\\Interfaces"))
                System.IO.Directory.CreateDirectory(path + "\\Interfaces");
            files = System.IO.Directory.GetFiles(projectApiPath + "\\Interfaces");
            foreach (string file in files)
            {
                string fileName = System.IO.Path.GetFileName(file);
                string newFilePath = System.IO.Path.Combine(path + "\\Interfaces", fileName);
                System.IO.File.Copy(file, newFilePath);
            }

            if (!System.IO.Directory.Exists(path + "\\Properties"))
                System.IO.Directory.CreateDirectory(path + "\\Properties");
            files = System.IO.Directory.GetFiles(projectApiPath + "\\Properties");
            foreach (string file in files)
            {
                string fileName = System.IO.Path.GetFileName(file);
                string newFilePath = System.IO.Path.Combine(path + "\\Properties", fileName);
                System.IO.File.Copy(file, newFilePath);
            }
        }

        internal static void SaveApiBinary(Settings settings, string path)
        {
            PathApi.CreateFolder(path);
            string binrayFilePath = System.IO.Path.Combine(path, "LateBindingApi.Core.dll");
            byte[] ressourceDll = RessourceApi.ReadBinaryFromResource("Api.LateBindingApi.Core" + "_v" + settings.Framework + ".dll");
            RessourceApi.WriteBinaryToFile(ressourceDll, binrayFilePath);
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

            if (!settings.UseApiAssembly)
            {
                string newRefProject = projectRef.Replace("Api", "").Replace("%Key%", "65442327-D01F-4ECB-8C39-6D5C7622A80F");
                newRefProject = newRefProject.Replace("%Name%", "LateBindingApi.Core");
                projectInclude += newRefProject;
            }

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
