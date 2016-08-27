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
        static Settings _settings;
        static XDocument _document;
        static DubletteManager _dublettes;
        static DerivedManager _derives;
        static FakedEnumeratorManager _enumerators;
        static CustomMethodManager _customMethods;
        ThreadJob _job = new ThreadJob();
         
        #endregion

        #region Properties

        public static Settings Settings
        {
            get
            {
                return _settings;
            }
        }

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

        private XDocument CreateWorkingCopy()
        {            
            XDocument document = new XDocument(_document);
            return document;
        }

        internal static  XDocument LinkFileDocument
        {
            get 
            {
                if (null == _linkFileDocument)
                    _linkFileDocument = XDocument.Load(Settings.LinkFilePath);
                return _linkFileDocument;
            }
        }
        private static XDocument _linkFileDocument;

        internal static bool IsRootProjectName(string name)
        {
            switch (name)
            {
                case "Excel":
                case "Word":
                case "Outlook":
                case "PowerPoint":
                case "Access":
                case "MSProject":
                case "Visio":
                case "Office":
                    return true;
                default:
                    return false;
            }
        }

        void _job_DoWork()
        {
            DoUpdate("Create Copy");
            XElement solution = CreateWorkingCopy().Element("LateBindingApi.CodeGenerator.Document").Element("Solution");

            DoUpdate("Scan for duplicated interfaces");
            _dublettes = new DubletteManager(this, solution.Document);
            _dublettes.ScanForDublettes();

            DoUpdate("Scan for derived interfaces");
            _derives = new DerivedManager(this, solution.Document);
            _derives.ScanForDerived();

            DoUpdate("Scan for missed enumerators");
            _enumerators = new FakedEnumeratorManager(this, _document);
            _enumerators.ScanForMissedEnumerators();
            
            DoUpdate("Scan for optional parameter methods");
            _customMethods = new CustomMethodManager(this, _document);
            _customMethods.ScanForOptionalMethods();

            DoUpdate("Scan for name conflicts");
            ConflictManager conflictManager = new ConflictManager(this, _document);
            conflictManager.ScanForConflicts();
             
            DoUpdate("Scan for CoClasses with multiple inherites");
            InheritedInterfaceManager inheritedManager = new InheritedInterfaceManager(this, _document);
            inheritedManager.ValidateMultipleCoClassInherited();

            DoUpdate("Create root folder");
            string solutionFolder = System.IO.Path.Combine(_settings.Folder, solution.Attribute("Name").Value);
            PathApi.ClearCreateFolder(solutionFolder);

            var projects = _document.Descendants("Project");
            foreach (var project in projects)
            {
                if ("true" == project.Attribute("Ignore").Value)
                    continue;

                if(true == _settings.RemoveRefAttribute)
                    ProjectApi.RemoveRefAttribute(project);
 
                DoUpdate("Create project " + project.Attribute("Name").Value);
                string projectFile = RessourceApi.ReadString("Project.Project.csproj");
                string assemblyInfo = RessourceApi.ReadString("Project.AssemblyInfo.cs");
                string constIncludes = ConstantApi.ConvertConstantsToFiles(project, project.Element("Constants"), _settings, solutionFolder);
                string enumIncludes = EnumsApi.ConvertEnumsToFiles(project, project.Element("Enums"), _settings, solutionFolder);
                string faceIncludes = InterfaceApi.ConvertInterfacesToFiles(project, project.Element("Interfaces"), _settings, solutionFolder);
                string dispatchIncludes = DispatchApi.ConvertInterfacesToFiles(project, project.Element("DispatchInterfaces"), _settings, solutionFolder);
                string eventIncludes = EventApi.ConvertInterfacesToFiles(project, project.Element("DispatchInterfaces"), project.Element("Interfaces"), _settings, solutionFolder);
                string typeDefsInclude = AliasApi.ConvertTypeDefsToString(project, project.Element("TypeDefs"));
                string modulesInclude = ModuleApi.ConvertModulesToFiles(project, project.Element("Modules"), _settings, solutionFolder);
                string recordsInclude = RecordsApi.ConvertRecordsToFiles(project, project.Element("Records"), _settings, solutionFolder);

                string toolsInclude = ToolsApi.ConvertToolsToFiles(project, _settings, solutionFolder);

                string classesIncludes = CoClassApi.ConvertCoClassesToFiles(project, project.Element("CoClasses"), _settings, solutionFolder);                
                string factoryInclude = ProjectApi.SaveFactoryFile(solutionFolder, project);

                assemblyInfo = ProjectApi.ReplaceAssemblyAttributes(_settings, solutionFolder, assemblyInfo, project, typeDefsInclude);
                projectFile = ProjectApi.ReplaceProjectAttributes(solutionFolder, projectFile, _settings, project, enumIncludes, constIncludes,
                                        faceIncludes, dispatchIncludes, classesIncludes, eventIncludes, modulesInclude, recordsInclude, toolsInclude, 
                                        factoryInclude);

                ProjectApi.SaveAssemblyInfoFile(solutionFolder, assemblyInfo, project);
                ProjectApi.SaveProjectFile(solutionFolder, projectFile, project);
            }
            
            DoUpdate("Create Solution");
            string solutionFile = RessourceApi.ReadString("Solution.Solution.sln");
            solutionFile = SolutionApi.ReplaceSolutionAttributes(_settings, solutionFile, solution);
            SolutionApi.SaveSolutionFile(_settings, solutionFolder, solutionFile, solution);
                
            SolutionApi.SaveApiProject(_settings, GetProjectApiPath(), solutionFolder);

            if (true == _settings.OpenFolder)
                System.Diagnostics.Process.Start(solutionFolder);
        }
 
        #endregion
    
        #region Static Methods

        internal static string ConvertTypeToConvertCall(string fullTypeName)
        {
            switch (fullTypeName.Trim().ToLower())
            {
                case "bool":
                    return "Boolean";
                case "string":
                    return "String";
                case "byte":
                    return "Byte";
                default:
                    return fullTypeName;
            }
        }

        internal static bool IsDuplicatedReturnValue(XElement returnValue)
        {
            return _dublettes.IsDuplicatedReturnValue(returnValue);
        }

        internal static bool IsDuplicated(string id)
        {
            return _dublettes.IsDuplicated(id);   
        }

        internal static bool IsDerivedReturnValue(XElement returnValue)
        {
            return _derives.IsDerivedReturnValue(returnValue);
        }

        internal static bool IsDerived(string id)
        {
            return _derives.IsDerived(id);
        }

        internal static XElement GetInterfaceOrClassFromKeyAndName(XElement itemParam)
        {
            if (itemParam.Attribute("ProjectKey") == null || itemParam.Attribute("ProjectKey").Value == "")
                return null;

            XElement node = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Elements("Solution").Elements("Projects").Elements("Project")
                             where a.Attribute("Key").Value.Equals(itemParam.Attribute("ProjectKey").Value , StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();

            if (null == node)
                throw new Exception("ProjectKey not found");

            if (itemParam.Attribute("TypeKind").Value == "TKIND_COCLASS")
            {
                XElement targetNode =  (from a in node.Elements("CoClasses").Elements("CoClass")
                        where a.Attribute("Name").Value.Equals(itemParam.Attribute("Type").Value, StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();

                if(null == targetNode)
                    throw new Exception("Class not found");
                return targetNode;
            }
            else if (itemParam.Attribute("TypeKind").Value == "TKIND_DISPATCH")
            {
                XElement targetNode = (from a in node.Elements("DispatchInterfaces").Elements("Interface")
                                       where a.Attribute("Name").Value.Equals(itemParam.Attribute("Type").Value, StringComparison.InvariantCultureIgnoreCase)
                                       select a).FirstOrDefault();

                if (null == targetNode)
                    throw new Exception("Interface not found");
                return targetNode;
            }
            else
            { 
            
            }
            return null;
                               // _x0037_9b19a83-a79f-446c-9b7e-9b595e918e40
        }

        internal static XElement GetInterfaceOrClassFromKey(string key)
        {
            XElement node = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Elements("Solution").Elements("Projects").Elements("Project").Elements("DispatchInterfaces").Elements("Interface")
                             where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();

            if (null != node)
                return node;

            node = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Elements("Solution").Elements("Projects").Elements("Project").Elements("Interfaces").Elements("Interface")
                    where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                    select a).FirstOrDefault();

            if (null != node)
                return node;

            node = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Elements("Solution").Elements("Projects").Elements("Project").Elements("CoClasses").Elements("CoClass")
                    where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                    select a).FirstOrDefault();

            if (null != node)
                return node;
            
            throw new Exception("key not found " + key);
        }
 
        internal static string GetProjectApiPath()
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                string projectPath = Application.StartupPath;
                projectPath = projectPath.Substring(0, projectPath.LastIndexOf("\\"));
                projectPath = projectPath.Substring(0, projectPath.LastIndexOf("\\"));
                projectPath = projectPath.Substring(0, projectPath.LastIndexOf("\\"));

                projectPath = System.IO.Path.Combine(projectPath, "NetOffice");
                if (!System.IO.Directory.Exists(projectPath))
                    throw (new System.IO.DirectoryNotFoundException(projectPath));
                return projectPath;
            }
            else
            {
                string projectPath = System.IO.Path.Combine(Application.StartupPath, "NetOffice");
                if (!System.IO.Directory.Exists(projectPath))
                    throw (new System.IO.DirectoryNotFoundException(projectPath));
                return projectPath;
            }
        }

        internal static string FirstCharLower(string expression)
        {
            if (null == expression)
                return null;

            return expression.Substring(0, 1).ToLower() + expression.Substring(1).ToLower();
        }

        internal static string GetQualifiedType(Settings settings, XElement value)
        {
            if( (true == settings.ConvertOptionalsToObject) || ("4.0" == settings.Framework) )
            {
                if ((null != value.Attribute("IsOptional")) && ("true" == value.Attribute("IsOptional").Value))
                    return "object";
            }

            string type = value.Attribute("Type").Value;
            string space = GetQualifiedNamespace(value);
            return space + type;
        }

        internal static bool IsFromIgnoreProject(XElement node)
        {

            string refProjectKey = node.Attribute("ProjectKey").Value;
            XElement projectNode = (from a in node.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project")
                                    where a.Attribute("Key").Value.Equals(refProjectKey)
                                    select a).FirstOrDefault();

            if (null == projectNode)
                return false;

            if (true == projectNode.Attribute("Ignore").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                return true;
            else
                return false;
        }

        internal static string GetQualifiedType(XElement value)
        {
            string type = value.Attribute("Type").Value;
            if ("COMVariant" == type)
                return "object";

            string space = GetQualifiedNamespace(value);
            return space + type;
        }

        internal static string GetQualifiedNamespace(XElement value)
        {
            XElement parentProject = value;
            while (parentProject.Name != "Project")
                parentProject = parentProject.Parent;

            string type = value.Attribute("Type").Value;

            if (("COMObject" == type) || ("COMVariant" == type) || ("object" == type))
                return "";

            if (true == value.Attribute("IsEnum").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                if (true == value.Attribute("IsExternal").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    string refProjectKey = value.Attribute("ProjectKey").Value;
                    if ("" != refProjectKey)
                    {
                        XElement projectNode = (from a in value.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project")
                                                where a.Attribute("Key").Value.Equals(refProjectKey)
                                                select a).FirstOrDefault();
                        return projectNode.Attribute("Namespace").Value + ".Enums.";
                    }
                }
                else
                {
                    return parentProject.Attribute("Namespace").Value + ".Enums.";
                }
            }
            else if (true == value.Attribute("IsComProxy").Value.Equals("true",StringComparison.InvariantCultureIgnoreCase))
            {
                if (true == value.Attribute("IsExternal").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    string refProjectKey = value.Attribute("ProjectKey").Value;
                    if ("" != refProjectKey)
                    {
                        XElement projectNode = (from a in value.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project")
                                                where a.Attribute("Key").Value.Equals(refProjectKey)
                                                select a).FirstOrDefault();
                        return projectNode.Attribute("Namespace").Value + ".";
                    }
                }
                else
                {
                    return parentProject.Attribute("Namespace").Value + ".";
                }
            }


            return "";
        }

        internal static string TabSpace(int number)
        {
            string tabSpace = "";
            for (int i = 1; i <= number; i++)
                tabSpace += "\t";
            return tabSpace;
        }

        private static void ListAdd(List<string> list, string value)
        {
            bool found = false;

            foreach (string item in list)
            {
                if (item.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                list.Add(value);
        }
        
        /// <summary>
        /// returns support libary versions for entityNode
        /// </summary>
        /// <param name="entityNode"></param>
        /// <returns></returns>
        internal static string GetSupportByVersionAttribute(string[] array, XElement entityNode)
        {
            XElement parentNode = entityNode;
            while (parentNode.Name != "Project")
                parentNode = parentNode.Parent;

            string result = "";
            string versions = "";
            List<string> listVersions = new List<string>();

            foreach (string item in array)
            {
                ListAdd(listVersions, item);
            }

            listVersions.Sort(CompareSupportByVersion);

            foreach (string versionAttribute in listVersions)
                versions += versionAttribute + ",";

            if (versions.Substring(versions.Length - 1) == ",")
                versions = versions.Substring(0, versions.Length - 1);

            result += "[SupportByVersionAttribute(" + "\"" + parentNode.Attribute("Name").Value + "\", " + versions + ")]";
            return result;
        }

        /// <summary>
        /// returns support libary versions for entityNode
        /// </summary>
        /// <param name="entityNode"></param>
        /// <returns></returns>
        internal static string GetSupportByVersionAttribute(XElement entityNode)
        {
            XElement parentNode = entityNode;
            while (parentNode.Name != "Project")
                parentNode = parentNode.Parent;

            string result = "";
            List<string> listVersions = new List<string>();
            string versions = "";

            XElement refLibs = entityNode.Element("RefLibraries");
            foreach (XElement refLib in refLibs.Elements())
            {
                string key = refLib.Attribute("Key").Value;
                XElement libNode = (from a in entityNode.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Libraries").Elements("Library")
                                    where a.Attribute("Key").Value.Equals(key)
                                    select a).FirstOrDefault();
                string versionAttribute = libNode.Attribute("Version").Value;
                ListAdd(listVersions, versionAttribute);
            }

            listVersions.Sort(CompareSupportByVersion);

            foreach (string versionAttribute in listVersions)
                versions += versionAttribute + ",";

            if (versions.Substring(versions.Length - 1) == ",")
                versions = versions.Substring(0, versions.Length - 1);

            result += "[SupportByVersionAttribute(" + "\"" + parentNode.Attribute("Name").Value + "\", " + versions + ")]";
            return result;
        }

        /// <summary>
        /// returns support libary versions for entityNode as array
        /// </summary>
        /// <param name="entityNode"></param>
        /// <returns></returns>
        public static string[] GetSupportByVersionArray(XElement entityNode)
        {
            List<string> result = new List<string>();
            XElement refLibs = entityNode.Element("RefLibraries");
            foreach (XElement refLib in refLibs.Elements())
            {
                string key = refLib.Attribute("Key").Value;
                XElement libNode = (from a in entityNode.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Libraries").Elements("Library")
                                    where a.Attribute("Key").Value.Equals(key)
                                    select a).FirstOrDefault();
                string versionAttribute = libNode.Attribute("Version").Value;
                ListAdd(result, versionAttribute);
            }
            result.Sort(CompareSupportByVersion);
            return result.ToArray();
        }

        /// <summary>
        /// returns support libary versions for entityNode as string
        /// </summary>
        /// <param name="entityNode"></param>
        /// <returns></returns>
        internal static string GetSupportByVersionSummary(string tabSpace, XElement entityNode)
        {
            List<string> resultList = new List<string>();

            XElement parentNode = entityNode;
            while (parentNode.Name != "Project")
                parentNode = parentNode.Parent;

            string summary1 = tabSpace + " /// <summary>\r\n";
            string between  = tabSpace + " /// SupportByVersion " +  parentNode.Attribute("Name").Value + " ";
            string summary2 = tabSpace + " /// </summary>\r\n";

            string[] result = GetSupportByVersionArray(entityNode);
            foreach (string item in result)
                ListAdd(resultList, item);

            resultList.Sort(CompareSupportByVersion);

            foreach (var item in resultList)
                between += item + ", ";

            if (between.EndsWith(", "))
                between = between.Substring(0, between.Length - 2);

            return summary1 + between  + "\r\n" + summary2;
        }

        /// <summary>
        /// returns support libary versions for entityNode as string
        /// </summary>
        /// <param name="entityNode"></param>
        /// <returns></returns>
        internal static string GetSupportByVersion(string tabSpace, XElement entityNode)
        {
            List<string> listVersions = new List<string>();

            XElement parentNode = entityNode;
            while (parentNode.Name != "Project")
                parentNode = parentNode.Parent;

            string res = "";
            string[] result = GetSupportByVersionArray(entityNode);
            foreach (string item in result)
                ListAdd(listVersions, item);

            listVersions.Sort(CompareSupportByVersion);

            foreach (string versionAttribute in listVersions)
                res += versionAttribute + ",";

            if (res.Substring(res.Length - 1) == ",")
                res = res.Substring(0, res.Length - 1);

            return tabSpace + "SupportByVersion " + "" + parentNode.Attribute("Name").Value + ", " + res;
        }

        public static int CompareSupportByVersion(string x, string y)
        {
            double val1 = Convert.ToDouble(x);
            double val2 = Convert.ToDouble(y);

            if (val1 > val2)
                return 1;
            if (val1 < val2)
                return -1;

            return 0;
        }

         

        /// <summary>
        /// returns support libary versions for entityNode as string
        /// </summary>
        /// <param name="entityNode"></param>
        /// <returns></returns>
        internal static string GetSupportByVersionString(string tabSpace, XElement entityNode)
        {
            List<string> listVersions = new List<string>();

            XElement parentNode = entityNode;
            while (parentNode.Name != "Project")
                parentNode = parentNode.Parent;

            string res = "";
            string[] result = GetSupportByVersionArray(entityNode);
            foreach (string item in result)
                ListAdd(listVersions, item);

            listVersions.Sort(CompareSupportByVersion);

            foreach (string versionAttribute in listVersions)
                res += versionAttribute + ",";
             
            if (res.Substring(res.Length - 1) == ",")
                res = res.Substring(0, res.Length - 1);

            return tabSpace + "SupportByVersionAttribute " + "" + parentNode.Attribute("Name").Value + ", " + res;
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

        #endregion
    }
}