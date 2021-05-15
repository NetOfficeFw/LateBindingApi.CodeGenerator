using System;
using System.IO;
using System.Runtime.InteropServices; 
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.ComponentModel;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using TypeLibInformation;
using TLI = TypeLibInformation;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer
{
    public delegate void UpdateHandler(object sender, string message);
    public delegate void FinishHandler(TimeSpan timeElapsed);
    
    public class Analyzer
    {
        #region Fields
        private static readonly Regex XmlInvalidControlChars = new Regex("[\x00-\x1f]", RegexOptions.Compiled);

        private readonly string  _documentVersion = "0.8";

        TLIApplication           _typeLibApplication;
        XDocument                _document;
        XmlSchemaSet             _schemaSet;
        XmlSchema                _schema;

        ThreadJob               _threadJob;
        bool                    _doAsync;
        bool                    _addToCurrentProject;
        string[]                _files;
        TimeSpan                _timeElapsed;

        #endregion

        #region Events

        public event UpdateHandler Update;
        public event FinishHandler Finish;

        #endregion

        #region Properties
        
        public bool IsAlive
        {
            get
            {
                return _threadJob.IsAlive;
            }
        }

        public bool DoAsync
        {
            get
            {
                return _doAsync;
            }
        }

        public XDocument Document
        {
            get 
            {
                return _document;
            }
        }

        public XmlSchema Schema
        {
            get
            {
                return _schema;
            }
        }
      
        #endregion

        #region Construction

        public Analyzer()
        {
            _threadJob = new ThreadJob();
            _threadJob.DoWork += new System.Threading.ThreadStart(threadJob_DoWork);
            _threadJob.RunWorkerCompleted += new ThreadCompletedEventHandler(threadJob_RunWorkerCompleted);
             ResetDocument();
        }

        #endregion

        #region Backgroundworker

        public void Abort()
        {
            _threadJob.Abort();
        }

        private void threadJob_DoWork()
        {
            try
            {
                DateTime startTime = DateTime.Now;
 
                DoUpdate("Prepare");
                _typeLibApplication = new TLIApplication();
                if (false == _addToCurrentProject)
                    ResetDocument();

                DoUpdate("Scan Type Libraries");
                List<TypeLibInfo> types = LoadLibraries(_files);

                DoUpdate("Add Dependencies");
                AddDependencies(types);

                DoUpdate("Load Solution");
                LoadSolution(types);
                
                DoUpdate("Scan Constants");
                LoadConstants(types);
                 
                DoUpdate("Scan Enums");
                LoadEnums(types);

                DoUpdate("Scan Moduls");
                LoadModuls(types);

                DoUpdate("Scan TypeDefs");
                LoadTypeDefs(types);

                DoUpdate("Scan Records");
                LoadRecords(types);

                DoUpdate("Scan Interfaces");
                LoadInterfaces(types, false);

                DoUpdate("Scan Dispatch Interfaces");
                LoadInterfaces(types, true);

                DoUpdate("Scan Inherited Interfaces");
                AddInheritedInterfacesInfo(types, "Interfaces");

                DoUpdate("Scan Inherited DispatchInterfaces");
                AddInheritedInterfacesInfo(types, "DispatchInterfaces");

                DoUpdate("Scan CoClasses");
                LoadCoClasses(types);

                DoUpdate("Scan Event Interfaces");
                AddEventCoClassInfo(types);

                DoUpdate("Scan Default Interfaces");
                AddDefaultCoClassInfo(types);
               
                DoUpdate("Scan Inherited CoClasses");
                AddInheritedCoClassInfo(types);

                DoUpdate("Update Project References");
                UpdateProjectReferences();

                DoUpdate("Update Event Interfaces");
                UpdateEventInterfaces();

                DoUpdate("Validate Type Names");
                ValidateTypeNames();

                DoUpdate("Finsishing operations");
                ReleaseTypeLibrariesList(types);
                Marshal.ReleaseComObject(_typeLibApplication);
                _typeLibApplication = null;

                _timeElapsed = DateTime.Now - startTime;
                DoUpdate("Done");
            }
            catch (Exception throwedException)
            {
                throw (throwedException);
            }
        }

        void threadJob_RunWorkerCompleted()
        {
            if (null != Finish)
                Finish(_timeElapsed);
        }

        /// <summary>
        /// fires update event to client
        /// </summary>
        /// <param name="message"></param>
        private void DoUpdate(string message)
        {
            if (null != Update)
                Update(this, message);
        }

        #endregion
 
        #region LoadSave Methods

        /// <summary>
        /// load type library informations to a LateBindingApi.CodeGenerator.Document
        /// see LateBindingApi.CodeGenerator.Document.xsd
        /// </summary>
        /// <param name="files">typelib paths</param>
        /// <param name="addToCurrentProject">dont clear old project</param>
        public void LoadTypeLibraries(string[] files, bool addToCurrentProject, bool doAsync)
        {
            _doAsync = doAsync;
            _files = files;
            _addToCurrentProject = addToCurrentProject;

            if (true == doAsync)
                _threadJob.Start();
            else
            {
                threadJob_DoWork();
                threadJob_RunWorkerCompleted();
            }
        }

        /// <summary>
        /// load project from xml file
        /// </summary>
        /// <param name="path">Path to a folder with Libraries.xml file.</param>
        public void LoadProject(string path)
        {
            var librariesDir = path;
            var librariesPath = Path.Combine(librariesDir, "Libraries.xml");

            if (!File.Exists(librariesPath))
            {
                throw new ProjectFileFormatException($"Project is missing Libraries.xml file. Project path is '{path}'.");
            }

            var documentElm = new XElement("LateBindingApi.CodeGenerator.Document", new XAttribute("Version", "0.8"));
            var projectsElm = new XElement("Projects");
            var solutionElm = new XElement("Solution", new XAttribute("Name", "NetOffice"), projectsElm);

            var rootDoc = new XDocument(documentElm);
            var librariesDoc = XDocument.Load(librariesPath);
            var projectDirectories = Directory.EnumerateDirectories(librariesDir);

            documentElm.Add(librariesDoc.Root);
            documentElm.Add(solutionElm);

            foreach (var projectPath in projectDirectories)
            {
                var projectDoc = LoadLibraryProject(projectPath);
                projectsElm.Add(projectDoc.Root);
            }

            _document = rootDoc;
            ValidateSchema();
        }

        /// <summary>
        /// load project from xml file
        /// </summary>
        /// <param name="path">Path to a folder with Libraries.xml file.</param>
        public XDocument LoadLibraryProject(string projectPath)
        {
            var projectFile = Path.Combine(projectPath, "Project.xml");

            var xi = (XNamespace)"http://www.w3.org/2001/XInclude";
            var projectDoc = XDocument.Load(projectFile);

            var includes = projectDoc.Descendants(xi + "include").ToList();
            foreach (var include in includes)
            {
                var href = include.Attribute("href").Value;
                var includedFilePath = Path.Combine(projectPath, href);
                var includedDoc = XDocument.Load(includedFilePath);

                include.ReplaceWith(includedDoc.Root);
            }

            return projectDoc;
        }

        public void ScanForOptionals()
        {
            foreach (var item in _document.Descendants("Parameter"))
            {
                if (item.Attribute("ParamFlags").Value.Equals("17", StringComparison.InvariantCultureIgnoreCase))
                {
                    item.Attribute("IsOptional").Value = "true";
                }
            }               
        }

        public void ScanFor15()
        {
            //foreach (var item in _document.Descendants("CoClass"))
            //{
            //    XElement lib = Is15Only(item.Element("RefLibraries").Elements("Ref"));
            //    if(null != lib)
            //        Console.WriteLine(lib.Attribute("Name").Value + "." + item.Attribute("Name").Value);
            //}

            //foreach (var item in _document.Descendants("Interface"))
            //{
            //    XElement lib = Is15Only(item.Element("RefLibraries").Elements("Ref"));
            //    if (null != lib)
            //        Console.WriteLine(lib.Attribute("Name").Value + "." + item.Attribute("Name").Value);
            //}

            //foreach (var item in _document.Descendants("Enum"))
            //{
            //    XElement lib = Is15Only(item.Element("RefLibraries").Elements("Ref"));
            //    if (null != lib)
            //        Console.WriteLine("Enum: " + lib.Attribute("Name").Value + "." + item.Attribute("Name").Value);
            //}

            //foreach (var item in _document.Descendants("Constant"))
            //{
            //    XElement lib = Is15Only(item.Element("RefLibraries").Elements("Ref"));
            //    if (null != lib)
            //        Console.WriteLine("Constant: " + lib.Attribute("Name").Value + "." + item.Attribute("Name").Value);
            //}

            //foreach (var item in _document.Descendants("EnumMember"))
            //{
            //    XElement lib = Is15Only(item.Element("RefLibraries").Elements("Ref"));
            //    if (null != lib)
            //        Console.WriteLine("EnumMember: " + lib.Attribute("Name").Value + "." + item.Attribute("Name").Value);
            //}

            //foreach (var item in _document.Descendants("Member"))
            //{
            //    XElement lib = Is15Only(item.Element("RefLibraries").Elements("Ref"));
            //    if (null != lib)
            //        if(lib.Attribute("Name").Value == "Office")
            //            if(!Is15Only(item.Parent.Parent))
            //                Console.WriteLine("EnumMember: " + lib.Attribute("Name").Value + "." + item.Parent.Parent.Attribute("Name").Value + "." + item.Attribute("Name").Value);
            //}

            //foreach (var item in _document.Descendants("Property"))
            //{
            //    XElement lib = Is15Only(item.Element("RefLibraries").Elements("Ref"));
            //    if (null != lib)
            //        if (lib.Attribute("Name").Value == "Office")
            //            if (!Is15Only(item.Parent.Parent))
            //                Console.WriteLine("Property: " + lib.Attribute("Name").Value + "." + item.Parent.Parent.Attribute("Name").Value + "." + item.Attribute("Name").Value);
            //}

            //foreach (var item in _document.Descendants("Method"))
            //{
            //    XElement lib = Is15Only(item.Element("RefLibraries").Elements("Ref"));
            //    if (null != lib)
            //        if (lib.Attribute("Name").Value == "Office")
            //            if (!Is15Only(item.Parent.Parent))
            //                Console.WriteLine("Method: " + lib.Attribute("Name").Value + "." + item.Parent.Parent.Attribute("Name").Value + "." + item.Attribute("Name").Value);
            //}
            // Constant

        }

        private bool Is15Only(XElement classOrInterface)
        {
            return Is15Only(classOrInterface.Element("RefLibraries").Elements("Ref")) != null;
        }

        private XElement Is15Only(IEnumerable<XElement> refLibs)
        {
            XElement lib = null;
            bool office15Found = false;
            bool office14Found = false;
            foreach (var refItem in refLibs)
            {
                lib = GetLibraryNode(refItem);
                if(lib.Attribute("Version").Value == "15")
                 office15Found = true;
                else if (lib.Attribute("Version").Value == "14")
                    office14Found = true;
            }

            if (office15Found && !office14Found)
                return lib;
            else
                return null;
        }

        private XElement GetLibraryNode(XElement refLib)
        {
            foreach (var item in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Libraries").Elements("Library"))
            {
                if (item.Attribute("Key").Value == refLib.Attribute("Key").Value)
                    return item;
            }
            throw new ArgumentOutOfRangeException("refLib");
        }

        /// <summary>
        /// Save project to a folder.
        /// </summary>
        /// <param name="librariesPath"></param>
        /// <remarks>
        /// Document structure:
        /// 
        /// LateBindingApi.CodeGenerator.Document
        ///   Libraries
        ///   Solution
        ///     Projects
        ///       Project, Name = XXXX
        /// 
        /// </remarks>
        public void SaveProject(string librariesPath)
        {
            var librariesFile = Path.Combine(librariesPath, "Libraries.xml");

            var librariesElm = _document.Root.Element("Libraries");
            librariesElm.Save(librariesFile);

            var allProjects = _document.Root.Element("Solution").Element("Projects").Elements("Project");
            foreach (var projectElm in allProjects)
            {
                SplitProject(projectElm, librariesPath);
            }
        }

        private static void SplitProject(XElement projectElm, string librariesPath)
        {
            var projectName = projectElm.Attribute("Name").Value;
            var projectPath = Path.Combine(librariesPath, projectName);
            var projectFile = Path.Combine(projectPath, "Project.xml");
            EnsureDirectory(projectPath);

            var xi = (XNamespace)"http://www.w3.org/2001/XInclude";
            var dataTypes = projectElm.Elements().ToList();

            foreach (var dataType in dataTypes)
            {
                var typeName = dataType.Name.LocalName;
                var typeFile = Path.Combine(projectPath, $"{typeName}.xml");
                dataType.Save(typeFile);
            }

            if (projectElm.Attribute(XNamespace.Xmlns + "xi") == null)
            {
                projectElm.Add(new XAttribute(XNamespace.Xmlns + "xi", xi.NamespaceName));
            }

            foreach (var dataType in dataTypes)
            {
                var typeName = dataType.Name.LocalName;
                var dataTypeLink = new XElement(xi + "include", new XAttribute("href", $"{typeName}.xml"));
                dataType.ReplaceWith(dataTypeLink);
            }

            projectElm.Save(projectFile);
        }

        private static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        ///  returns library node from ref
        /// </summary>
        /// <param name="refDependLib"></param>
        /// <returns></returns>
        private XElement GetDependLib(XElement refDependLib)
        {
            string name = refDependLib.Attribute("Name").Value;
            string guid = refDependLib.Attribute("GUID").Value;
            string major = refDependLib.Attribute("Major").Value;
            string minor = refDependLib.Attribute("Minor").Value;
            string desc = refDependLib.Attribute("Description").Value;

            XElement dependLibNode = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Libraries").Elements("Library")
                                 where a.Attribute("Name").Value.Equals(name) &&
                                   a.Attribute("GUID").Value.Equals(guid) &&
                                   a.Attribute("Major").Value.Equals(major) &&
                                   a.Attribute("Minor").Value.Equals(minor) &&
                                   a.Attribute("Description").Value.Equals(desc)
                                 select a).FirstOrDefault();

            if ((null == dependLibNode) && ("Office" == refDependLib.Attribute("Name").Value))
            {
                Console.WriteLine("Warning: Attempt Office Hotfix");
                dependLibNode = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Libraries").Elements("Library")
                                 where a.Attribute("Name").Value.Equals(name) &&
                                   a.Attribute("GUID").Value.Equals(guid) &&
                                   a.Attribute("Major").Value.Equals(major) &&
                                   a.Attribute("Minor").Value.Equals(minor)
                                 select a).FirstOrDefault();
            }

            return dependLibNode;
        }

        /// <summary>
        /// returns projects there ref dependLibNode
        /// </summary>
        /// <param name="dependLibNode"></param>
        /// <returns></returns>
        private List<XElement> GetRefProjects(XElement dependLibNode)
        {
            List<XElement> listToReturn = new List<XElement>();

            string dependLibKey = dependLibNode.Attribute("Key").Value;
            IEnumerable<XElement> projects = _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project");
            foreach (XElement project in projects)
	        {
                var refLibNode = (from a in project.Element("RefLibraries").Elements("Ref")
                                  where a.Attribute("Key").Value.Equals(dependLibKey)
                                  select a).FirstOrDefault();

                if (null != refLibNode)
                    listToReturn.Add(project);

	        }

            return listToReturn;
        }

        /// <summary>
        ///  add refProjects as ref to project
        /// </summary>
        /// <param name="project"></param>
        /// <param name="refProjects"></param>
        private void AddProjectReferences(XElement project, List<XElement> refProjects)
        {
            foreach (var item in refProjects)
            {
                string key = item.Attribute("Key").Value;
                var refNode = (from a in project.Element("RefProjects").Elements("RefProject")
                               where a.Attribute("Key").Value.Equals(key)
                               select a).FirstOrDefault();
                if (null == refNode)
                {
                    refNode = new XElement("RefProject",
                        new XAttribute("Key", key));

                    project.Element("RefProjects").Add(refNode);
                }
            }
        }
        
        /// <summary>
        /// validate param type names, change to correct spelling if needed
        /// </summary>
        void ValidateTypeNames()
        { 
            var interfaceMethods = _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Elements("Projects").Elements("Project").Elements("Interfaces").Elements("Interface").Elements("Methods").Elements("Method");
            var dispatchMethods  = _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Elements("Projects").Elements("Project").Elements("DispatchInterfaces").Elements("Interface").Elements("Methods").Elements("Method");
            var interfaceProperties = _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Elements("Projects").Elements("Project").Elements("Interfaces").Elements("Interface").Elements("Properties").Elements("Property");
            var dispatchProperties = _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Elements("Projects").Elements("Project").Elements("DispatchInterfaces").Elements("Interface").Elements("Properties").Elements("Property");

            ValidateTypes(interfaceMethods);
            ValidateTypes(dispatchMethods);
            ValidateTypes(interfaceProperties);
            ValidateTypes(dispatchProperties);
        }
 
        /// <summary>
        /// update project-to-project reference informations
        /// </summary>
        void UpdateProjectReferences()
        {
            foreach(var project in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Elements("Projects").Elements("Project"))
            {
                foreach (XElement refLib in project.Element("RefLibraries").Elements())
                {
                    string refLibKey = refLib.Attribute("Key").Value;
                    var refLibNode = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Libraries").Elements("Library")
                                   where a.Attribute("Key").Value.Equals(refLibKey)
                                    select a).FirstOrDefault();

                    foreach (XElement dependLib in refLibNode.Elements("DependLib"))
                    {
                        XElement dependLibNode = GetDependLib(dependLib);
                        List<XElement> refProjects = GetRefProjects(dependLibNode);
                        AddProjectReferences(project, refProjects); 
                    }
                }
            }
        }

        /// <summary>
        /// load all typelib files
        /// </summary>
        /// <param name="files"></param>
        /// <returns>list of loaded typelib</returns>
        private List<TypeLibInfo> LoadLibraries(string[] files)
        {
            List<TypeLibInfo> listReturn = new List<TypeLibInfo>();

            foreach (string file in files)
            {
                // load typelib and add to list
                TypeLibInfo libInfo = _typeLibApplication.TypeLibInfoFromFile(file);
                listReturn.Add(libInfo);

                // add typelib info to xml document
                AddTypeLibToDocument(libInfo);
            }

            return listReturn;
        }
   
        /// <summary>
        /// scan typelib list and dependency typelibs to the list
        /// </summary>
        /// <param name="list"></param>
        private void AddDependencies(List<TypeLibInfo> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TypeLibInfo libInfo = list[i];
                XElement libNode = GetLibraryNode(libInfo);
                CoClasses classInfos = libInfo.CoClasses;
                foreach (CoClassInfo itemInfo in classInfos)
                {
                    DetectMembersTypesDefinedInExternalLibrary(list, itemInfo.DefaultInterface.Members,libNode);
                    Marshal.ReleaseComObject(itemInfo);
                }
                Marshal.ReleaseComObject(classInfos);

                Interfaces faceInfos = libInfo.Interfaces;
                foreach (InterfaceInfo itemInfo in faceInfos)
                {
                    DetectMembersTypesDefinedInExternalLibrary(list, itemInfo.Members,libNode);
                    Marshal.ReleaseComObject(itemInfo);
                }
                Marshal.ReleaseComObject(faceInfos);
            }
        }

        /// <summary>
        /// add a typelib to document
        /// </summary>
        /// <param name="libInfo"></param>
        private void AddTypeLibToDocument(TypeLibInfo libInfo)
        {
            // check lib not exists in document
            string guid = Utils.EncodeGuid(libInfo.GUID);
            var node = (from a in _document.Descendants("Libraries").Elements("Library")
                        where a.Attribute("GUID").Value.Equals(guid) &&
                              a.Attribute("Name").Value.Equals(libInfo.Name) &&
                              a.Attribute("Description").Value.Equals(TypeDescriptor.GetTypeLibDescription(libInfo)) &&
                              a.Attribute("Major").Value.Equals(libInfo.MajorVersion.ToString()) &&
                              a.Attribute("Minor").Value.Equals(libInfo.MinorVersion.ToString())
                        select a).FirstOrDefault();

            if ((node == null) && ("Office" == libInfo.Name))
            {
                Console.WriteLine("Warning: Attempt Office Hotfix.");

                node = (from a in _document.Descendants("Libraries").Elements("Library")
                        where a.Attribute("GUID").Value.Equals(guid) &&
                              a.Attribute("Name").Value.Equals(libInfo.Name) &&
                              a.Attribute("Major").Value.Equals(libInfo.MajorVersion.ToString()) &&
                              a.Attribute("Minor").Value.Equals(libInfo.MinorVersion.ToString())
                        select a).FirstOrDefault();
            }

            if (null == node)
            {
                node = new XElement("Library",
                            new XAttribute("Name", libInfo.Name),
                            new XAttribute("File", libInfo.ContainingFile),
                            new XAttribute("Key", Utils.NewEncodedGuid()),
                            new XAttribute("GUID", guid),
                            new XAttribute("HelpFile", Utils.RemoveBadChars(libInfo.HelpFile)),
                            new XAttribute("Major", libInfo.MajorVersion.ToString()),
                            new XAttribute("Minor", libInfo.MinorVersion.ToString()),
                            new XAttribute("LCID", libInfo.LCID),
                            new XAttribute("Description", TypeDescriptor.GetTypeLibDescription(libInfo)),
                            new XAttribute("Version", libInfo.Name.Substring(0, 2).ToUpper() + "1"),
                            new XAttribute("SysKind", Convert.ToInt32(libInfo.SysKind)));

                var components = (_document.Descendants("Libraries")).FirstOrDefault();
                components.Add(node);
            }
        }

        /// <summary>
        /// load all constantinfos to document
        /// </summary>
        /// <param name="typelibs"></param>
        private void LoadConstants(List<TypeLibInfo> typelibs)
        {
            foreach (TypeLibInfo item in typelibs)
            {
                var library = GetLibraryNode(item);
                var project = GetProjectNode(item.Name);
                var consts = project.Elements("Constants").FirstOrDefault();

                TLI.Constants constants = item.Constants;
                foreach (TLI.ConstantInfo itemConstant in constants)
                {
                    if (itemConstant.TypeKind == TypeKinds.TKIND_MODULE)
                    {
                        var constantNode = CreateConstantNode(consts, itemConstant);

                        var refComponents = constantNode.Elements("RefLibraries").FirstOrDefault();
                        AddLibraryKeyToRefLibraries(library, refComponents);

                        TLI.Members members = itemConstant.Members;
                        foreach (TLI.MemberInfo itemMember in members)
                        {
                            var membersNode = constantNode.Element("Members");
                            var memberNode = CreateConstantMemberNode(membersNode, itemMember);

                            refComponents = memberNode.Elements("RefLibraries").FirstOrDefault();
                            AddLibraryKeyToRefLibraries(library, refComponents);

                            Marshal.ReleaseComObject(itemMember);
                        }
                        Marshal.ReleaseComObject(members);
                    }
                    Marshal.ReleaseComObject(itemConstant);
                }
                Marshal.ReleaseComObject(constants);
            }
        }

        /// <summary>
        /// load all enuminfos to document
        /// </summary>
        /// <param name="typelibs"></param>
        private void LoadEnums(List<TypeLibInfo> typelibs)
        {
            foreach (TypeLibInfo item in typelibs)
            {
                var library   = GetLibraryNode(item);
                var project   = GetProjectNode(item.Name);
                var enums     = project.Elements("Enums").FirstOrDefault();

                TLI.Constants constants = item.Constants;
                foreach (TLI.ConstantInfo itemConstant in constants)
                {
                    if (itemConstant.TypeKind == TypeKinds.TKIND_ENUM)
                    { 
                        var enumNode = CreateEnumNode(enums, itemConstant);

                        var refComponents = enumNode.Elements("RefLibraries").FirstOrDefault();
                        AddLibraryKeyToRefLibraries(library, refComponents);
                     
                        TLI.Members members = itemConstant.Members;
                        foreach (TLI.MemberInfo itemMember in members)
                        {
                            var membersNode = enumNode.Element("Members");
                            var memberNode = CreateEnumMemberNode(membersNode, itemMember);

                            refComponents = memberNode.Elements("RefLibraries").FirstOrDefault();
                            AddLibraryKeyToRefLibraries(library, refComponents);

                            Marshal.ReleaseComObject(itemMember);
                        }
                        Marshal.ReleaseComObject(members);
                    }
                    Marshal.ReleaseComObject(itemConstant);
                }
                Marshal.ReleaseComObject(constants);
            }
        }

        /// <summary>
        /// load all typelibs in project nodes
        /// </summary>
        /// <param name="typelibs"></param>
        private void LoadSolution(List<TypeLibInfo> typelibs)
        {
            foreach (TypeLibInfo item in typelibs)
            {
                // check project exists
                var node = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Elements("Projects").Elements("Project")
                            where a.Attribute("Name").Value.Equals(item.Name)
                            select a).FirstOrDefault();

                if (null == node)
                {
                    // exclude OLE
                    string preNamespace = "LateBindingApi.";
                    string ignore = "false";
                    if (("{00020430-0000-0000-C000-000000000046}" == item.GUID))
                    {
                        preNamespace = "";
                        ignore = "true";
                    }

                    node = new XElement("Project",
                               new XElement("Constants"),
                               new XElement("Enums"),
                               new XElement("Modules"),
                               new XElement("TypeDefs"),
                               new XElement("Records"),
                               new XElement("CoClasses"),
                               new XElement("DispatchInterfaces"),
                               new XElement("Interfaces"),
                               new XElement("RefLibraries"),
                               new XElement("RefProjects"),
                               new XAttribute("Name",        item.Name),
                               new XAttribute("Namespace", preNamespace + item.Name + "Api"),
                               new XAttribute("Key",         Utils.NewEncodedGuid()),
                               new XAttribute("Description", "NetOffice " + item.Name + " Api"),
                               new XAttribute("Configuration", ""),
                               new XAttribute("Company", "netoffice.codeplex.com"),
                               new XAttribute("Product", "NetOffice"),
                               new XAttribute("Copyright", "Sebastian Lange"),
                               new XAttribute("Trademark", ""),
                               new XAttribute("Culture", ""),
                               new XAttribute("Version", "1.4.0.0"),
                               new XAttribute("FileVersion", "1.4.0.0"),
                               new XAttribute("Ignore", ignore)                               
                               );

                    var projects = _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Elements("Projects").FirstOrDefault();
                    projects.Add(node);
                }
                
                // add library reference
                XElement libNode = GetLibraryNode(item);
                string libKey = libNode.Attribute("Key").Value;
                XElement refLibnode = (from a in node.Element("RefLibraries").Elements("Ref")
                                       where a.Attribute("Key").Value.Equals(libKey)
                                       select a).FirstOrDefault();
                if (null == refLibnode)
                {
                    refLibnode = new XElement("Ref",
                         new XAttribute("Key", libKey));
                    node.Element("RefLibraries").Add(refLibnode);
                }
            }
        }
        
        private void LoadTypeDefs(List<TypeLibInfo> list)
        {
            foreach (TypeLibInfo item in list)
            {
                var library = GetLibraryNode(item);
                var project = GetProjectNode(item.Name);
                var typedefs = project.Elements("TypeDefs").FirstOrDefault();

                TLI.IntrinsicAliases itemTypeDefs = item.IntrinsicAliases;
                foreach (TLI.IntrinsicAliasInfo itemAlias in itemTypeDefs)
                {
                    var modulNode = CreateAliasNode(typedefs, itemAlias);
                    AddDispatchIdToEntityNode(library, modulNode, Utils.EncodeGuid(itemAlias.GUID));

                    var refComponents = modulNode.Elements("RefLibraries").FirstOrDefault();
                    AddLibraryKeyToRefLibraries(library, refComponents);
                      
                    Marshal.ReleaseComObject(itemAlias);
                }
                Marshal.ReleaseComObject(itemTypeDefs);
            }
        }

        private void LoadRecords(List<TypeLibInfo> list)
        {
            foreach (TypeLibInfo item in list)
            {
                var library = GetLibraryNode(item);
                var project = GetProjectNode(item.Name);
                var records = project.Elements("Records").FirstOrDefault();

                TLI.Records itemRecords = item.Records;
                foreach (TLI.RecordInfo itemRecord in itemRecords)
                {
                    var recordNode = CreateRecordNode(records, itemRecord);
                    AddDispatchIdToEntityNode(library, recordNode, Utils.EncodeGuid(itemRecord.GUID));

                    var refComponents = recordNode.Elements("RefLibraries").FirstOrDefault();
                    AddLibraryKeyToRefLibraries(library, refComponents);

                    TLI.Members members = itemRecord.Members;
                    foreach (TLI.MemberInfo itemMember in members)
                    {
                        var membersNode = recordNode.Element("Members");
                        var memberNode = CreateRecordMemberNode(membersNode, itemMember);

                        refComponents = memberNode.Elements("RefLibraries").FirstOrDefault();
                        AddLibraryKeyToRefLibraries(library, refComponents);

                        Marshal.ReleaseComObject(itemMember);
                    }
                    Marshal.ReleaseComObject(members);

                    Marshal.ReleaseComObject(itemRecord);
                }
                Marshal.ReleaseComObject(itemRecords);
            }
        }
 
        private void LoadModuls(List<TypeLibInfo> list)
        {
            foreach (TypeLibInfo item in list)
            {
                var library = GetLibraryNode(item);
                var project = GetProjectNode(item.Name);
                var moduls = project.Elements("Modules").FirstOrDefault();
                TLI.Declarations itemModuls = item.Declarations;
                foreach (TLI.DeclarationInfo itemModul in itemModuls)
                {
                    var modulNode = CreateModulNode(moduls, itemModul);
                    AddDispatchIdToEntityNode(library, modulNode, Utils.EncodeGuid(itemModul.GUID));

                    var refComponents = modulNode.Elements("RefLibraries").FirstOrDefault();
                    AddLibraryKeyToRefLibraries(library, refComponents);

                    List<TLI.MemberInfo> listMembers = TypeDescriptor.GetFilteredMembers(itemModul);
                    foreach (TLI.MemberInfo itemMember in listMembers)
                    {
                        if (true == TypeDescriptor.IsInterfaceMethod(itemMember, item.Name))
                        {
                            var methodNode = MethodHandler.CreateMethodNode(itemMember, modulNode);
                            AddDispatchIdToEntityNode(library, methodNode, itemMember.MemberId.ToString());
                            var refNode = methodNode.Elements("RefLibraries").FirstOrDefault();
                            AddLibraryKeyToRefLibraries(library, refNode);
                            MethodHandler.AddMethod(library, methodNode, itemMember);
                        }
                        Marshal.ReleaseComObject(itemMember);
                    }
                    foreach (TLI.MemberInfo itemFree in listMembers)
                        Marshal.ReleaseComObject(itemFree);

                    Marshal.ReleaseComObject(itemModul);
                }
                Marshal.ReleaseComObject(itemModuls);
            }
        }
        
        /// <summary>
        /// load all interface infos to document
        /// </summary>
        /// <param name="list"></param>
        /// <param name="wantDispatch"></param>
        private void LoadInterfaces(List<TypeLibInfo> list, bool wantDispatch)
        {
            string wantedInterfaces = "Interfaces";
            if (wantDispatch == true)
                wantedInterfaces = "DispatchInterfaces";

            foreach (TypeLibInfo item in list)
            {
                var library = GetLibraryNode(item);
                var project = GetProjectNode(item.Name);
                var faces = project.Elements(wantedInterfaces).FirstOrDefault();

                TLI.Interfaces interfaces = item.Interfaces;
                foreach (TLI.InterfaceInfo itemInterface in interfaces)
                {
                    if (itemInterface.Name == "IAccessible")
                    {
                        // as ITypeInfo                     
                    }

                    if (true == TypeDescriptor.IsTargetInterfaceType(itemInterface.TypeKind, wantDispatch))
                    {
                        var faceNode = CreateInterfaceNode(faces, itemInterface);

                        List<TLI.MemberInfo> listMembers = TypeDescriptor.GetFilteredMembers(itemInterface);
                        foreach (TLI.MemberInfo itemMember in listMembers)
                        {
                            if (true == TypeDescriptor.IsInterfaceMethod(itemMember, item.Name))
                            {
                   

                                var methodNode = MethodHandler.CreateMethodNode(itemMember, faceNode);
                                AddDispatchIdToEntityNode(library, methodNode, itemMember.MemberId.ToString());
                                var refNode = methodNode.Elements("RefLibraries").FirstOrDefault();
                                AddLibraryKeyToRefLibraries(library, refNode);
                                MethodHandler.AddMethod(library, methodNode, itemMember);
                            }
                            else if (true == TypeDescriptor.IsInterfaceProperty(itemMember))
                            {
                                if (itemMember.Name.Equals("accChild", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    // ITypeInfo tInfo = itemInterface.VTableInterface.ITypeInfo as ITypeInfo;
                                }

                                var propertyNode = PropertyHandler.CreatePropertyNode(itemMember, faceNode);
                                AddDispatchIdToEntityNode(library, propertyNode, itemMember.MemberId.ToString());
                                var refNode = propertyNode.Elements("RefLibraries").FirstOrDefault();
                                AddLibraryKeyToRefLibraries(library, refNode);
                                PropertyHandler.AddProperty(library, propertyNode, itemMember);
                            }

                            Marshal.ReleaseComObject(itemMember);
                        }
                       
                        AddDispatchIdToEntityNode(library, faceNode, Utils.EncodeGuid(itemInterface.GUID));  

                        var refComponents = faceNode.Elements("RefLibraries").FirstOrDefault();
                        AddLibraryKeyToRefLibraries(library, refComponents);
                    }
                    Marshal.ReleaseComObject(itemInterface);
                }
                Marshal.ReleaseComObject(interfaces);
            }

        }
        
        /// <summary>
        /// load all coclass infos to document
        /// </summary>
        /// <param name="list"></param>
        private void LoadCoClasses(List<TypeLibInfo> list)
        {
            foreach (TypeLibInfo item in list)
            {
                var library = GetLibraryNode(item);
                var project = GetProjectNode(item.Name);
                var classes = project.Elements("CoClasses").FirstOrDefault();

                TLI.CoClasses itemClasses = item.CoClasses;
                foreach (TLI.CoClassInfo itemClass in itemClasses)
                {
                    var classNode = CreateCoClassNode(classes, itemClass);
                    AddDispatchIdToEntityNode(library, classNode, Utils.EncodeGuid(itemClass.GUID));
                    
                    var refComponents = classNode.Elements("RefLibraries").FirstOrDefault();
                    AddLibraryKeyToRefLibraries(library, refComponents);

                    Marshal.ReleaseComObject(itemClass);
                }
                Marshal.ReleaseComObject(itemClasses);
            }
        }
        
        /// <summary>
        /// validate param type names, change to correct spelling if needed
        /// </summary>
        /// <param name="entities"></param>
        private void ValidateTypes(IEnumerable<XElement> entities)
        {
            var pars = entities.Elements("Parameters");
            foreach (var item in pars.Elements("Parameter"))
            {
                if ("" != item.Attribute("TypeKey").Value)
                    item.Attribute("Type").Value = GetNodeByKey(item.Attribute("TypeKey")).Attribute("Name").Value;

                if ("GUID" == item.Attribute("Type").Value)
                    item.Attribute("Type").Value = "Guid";
            }

            foreach (var item in pars)
            {
                XElement returnValue = item.Element("ReturnValue");
                if ("void" != returnValue.Attribute("Type").Value)
                {
                    if ("" != returnValue.Attribute("TypeKey").Value)
                        returnValue.Attribute("Type").Value = GetNodeByKey(returnValue.Attribute("TypeKey")).Attribute("Name").Value;

                    if ("GUID" == returnValue.Attribute("Type").Value)
                        returnValue.Attribute("Type").Value = "Guid";
                }
            }

        }

        /// <summary>
        /// get an element by key attribute
        /// </summary>
        /// <param name="attKey"></param>
        /// <returns></returns>
        private XElement GetNodeByKey(XAttribute attKey)
        {
            var projects = attKey.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project");
            foreach (var project in projects)
            {
                var face = (from a in project.Element("Interfaces").Elements("Interface")
                            where a.Attribute("Key").Value.Equals(attKey.Value)
                            select a).FirstOrDefault();
                if (null != face)
                    return face;

                face = (from a in project.Element("DispatchInterfaces").Elements("Interface")
                        where a.Attribute("Key").Value.Equals(attKey.Value)
                        select a).FirstOrDefault();
                if (null != face)
                    return face;

                face = (from a in project.Element("Enums").Elements("Enum")
                        where a.Attribute("Key").Value.Equals(attKey.Value)
                        select a).FirstOrDefault();
                if (null != face)
                    return face;


                face = (from a in project.Element("CoClasses").Elements("CoClass")
                        where a.Attribute("Key").Value.Equals(attKey.Value)
                        select a).FirstOrDefault();
                if (null != face)
                    return face;

                face = (from a in project.Element("Records").Elements("Record")
                        where a.Attribute("Key").Value.Equals(attKey.Value)
                        select a).FirstOrDefault();
                if (null != face)
                    return face;

            }
            throw (new ArgumentOutOfRangeException("key not found"));
        }

        /// <summary>
        /// Get component node from libInfo
        /// </summary>
        /// <param name="libInfo"></param>
        /// <returns></returns>
        private XElement GetLibraryNode(TypeLibInfo libInfo)
        {
            string guid = Utils.EncodeGuid(libInfo.GUID);
            var library = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Elements("Libraries").Elements("Library")
                             where a.Attribute("GUID").Value.Equals(guid) &&
                                   a.Attribute("Name").Value.Equals(libInfo.Name) &&
                                   a.Attribute("Description").Value.Equals(TypeDescriptor.GetTypeLibDescription(libInfo)) &&
                                   a.Attribute("Major").Value.Equals(libInfo.MajorVersion.ToString()) &&
                                   a.Attribute("Minor").Value.Equals(libInfo.MinorVersion.ToString())
                             select a).FirstOrDefault();

            if ((null == library) && ("Office" == libInfo.Name))
            {
                Console.WriteLine("Warning: Attempt Office Hotfix");
                library = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Elements("Libraries").Elements("Library")
                           where a.Attribute("GUID").Value.Equals(guid) &&
                                 a.Attribute("Name").Value.Equals(libInfo.Name) &&
                                 a.Attribute("Major").Value.Equals(libInfo.MajorVersion.ToString()) &&
                                 a.Attribute("Minor").Value.Equals(libInfo.MinorVersion.ToString())
                           select a).FirstOrDefault();
            }


            return library;
        }

        /// <summary>
        /// get project node by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private XElement GetProjectNode(string name)
        {
            var project = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Elements("Projects").Elements("Project")
                           where a.Attribute("Name").Value.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                           select a).FirstOrDefault();
            return project;
        }
        
        /// <summary>
        /// returs existing class node for itemClass
        /// </summary>
        /// <param name="libInfo"></param>
        /// <param name="itemInterface"></param>
        /// <returns></returns>
        private XElement GetClassNode(TypeLibInfo libInfo, TLI.CoClassInfo itemClass)
        {
            var project = GetProjectNode(libInfo.Name);
            string name = itemClass.Name;

            var face = (from a in project.Element("CoClasses").Elements("CoClass")
                        where a.Attribute("Name").Value.Equals(name)
                        select a).FirstOrDefault();


            return face;
        }

        /// <summary>
        /// returs existing interface node for libInfo->itemInterface
        /// </summary>
        /// <param name="libInfo"></param>
        /// <param name="itemInterface"></param>
        /// <returns></returns>
        private XElement GetInterfaceNode(TypeLibInfo libInfo, TLI.InterfaceInfo itemInterface)
        {
            var project = GetProjectNode(libInfo.Name);
            string name = itemInterface.Name;

            var face = (from a in project.Element("Interfaces").Elements("Interface")
                        where a.Attribute("Name").Value.Equals(name) 
                        select a).FirstOrDefault();

            if(null == face)
                face = (from a in project.Element("DispatchInterfaces").Elements("Interface")
                        where a.Attribute("Name").Value.Equals(name)
                        select a).FirstOrDefault();

            return face;
        }
        
        /// <summary>
        /// returs existing interface node for key
        /// </summary>
        /// <param name="libInfo"></param>
        /// <param name="itemInterface"></param>
        /// <returns></returns>
        private XElement GetInterfaceNode(XElement projectNode, string key)
        {
            var face = (from a in projectNode.Element("Interfaces").Elements("Interface")
                        where a.Attribute("Key").Value.Equals(key)
                        select a).FirstOrDefault();

            if (null == face)
                face = (from a in projectNode.Element("DispatchInterfaces").Elements("Interface")
                        where a.Attribute("Key").Value.Equals(key)
                        select a).FirstOrDefault();

            return face;
        }

        /// <summary>
        /// returs existing interface node for inheritedInterface
        /// </summary>
        /// <param name="inheritedInterface"></param>
        /// <returns></returns>
        private XElement GetInterfaceNodeFromInheritedInfo(TLI.InterfaceInfo inheritedInterface)
        {
            string name = inheritedInterface.Name;
            string projectName = inheritedInterface.Parent.Name;
            var project = GetProjectNode(projectName);

            var face = (from a in project.Element("Interfaces").Elements("Interface")
                        where a.Attribute("Name").Value.Equals(name)
                        select a).FirstOrDefault();

            if (null == face)
                face = (from a in project.Element("DispatchInterfaces").Elements("Interface")
                        where a.Attribute("Name").Value.Equals(name)
                        select a).FirstOrDefault();

            return face;
        }

        /// <summary>
        /// create new interface node or get existing
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="itemInterface"></param>
        /// <returns></returns>
        private XElement CreateInterfaceNode(XElement faces, TLI.InterfaceInfo itemInterface)
        {
            // check interface exists
            var faceNode = (from a in faces.Elements()
                            where a.Attribute("Name").Value.Equals(itemInterface.Name, StringComparison.InvariantCultureIgnoreCase)
                            select a).FirstOrDefault();

            // look for a mirror interface
            if (null == faceNode)
            {
                if (itemInterface.TypeKind == TypeKinds.TKIND_DISPATCH)
                {
                    XElement otherFaces = faces.Parent.Element("Interfaces");
                    var otherFaceNode = (from a in otherFaces.Elements()
                                    where a.Attribute("Name").Value.Equals(itemInterface.Name, StringComparison.InvariantCultureIgnoreCase)
                                    select a).FirstOrDefault();
                    faceNode = otherFaceNode;
                }
                else if (itemInterface.TypeKind == TypeKinds.TKIND_INTERFACE)
                {
                    XElement otherFaces = faces.Parent.Element("DispatchInterfaces");
                    var otherFaceNode = (from a in otherFaces.Elements()
                                         where a.Attribute("Name").Value.Equals(itemInterface.Name, StringComparison.InvariantCultureIgnoreCase)
                                         select a).FirstOrDefault();
                    faceNode = otherFaceNode;
                }
            }

            if (null == faceNode)
            {
                faceNode = new XElement("Interface",
                               new XElement("Methods"),
                               new XElement("Properties"),
                               new XElement("DispIds"),
                               new XElement("Inherited"),
                               new XElement("RefLibraries"),
                               new XAttribute("IsEventInterface", "false"),
                               new XAttribute("IsEarlyBind", "false"),
                               new XAttribute("IsHidden", TypeDescriptor.IsHidden(itemInterface)),
                               new XAttribute("TypeLibType", itemInterface.AttributeMask),
                               new XAttribute("Name", itemInterface.Name),
                               new XAttribute("Key",  Utils.NewEncodedGuid()));

                faces.Add(faceNode);
            }

            return faceNode;
        }

        /// <summary>
        /// create new record node or get existing
        /// </summary>
        /// <param name="classes"></param>
        /// <param name="itemClass"></param>
        /// <returns></returns>
        private XElement CreateRecordNode(XElement records, TLI.RecordInfo itemRecord)
        {
            // check class exists
            var aliasNode = (from a in records.Elements()
                             where a.Attribute("Name").Value.Equals(itemRecord.Name, StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();

            if (null == aliasNode)
            {
                aliasNode = new XElement("Record",
                               new XElement("RefLibraries"),
                               new XElement("Members"),
                               new XElement("DispIds"),
                               new XAttribute("Name", itemRecord.Name),
                               new XAttribute("GUID", Utils.EncodeGuid(itemRecord.GUID)),
                               new XAttribute("TypeLibType", itemRecord.AttributeMask.ToString()),
                               new XAttribute("Key", Utils.NewEncodedGuid()));
                
                records.Add(aliasNode);
            }

            return aliasNode;
        }

        /// <summary>
        /// create new alias node or get existing
        /// </summary>
        /// <param name="classes"></param>
        /// <param name="itemClass"></param>
        /// <returns></returns>
        private XElement CreateAliasNode(XElement typedefs, TLI.IntrinsicAliasInfo itemAlias)
        {
            // check class exists
            var aliasNode = (from a in typedefs.Elements()
                             where a.Attribute("Name").Value.Equals(itemAlias.Name, StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();

            if (null == aliasNode)
            {
                aliasNode = new XElement("Alias",
                               new XElement("RefLibraries"),
                               new XElement("DispIds"),
                               new XAttribute("Name", itemAlias.Name),
                               new XAttribute("Intrinsic", TypeDescriptor.FormattedType(itemAlias.ResolvedType, false)),
                               new XAttribute("Key", Utils.NewEncodedGuid()));
                
                typedefs.Add(aliasNode);
            }

            return aliasNode;
        }

        /// <summary>
        /// create new module node or get existing
        /// </summary>
        /// <param name="classes"></param>
        /// <param name="itemClass"></param>
        /// <returns></returns>
        private XElement CreateModulNode(XElement moduls, TLI.DeclarationInfo itemModul)
        {
            // check class exists
            var modulNode = (from a in moduls.Elements()
                             where a.Attribute("Name").Value.Equals(itemModul.Name, StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();

            if (null == modulNode)
            {
                modulNode = new XElement("Module",
                               new XElement("Methods"),
                               new XElement("RefLibraries"),
                               new XElement("DispIds"),
                               new XAttribute("Name", itemModul.Name),
                               new XAttribute("Key", Utils.NewEncodedGuid()));

                moduls.Add(modulNode);
            }

            return modulNode;
        }

        /// <summary>
        /// create new class node or get existing
        /// </summary>
        /// <param name="classes"></param>
        /// <param name="itemClass"></param>
        /// <returns></returns>
        private XElement CreateCoClassNode(XElement classes, TLI.CoClassInfo itemClass)
        {
            // check class exists
            var classNode = (from a in classes.Elements()
                             where a.Attribute("Name").Value.Equals(itemClass.Name, StringComparison.InvariantCultureIgnoreCase)
                            select a).FirstOrDefault();

            if (null == classNode)
            {
                
                classNode = new XElement("CoClass",
                               new XElement("RefLibraries"),
                               new XElement("DefaultInterfaces"),
                               new XElement("EventInterfaces"),
                               new XElement("Inherited"),
                               new XElement("DispIds"),
                               new XAttribute("Name", itemClass.Name),
                               new XAttribute("AutomaticQuit", "false"),
                               new XAttribute("IsAppObject", TypeDescriptor.IsAppObject(itemClass)),
                               new XAttribute("Key", Utils.NewEncodedGuid()));

                classes.Add(classNode);
            }

            return classNode;
        }
 
        /// <summary>
        /// add dispatch id to method or property node
        /// </summary>
        /// <param name="library"></param>
        /// <param name="node"></param>
        /// <param name="id"></param>
        private void AddDispatchIdToEntityNode(XElement library, XElement node, string id)
        {
            var dispatchIdsNode = node.Elements("DispIds").FirstOrDefault();
            var idNode = (from a in dispatchIdsNode.Elements("DispId")
                        where a.Attribute("Id").Value.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();

            if (null == idNode)
            {
                idNode = new XElement("DispId",
                               new XElement("RefLibraries"),
                               new XAttribute("Id", id));

                dispatchIdsNode.Add(idNode);
            }

            string key = library.Attribute("Key").Value;
            var refLibsNode = idNode.Elements("RefLibraries").FirstOrDefault();
            var refLib = (from a in refLibsNode.Elements("Ref")
                          where a.Attribute("Key").Value.Equals(key)
                          select a).FirstOrDefault();

            if (null == refLib)
            {
                refLib = new XElement("Ref",
                              new XAttribute("Key", key));

                refLibsNode.Add(refLib);
            }
        }

        /// <summary>
        /// create new constant node or get existing
        /// </summary>
        /// <param name="enums"></param>
        /// <param name="itemConstant"></param>
        /// <returns></returns>
        private XElement CreateConstantNode(XElement constants, TLI.ConstantInfo itemConstant)
        {
            // check const exists
            var constNode = (from a in constants.Elements()
                            where a.Attribute("Name").Value.Equals(itemConstant.Name, StringComparison.InvariantCultureIgnoreCase)
                            select a).FirstOrDefault();

            if (null == constNode)
            {
                constNode = new XElement("Constant",
                               new XElement("Members"),
                               new XElement("RefLibraries"),
                               new XAttribute("Name", itemConstant.Name),
                               new XAttribute("Key", Utils.NewEncodedGuid()));

                constants.Add(constNode);
            }

            return constNode;
        }

        /// <summary>
        /// create new enum node or get existing
        /// </summary>
        /// <param name="enums"></param>
        /// <param name="itemConstant"></param>
        /// <returns></returns>
        private XElement CreateEnumNode(XElement enums, TLI.ConstantInfo itemConstant)
        {
            // check enum exists
            var enumNode = (from a in enums.Elements()
                            where a.Attribute("Name").Value.Equals(itemConstant.Name, StringComparison.InvariantCultureIgnoreCase)
                            select a).FirstOrDefault();

            if (null == enumNode)
            {
                enumNode = new XElement("Enum",
                               new XElement("Members"),
                               new XElement("RefLibraries"),
                               new XAttribute("Name", itemConstant.Name),
                               new XAttribute("Key", Utils.NewEncodedGuid()));

                enums.Add(enumNode);
            }

            return enumNode;
        }

        /// <summary>
        /// create new constant member node or get existing
        /// </summary>
        /// <param name="membersNode"></param>
        /// <param name="itemMember"></param>
        /// <returns></returns>
        private XElement CreateConstantMemberNode(XElement membersNode, TLI.MemberInfo itemMember)
        {
            // check enum member exists
            var memberNode = (from a in membersNode.Elements()
                              where a.Attribute("Name").Value.Equals(itemMember.Name, StringComparison.InvariantCultureIgnoreCase)
                              select a).FirstOrDefault();

            if (null == memberNode)
            {
                var itemName = itemMember.Name;
                var itemValue = itemMember.Value;
                var itemStringValue = itemValue as string;
                if (itemStringValue != null)
                {
                    itemValue = FixXmlInvalidChars(itemStringValue);
                }

                memberNode = new XElement("Member",
                                new XElement("RefLibraries"),
                                new XAttribute("Name", itemName),
                                new XAttribute("Type", TypeDescriptor.FormattedType(itemMember.ReturnType,false)),
                                new XAttribute("Value", itemValue));
                 
                membersNode.Add(memberNode);
            }

            return memberNode;
        }

        private string FixXmlInvalidChars(string itemStringValue)
        {
            return XmlInvalidControlChars.Replace(itemStringValue, InvalidXmlCharsReplacer);
        }

        private static string InvalidXmlCharsReplacer(Match match)
        {
            var value = match.Value[0];
            switch (value)
            {
                case '\t':
                case '\n':
                case '\r':
                    return match.Value;
                default:
                    return "&#" + ((int)value).ToString("X4") + ";";
            }
        }

        /// <summary>
        /// create new record member node or get existing
        /// </summary>
        /// <param name="membersNode"></param>
        /// <param name="itemMember"></param>
        /// <returns></returns>
        private XElement CreateRecordMemberNode(XElement membersNode, TLI.MemberInfo itemMember)
        {
            // check enum member exists
            var memberNode = (from a in membersNode.Elements()
                              where a.Attribute("Name").Value.Equals(itemMember.Name, StringComparison.InvariantCultureIgnoreCase)
                              select a).FirstOrDefault();

            if (null == memberNode)
            {
                VarTypeInfo returnTypeInfo = itemMember.ReturnType;
                string returnTypeName = TypeDescriptor.FormattedEnumMemberType(returnTypeInfo);
                memberNode = new XElement("Member",
                                new XElement("RefLibraries"),
                                new XAttribute("Name", itemMember.Name),
                                new XAttribute("Type", returnTypeName),
                                new XAttribute("TypeKind", MethodHandler.TypeInfo(itemMember.ReturnType.TypeInfo)),
                                new XAttribute("IsComProxy", TypeDescriptor.IsCOMProxy(itemMember.ReturnType).ToString().ToLower()),
                                new XAttribute("IsEnum", TypeDescriptor.IsEnum(itemMember.ReturnType).ToString().ToLower()),
                                new XAttribute("IsExternal", TypeDescriptor.IsExternal(itemMember.ReturnType).ToString().ToLower()),
                                new XAttribute("IsArray", TypeDescriptor.IsArray(itemMember.ReturnType).ToString().ToLower()),
                                new XAttribute("IsNative", TypeDescriptor.IsNative(returnTypeName).ToString().ToLower()),
                                new XAttribute("MarshalAs", TypeDescriptor.MarshalMemberAsAs(itemMember)),
                                new XAttribute("TypeKey", TypeDescriptor.GetTypeKey(membersNode.Document, itemMember.ReturnType)),
                                new XAttribute("ProjectKey", TypeDescriptor.GetProjectKey(membersNode.Document, itemMember.ReturnType)),
                                new XAttribute("LibraryKey", TypeDescriptor.GetLibraryKey(membersNode.Document, itemMember.ReturnType))
                                );
                  
                membersNode.Add(memberNode);
            }

            return memberNode;
        }

        /// <summary>
        /// create new enum member node or get existing
        /// </summary>
        /// <param name="membersNode"></param>
        /// <param name="itemMember"></param>
        /// <returns></returns>
        private XElement CreateEnumMemberNode(XElement membersNode, TLI.MemberInfo itemMember)
        {
            // check enum member exists
            var memberNode = (from a in membersNode.Elements()
                              where a.Attribute("Name").Value.Equals(itemMember.Name, StringComparison.InvariantCultureIgnoreCase)
                              select a).FirstOrDefault();

            if (null == memberNode)
            {
                memberNode = new XElement("Member",
                                new XElement("RefLibraries"),
                                new XAttribute("Name", itemMember.Name),
                                new XAttribute("Value", itemMember.Value));

                membersNode.Add(memberNode);
            }

            return memberNode;
        }

        /// <summary>
        /// add component key to refs node
        /// </summary>
        /// <param name="component"></param>
        /// <param name="refNode"></param>
        private void AddLibraryKeyToRefLibraries(XElement component, XElement refsNode)
        {
            string key = component.Attributes("Key").FirstOrDefault().Value;
            
            // check refComponent exists 
            var comp = (from a in refsNode.Elements("Ref")
                        where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();

            if (null == comp)
            {
                comp = new XElement("Ref",
                            new XAttribute("Key", key));

                refsNode.Add(comp);
            }
        }

        /// <summary>
        /// release all libs in list and clear
        /// </summary>
        /// <param name="list"></param>
        private void ReleaseTypeLibrariesList(List<TypeLibInfo> list)
        {
            foreach (TypeLibInfo item in list)
                Marshal.ReleaseComObject(item);

            list.Clear();
        }

        /// <summary>
        /// check all member types and looks type comes from external library
        /// in case of true, the method add the external library to typelibrary list and xml document
        /// </summary>
        /// <param name="list"></param>
        /// <param name="members"></param>
        private void DetectMembersTypesDefinedInExternalLibrary(List<TypeLibInfo> list, Members members, XElement libNode)
        {
            bool found = false;
            string containingFile = "";
            
            foreach (MemberInfo itemMember in members)
            {
                VarTypeInfo typeInfo = itemMember.ReturnType;             
                if ((typeInfo != null) && (true == TypeDescriptor.IsExternal(typeInfo)))
                {
                    found = false;
                    containingFile = typeInfo.TypeLibInfoExternal.ContainingFile;
                    foreach (TypeLibInfo itemLib in list)
                    {
                        if (itemLib.ContainingFile == containingFile)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (false == found)
                    {     
                        TypeLibInfo libInfo = _typeLibApplication.TypeLibInfoFromFile(containingFile);
                        list.Add(libInfo);
                        AddTypeLibToDocument(libInfo);
                        
                    }
                   
                    foreach (TypeLibInfo itemLib in list)
                    {
                        if (itemLib.ContainingFile == containingFile)
                        {
                            XElement dependLibNode = (from a in libNode.Elements("DependLib")
                                                      where a.Attribute("Name").Value.Equals(itemLib.Name, StringComparison.InvariantCultureIgnoreCase) &&
                                                      a.Attribute("Description").Value.Equals(TypeDescriptor.GetTypeLibDescription(itemLib), StringComparison.InvariantCultureIgnoreCase)
                                                      select a).FirstOrDefault();

                            if (null == dependLibNode)
                            {
                                dependLibNode = new XElement("DependLib",
                                           new XAttribute("Name", itemLib.Name),
                                           new XAttribute("GUID", Utils.EncodeGuid(itemLib.GUID)),
                                           new XAttribute("Major", itemLib.MajorVersion.ToString()),
                                           new XAttribute("Minor", itemLib.MinorVersion.ToString()),
                                           new XAttribute("Description", TypeDescriptor.GetTypeLibDescription(itemLib)));

                                libNode.Add(dependLibNode);
                            }
                        }
                    }
                }
                
                Parameters memberParams = itemMember.Parameters;
                foreach (ParameterInfo itemParam in memberParams)
                {
                    VarTypeInfo paramTypeInfo = itemParam.VarTypeInfo;
                    if ((paramTypeInfo != null) && (true == TypeDescriptor.IsExternal(paramTypeInfo)))
                    {
                         found = false;
                         containingFile = paramTypeInfo.TypeLibInfoExternal.ContainingFile;
                         foreach (TypeLibInfo itemLib in list)
                         {
                            
                             if (itemLib.ContainingFile == containingFile)
                             {
                                 found = true;
                                 break;
                             }
                         }

                         if (false == found)
                         {
                            TypeLibInfo libInfo = _typeLibApplication.TypeLibInfoFromFile(containingFile);
                            list.Add(libInfo);
                            AddTypeLibToDocument(libInfo);
                           
                         }

                         foreach (TypeLibInfo itemLib in list)
                         {
                             if (itemLib.ContainingFile == containingFile)
                             {
                                 XElement dependLibNode = (from a in libNode.Elements("DependLib")
                                                           where a.Attribute("Name").Value.Equals(itemLib.Name, StringComparison.InvariantCultureIgnoreCase) &&
                                                           a.Attribute("Description").Value.Equals(TypeDescriptor.GetTypeLibDescription(itemLib), StringComparison.InvariantCultureIgnoreCase)
                                                           select a).FirstOrDefault();

                                 if (null == dependLibNode)
                                 {
                                     dependLibNode = new XElement("DependLib",
                                                new XAttribute("Name", itemLib.Name),
                                                new XAttribute("GUID", Utils.EncodeGuid(itemLib.GUID)),
                                                new XAttribute("Major", itemLib.MajorVersion.ToString()),
                                                new XAttribute("Minor", itemLib.MinorVersion.ToString()),
                                                new XAttribute("Description", TypeDescriptor.GetTypeLibDescription(itemLib)));

                                     libNode.Add(dependLibNode);
                                 }
                             }
                         }

                    }

                    Marshal.ReleaseComObject(paramTypeInfo);
                    Marshal.ReleaseComObject(itemParam);
                }
                Marshal.ReleaseComObject(memberParams);

                if (null != typeInfo)
                    Marshal.ReleaseComObject(typeInfo);
            }
        }

        /// <summary>
        /// add component key to refs node
        /// </summary>
        /// <param name="component"></param>
        /// <param name="refNode"></param>
        private void AddInterfaceKeyToEvent(XElement faceNode, string key, XElement libraryNode)
        {
            // check Interface exists 
            XElement inheritedsNode = faceNode.Element("EventInterfaces");
            var node = (from a in inheritedsNode.Elements("Ref")
                        where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();

            if (null == node)
            {
                node = new XElement("Ref",
                            new XElement("RefLibraries", ""),
                            new XAttribute("Key", key));

                inheritedsNode.Add(node);
            }

            string libKey = libraryNode.Attribute("Key").Value;

            XElement refsNode = node.Element("RefLibraries");
            var libRefNode = (from a in refsNode.Elements("Ref")
                              where a.Attribute("Key").Value.Equals(libKey, StringComparison.InvariantCultureIgnoreCase)
                              select a).FirstOrDefault();

            if (null == libRefNode)
            {
                libRefNode = new XElement("Ref",
                    new XAttribute("Key", libKey));

                refsNode.Add(libRefNode);
            }
        }

        /// <summary>
        /// add component key to refs node
        /// </summary>
        /// <param name="component"></param>
        /// <param name="refNode"></param>
        private void AddInterfaceKeyToDefault(XElement faceNode, string key, XElement libraryNode)
        {
            // check Interface exists 
            XElement inheritedsNode = faceNode.Element("DefaultInterfaces");
            var node = (from a in inheritedsNode.Elements("Ref")
                        where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();

            if (null == node)
            {
                node = new XElement("Ref",
                            new XElement("RefLibraries", ""),
                            new XAttribute("Key", key));

                inheritedsNode.Add(node);
            }

            string libKey = libraryNode.Attribute("Key").Value;

            XElement refsNode = node.Element("RefLibraries");
            var libRefNode = (from a in refsNode.Elements("Ref")
                              where a.Attribute("Key").Value.Equals(libKey, StringComparison.InvariantCultureIgnoreCase)
                              select a).FirstOrDefault();

            if (null == libRefNode)
            {
                libRefNode = new XElement("Ref",
                    new XAttribute("Key", libKey));

                refsNode.Add(libRefNode);
            }
        }

        /// <summary>
        /// add component key to refs node
        /// </summary>
        /// <param name="component"></param>
        /// <param name="refNode"></param>
        private void AddInterfaceKeyToInherited(XElement faceNode, string key, XElement libraryNode)
        {
            // check Interface exists 
            XElement inheritedsNode = faceNode.Element("Inherited");
            var node = (from a in inheritedsNode.Elements("Ref")
                        where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();

            if (null == node)
            {
                node = new XElement("Ref",
                            new XElement("RefLibraries", ""),
                            new XAttribute("Key", key));

                inheritedsNode.Add(node);
            }

            string libKey = libraryNode.Attribute("Key").Value;
 
            XElement refsNode = node.Element("RefLibraries");
            var libRefNode = (from a in refsNode.Elements("Ref")
                        where a.Attribute("Key").Value.Equals(libKey, StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();

            if (null == libRefNode)
            {
                libRefNode = new XElement("Ref",
                    new XAttribute("Key", libKey));

                refsNode.Add(libRefNode);
            }
        }

        /// <summary>
        /// scan all interfaces in typelibs and info to document about inherited interfaces
        /// all interfaces must be listed in document before call this method
        /// </summary>
        /// <param name="list"></param>
        /// <param name="wantDispatch"></param>
        private void AddInheritedInterfacesInfo(List<TypeLibInfo> list, string want)
        {
            foreach (TypeLibInfo item in list)
            {
                var library = GetLibraryNode(item);
                var project = GetProjectNode(item.Name);
                var faces = project.Elements(want).FirstOrDefault();

                TLI.Interfaces interfaces = item.Interfaces;
                foreach (TLI.InterfaceInfo itemInterface in interfaces)
                {
                    List<InterfaceInfo> inheritedList = TypeDescriptor.GetInheritedInterfaces(itemInterface);
                    if (inheritedList.Count > 0)
                    {
                        XElement faceNode = GetInterfaceNode(item, itemInterface);
                        foreach (InterfaceInfo itemInherited in inheritedList)
                        {
                            XElement inheritedInterface = GetInterfaceNodeFromInheritedInfo(itemInherited);
                            string key = inheritedInterface.Attribute("Key").Value;
                            AddInterfaceKeyToInherited(faceNode, key, library);

                            Marshal.ReleaseComObject(itemInherited);
                        }
                        inheritedList.Clear();
                    }

                    Marshal.ReleaseComObject(itemInterface);
                }
                Marshal.ReleaseComObject(interfaces);
            }
        }

        /// <summary>
        /// marks used event interfaces
        /// </summary>
        private void UpdateEventInterfaces()
        {
            var classes = _document.Element("LateBindingApi.CodeGenerator.Document").Elements("Solution").Elements("Projects").Elements("Project").Descendants("CoClass");
            foreach (var itemClass in classes)
            {
                var events = itemClass.Element("EventInterfaces");
                foreach (var evItem in events.Elements("Ref"))
                {
                    XElement face = GetNodeByKey(evItem.Attribute("Key"));
                    face.Attribute("IsEventInterface").Value = "true";
                }
            }
        }

        /// <summary>
        /// scan all interfaces in typelibs and info to document about evet interfaces
        /// all interfaces must be listed in document before call this method
        /// </summary>
        /// <param name="list"></param>
        private void AddEventCoClassInfo(List<TypeLibInfo> list)
        {
            foreach (TypeLibInfo itemLib in list)
            {
                var libraryNode = GetLibraryNode(itemLib);

                TLI.CoClasses classes = itemLib.CoClasses;
                foreach (TLI.CoClassInfo itemClass in classes)
                {
                    List<InterfaceInfo> inheritedList = TypeDescriptor.GetEventInterfaces(itemClass);
                    if (inheritedList.Count > 0)
                    {
                        XElement classNode = GetClassNode(itemLib, itemClass);
                        foreach (InterfaceInfo itemInherited in inheritedList)
                        {
                            XElement inheritedInterface = GetInterfaceNodeFromInheritedInfo(itemInherited);
                            string key = inheritedInterface.Attribute("Key").Value;
                            AddInterfaceKeyToEvent(classNode, key, libraryNode);

                            Marshal.ReleaseComObject(itemInherited);
                        }
                        inheritedList.Clear();
                    }

                    Marshal.ReleaseComObject(itemClass);
                }
                Marshal.ReleaseComObject(classes);
            }
        }

        /// <summary>
        /// scan all interfaces in typelibs and info to document about default interfaces
        /// all interfaces must be listed in document before call this method
        /// </summary>
        /// <param name="list"></param>
        private void AddDefaultCoClassInfo(List<TypeLibInfo> list)
        {
            foreach (TypeLibInfo itemLib in list)
            {
                var libraryNode = GetLibraryNode(itemLib);

                TLI.CoClasses classes = itemLib.CoClasses;
                foreach (TLI.CoClassInfo itemClass in classes)
                {
                    List<InterfaceInfo> inheritedList = TypeDescriptor.GetDefaultInterfaces(itemClass);
                    if (inheritedList.Count > 0)
                    {
                        XElement classNode = GetClassNode(itemLib, itemClass);
                        foreach (InterfaceInfo itemInherited in inheritedList)
                        {
                            XElement inheritedInterface = GetInterfaceNodeFromInheritedInfo(itemInherited);
                            string key = inheritedInterface.Attribute("Key").Value;
                            AddInterfaceKeyToDefault(classNode, key, libraryNode);

                            Marshal.ReleaseComObject(itemInherited);
                        }
                        inheritedList.Clear();
                    }

                    Marshal.ReleaseComObject(itemClass);
                }
                Marshal.ReleaseComObject(classes);
            }
        }

        /// <summary>
        /// scan all interfaces in typelibs and info to document about inherited interfaces
        /// all interfaces must be listed in document before call this method
        /// </summary>
        /// <param name="list"></param>
        private void AddInheritedCoClassInfo(List<TypeLibInfo> list)
        {
            foreach (TypeLibInfo itemLib in list)
            {
                var libraryNode = GetLibraryNode(itemLib);
              
                TLI.CoClasses classes = itemLib.CoClasses;
                foreach (TLI.CoClassInfo itemClass in classes)
                {
                    List<InterfaceInfo> inheritedList = TypeDescriptor.GetInheritedInterfaces(itemClass);
                    if (inheritedList.Count > 0)
                    {
                        XElement classNode = GetClassNode(itemLib, itemClass);
                        foreach (InterfaceInfo itemInherited in inheritedList)
                        {
                            if (false == HasEventInterface(classNode, itemInherited.Name))
                            { 
                                XElement inheritedInterface = GetInterfaceNodeFromInheritedInfo(itemInherited);
                                string key = inheritedInterface.Attribute("Key").Value;
                                AddInterfaceKeyToInherited(classNode, key, libraryNode);
                            }
                            Marshal.ReleaseComObject(itemInherited);
                        }
                        inheritedList.Clear();
                    }

                    Marshal.ReleaseComObject(itemClass);
                }
                Marshal.ReleaseComObject(classes);
            }
        }

        /// <summary>
        /// returns info classNode as an EventInterface named with interfaceName
        /// </summary>
        /// <param name="classNode"></param>
        /// <param name="interfaceName"></param>
        /// <returns></returns>
        private bool HasEventInterface(XElement classNode, string interfaceName)
        {
            foreach (var item in classNode.Element("EventInterfaces").Elements("Ref"))
	        {
                string key = item.Attribute("Key").Value;
                XElement projectNode = classNode.Parent.Parent;
                XElement face = GetInterfaceNode(projectNode, key);
                if(face.Attribute("Name").Value == interfaceName)
                    return true;
	        }            
            return false;
        }

        #endregion

        #region ResetDocument
        
        /// <summary>
        /// clear document and load template xml
        /// </summary>
        private void ResetDocument()
        {
            if (null == _schemaSet)
                _schemaSet = new XmlSchemaSet();

            if (null == _document)
                _document = new XDocument();
            
            _document.RemoveNodes();

            string xmlTemplateContent = Utils.ReadTextFileFromRessource("LateBindingApi.CodeGenerator.Document.xml");
            _document = XDocument.Parse(xmlTemplateContent);

            ValidateSchema();
       }

        /// <summary>
        /// validate document schema and throws a XmlException in case of rules violated
        /// </summary>
        private void ValidateSchema()
        {
            string xsdSchemaContent = Utils.ReadTextFileFromRessource("LateBindingApi.CodeGenerator.Document.xsd");
          
            if (null == _schema)
                _schema = _schemaSet.Add("http://latebindingapi.codeplex.com/XMLSchema.xsd", XmlReader.Create(new StringReader(xsdSchemaContent)));

            bool errors = false;
            _document.Validate(_schemaSet, (o, e) => errors = true);
            
            if (true == errors)
            {
                string message = "XML contains schema errors.";
                throw (new XmlException(message));
            }

            // version check
            XElement docElement = _document.Element("LateBindingApi.CodeGenerator.Document");
            XAttribute versionAttribute = docElement.Attribute("Version");

            if(null == versionAttribute)
                throw (new ProjectFileFormatException("Document is not a valid LateBindingApi.CodeGenerator.Document"));

            if (_documentVersion != versionAttribute.Value)
            {
                string message = string.Format("Document is not a valid LateBindingApi.CodeGenerator.Document in Version {0}.\r\nThe specified version of your file is {1}.", _documentVersion, versionAttribute.Value);
                throw (new ProjectFileFormatException(message));            
            }
        }
         
        #endregion 
    }
}
