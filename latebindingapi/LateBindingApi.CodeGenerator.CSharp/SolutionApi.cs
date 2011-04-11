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

        internal static string ReplaceSolutionAttributes(string solutionFile, XElement solution)
        {
            string projects = "";
            string configs = "";

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

        internal static void SaveSolutionFile(string path, string solutionFile, XElement solution)        
        {
            string solutionName = solution.Attribute("Name").Value;
            PathApi.CreateFolder(path);
            string solutionFilePath = System.IO.Path.Combine(path, solutionName + ".sln");
            System.IO.File.WriteAllText(solutionFilePath, solutionFile, Encoding.UTF8);
        }

        internal static void SaveApiBinary(string path)
        {
            PathApi.CreateFolder(path); 
            string binrayFilePath = System.IO.Path.Combine(path, "LateBindingApi.dll");
            byte[] ressourceDll = RessourceApi.ReadBinaryFromResource("Api.LateBindingApi.dll");
            RessourceApi.WriteBinaryToFile(ressourceDll, binrayFilePath);
        }
    }
}
