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
        internal static readonly string _projectLine = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"%Name%\",\"%Name%\\%Name%.csproj\", \"{%Key%}\"\r\n%Depend%EndProject\r\n";

        internal static readonly string _buildConfig = "\t\t{%Key%}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\r\n"
                                                     + "\t\t{%Key%}.Debug|Any CPU.Build.0 = Debug|Any CPU\r\n"
                                                     + "\t\t{%Key%}.Release|Any CPU.ActiveCfg = Release|Any CPU\r\n";

        internal static string ReplaceSolutionAttributes(Settings settings, string solutionFile, XElement solution)
        {
            string projects = "";
            string configs = "";

            if (true == settings.AddTestApp)
            {
                string newProjectLine = _projectLine.Replace("%Name%", "ClientApplication");
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
 
            solutionFile = solutionFile.Replace("%Projects%", projects);
            solutionFile = solutionFile.Replace("%Config%", configs);
            return solutionFile;
        }

        internal static void SaveSolutionFile(Settings settings, string path, string solutionFile, XElement solution)
        {
            if (true == settings.AddTestApp)
                SaveTestClient(solution, path);

            string solutionName = solution.Attribute("Name").Value;
            PathApi.CreateFolder(path);
            string solutionFilePath = System.IO.Path.Combine(path, solutionName + ".sln");
            System.IO.File.WriteAllText(solutionFilePath, solutionFile, Encoding.UTF8);
        }

        internal static void SaveApiBinary(Settings settings, string path)
        {
            string framework = settings.Framework.Substring(settings.Framework.LastIndexOf(" ") + 1); 

            PathApi.CreateFolder(path);
            string binrayFilePath = System.IO.Path.Combine(path, "LateBindingApi.Core.dll");
            byte[] ressourceDll = RessourceApi.ReadBinaryFromResource("Api.LateBindingApi.Core" + "_v" + framework + ".dll");
            RessourceApi.WriteBinaryToFile(ressourceDll, binrayFilePath);
        }

        internal static void SaveTestClient(XElement solution, string path)
        {
            string projectFile  = RessourceApi.ReadString("TestClient.ClientApplication.csproj");
            string programFile  = RessourceApi.ReadString("TestClient.Program.cs");
            string formFile     = RessourceApi.ReadString("TestClient.Form1.cs");
            
            string projectRef = "    <ProjectReference Include=\"..\\%Name%\\%Name%.csproj\">\r\n"
                                                   + "      <Project>{%Key%}</Project>\r\n"
                                                   + "      <Name>%Name%</Name>\r\n"
                                                   + "    </ProjectReference>\r\n";

            string formUsings = "";
            string projectInclude = "";
            foreach (var item in solution.Element("Projects").Elements("Project"))
            {
                string newRefProject = projectRef.Replace("%Key%", CSharpGenerator.ValidateGuid(item.Attribute("Key").Value));
                newRefProject = newRefProject.Replace("%Name%", item.Attribute("Name").Value);
                projectInclude += newRefProject;

                string newUsing = "using " + item.Attribute("Namespace").Value + ";\r\n";
                formUsings += newUsing;
            }

            formFile = formFile.Replace("using xyz;", formUsings);

            string refProjectInclude = "  <ItemGroup>\r\n" + projectInclude + "  </ItemGroup>";
            projectFile = projectFile.Replace("%ProjectRefInclude%", refProjectInclude);

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
