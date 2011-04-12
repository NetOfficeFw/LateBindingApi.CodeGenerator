using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using System.Linq;

using LateBindingApi.CodeGenerator.ComponentAnalyzer;

namespace LateBindingApi.CodeGenerator.CSharp
{
    public class CSharpGenerator : ICodeGenerator
    {
        #region Fields

        DateTime _startTimeOperation;
        Settings _settings;
        XDocument _document;
        ThreadJob _job = new ThreadJob();

        #endregion

        #region Construction
        
        public CSharpGenerator()
        {
            _job.DoWork += new System.Threading.ThreadStart(_job_DoWork);
            _job.RunWorkerCompleted += new ThreadCompletedEventHandler(_job_RunWorkerCompleted);
        }

        void _job_RunWorkerCompleted()
        {
            if (null != Finish)
            {
                TimeSpan ts = DateTime.Now - _startTimeOperation;
                Finish(ts);
            }
        }

        private void DoUpdate(string message)
        {
            if (null != Progress)
                Progress(message);
        }

        #endregion
        
        #region ICodeGenerator Members

        public event ICodeGeneratorProgressHandler Progress;
        public event ICodeGeneratorFinishHandler Finish;

        public bool IsAlive
        {
            get
            {
                return _job.IsAlive;
            }
        }

        public string Name
        {
            get 
            {
                return "C#";
            }
        }

        public string Description
        {
            get
            {
                return "Creates a VS 2008 Solution with C# Projects (.csproj)";
            }
        }

        public Version Version
        {
            get
            {
                return new Version("1.0");
            }
        }

        public void Abort()
        {
            _job.Abort(); 
        }

        public DialogResult ShowConfigDialog(Control parentDialog)
        {
            FormConfigDialog formConfig = new FormConfigDialog();
            DialogResult dr = formConfig.ShowDialog(parentDialog);
            if (dr == DialogResult.OK)
            {
                _settings = formConfig.Selected;
                return dr;
            }
            else
                return dr;
        }

        public void Generate(XDocument document)
        {
            _document = document;
            _job.Start();
            _startTimeOperation = DateTime.Now;
        }

        void _job_DoWork()
        {
            DoUpdate("Create root folder");
            XElement solution = _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution");
            string solutionFolder = System.IO.Path.Combine(_settings.Folder, solution.Attribute("Name").Value);
            PathApi.ClearCreateFolder(solutionFolder);

            var projects = _document.Descendants("Project");
            foreach (var project in projects)
            {
                DoUpdate("Create project " + project.Attribute("Name").Value);
                string projectFile = RessourceApi.ReadString("Project.Project.csproj");
                string assemblyInfo = RessourceApi.ReadString("Project.AssemblyInfo.cs");
                string constIncludes = ConstantApi.ConvertConstantsToFiles(project, project.Element("Constants"), _settings, solutionFolder);
                string enumIncludes = EnumsApi.ConvertEnumsToFiles(project, project.Element("Enums"), _settings, solutionFolder);
                string faceIncludes = InterfaceApi.ConvertInterfacesToFiles(project, project.Element("Interfaces"), _settings, solutionFolder);
                string dispatchIncludes = DispatchApi.ConvertInterfacesToFiles(project, project.Element("DispatchInterfaces"), _settings, solutionFolder);
                string factoryInclude = ProjectApi.SaveFactoryFile(solutionFolder, project);

                assemblyInfo = ProjectApi.ReplaceAssemblyAttributes(assemblyInfo, project);
                projectFile = ProjectApi.ReplaceProjectAttributes(projectFile, _settings, project, enumIncludes, constIncludes, faceIncludes, dispatchIncludes, factoryInclude);

                ProjectApi.SaveAssemblyInfoFile(solutionFolder, assemblyInfo, project);
                ProjectApi.SaveProjectFile(solutionFolder, projectFile, project);
            }

            DoUpdate("Create Solution");
            string solutionFile = RessourceApi.ReadString("Solution.Solution.sln");
            solutionFile = SolutionApi.ReplaceSolutionAttributes(solutionFile, solution);
            SolutionApi.SaveSolutionFile(solutionFolder, solutionFile, solution);
            SolutionApi.SaveApiBinary(_settings, solutionFolder);

            if (true == _settings.OpenFolder)
                System.Diagnostics.Process.Start(solutionFolder);
        }
 
        #endregion

        /// <summary>
        /// returns support libary versions for entityNode
        /// </summary>
        /// <param name="entityNode"></param>
        /// <returns></returns>
        internal static string GetSupportByLibraryAttribute(XElement entityNode)
        {
            string result = "";
            string versions = "";

            XElement refLibs = entityNode.Element("RefLibraries");
            foreach (XElement refLib in refLibs.Elements())
            {
                string key = refLib.Attribute("Key").Value;
                XElement libNode = (from a in entityNode.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Libraries").Elements("Library")
                                    where a.Attribute("Key").Value.Equals(key)
                                    select a).FirstOrDefault();
                string versionAttribute = libNode.Attribute("Version").Value;
                versions += "\"" + versionAttribute + "\",";
            }
            versions = versions.Substring(0, versions.Length - 1);
            result += "[SupportByLibrary(" + versions + ")]";
            return result;
        }

        /// <summary>
        /// returns xml encoded guid, convert low chars to upper
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        internal static string ValidateGuid(string guid)
        {
            guid = XmlConvert.DecodeName(guid);
            string validGuid = "";
            char[] array = guid.ToCharArray();
            foreach (char item in array)
            {
                if ((item >= 64) && (item <= 122))
                    validGuid += Convert.ToString(item).ToUpper();
                else
                    validGuid += Convert.ToString(item);
            }
            return validGuid;
        }
    }
}
