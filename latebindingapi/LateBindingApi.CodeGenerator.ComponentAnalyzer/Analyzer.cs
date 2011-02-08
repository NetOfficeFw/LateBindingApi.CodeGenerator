using System;
using System.IO;
using System.Runtime.InteropServices; 
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;

using TLI;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer
{
    public delegate void UpdateHandler(object sender, string message);

    public partial class Analyzer
    {
        #region Fields
        
        TLIApplication           _typeLibApplication;
        XDocument                _document;
      
        #endregion

        #region Events

        public event UpdateHandler Update;

        #endregion

        #region Properties

        public XDocument Document
        {
            get 
            {
                return _document;
            }
        }
      
        #endregion

        #region Construction

        public Analyzer()
        {
            ResetDocument();
        }

        #endregion
 
        #region LoadSave Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="addToCurrentProject"></param>
        public void LoadCOMComponents(string[] files, bool addToCurrentProject)
        {
            DoUpdate("Prepare");
            _typeLibApplication = new TLIApplication();
            if (false == addToCurrentProject)
                ResetDocument();
            
            DoUpdate("Load Components");
            List<TypeLibInfo> types = LoadComponents(files);

            DoUpdate("Add Dependencies");
            AddDependencies(types);

            DoUpdate("Load Solution");
            LoadSolution(types);

            DoUpdate("Load Enums");
            LoadEnums(types);

            DoUpdate("Load Interfaces");
            LoadInterfaces(types, false);

            DoUpdate("Load Dispatch Interfaces");
            LoadInterfaces(types, true);

            /*
                LoadCoClasses(settings);
            */

            DoUpdate("Finsishing operations");
            ClearList(types);
            Marshal.ReleaseComObject(_typeLibApplication);
            _typeLibApplication = null;

            DoUpdate("Done!");
        }

        public void LoadProject(string fileName)
        {
            _document = XDocument.Load(fileName);
            ValidateSchema();
        }

        public void SaveProject(string fileName)
        {
            ValidateSchema();
            _document.Save(fileName);
        }

        #endregion
        
        #region Private Methods

        private void DoUpdate(string message)
        {
            if (null != Update)
                Update(this, message); 
        }

        private void ClearList(List<TypeLibInfo> list)
        {
            foreach (TypeLibInfo item in list)
                Marshal.ReleaseComObject(item);

            list.Clear();
        }

        private void DetectMembers(List<TypeLibInfo> list, Members members)
        {
            foreach (MemberInfo itemMember in members)
            {
                VarTypeInfo typeInfo = itemMember.ReturnType;
                if ((typeInfo != null) && (true == typeInfo.IsExternalType))
                {
                    bool found = false;
                    string containingFile = typeInfo.TypeLibInfoExternal.ContainingFile;
                    foreach (TypeLibInfo item in list)
                    {
                        if (item.ContainingFile == containingFile)
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
        
                }
                if (null != typeInfo)
                    Marshal.ReleaseComObject(typeInfo);
            }
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
                CoClasses classInfos = libInfo.CoClasses;
                foreach (CoClassInfo itemInfo in classInfos)
                {
                    DetectMembers(list, itemInfo.DefaultInterface.Members);
                    Marshal.ReleaseComObject(itemInfo);
                }
                Marshal.ReleaseComObject(classInfos);

                Interfaces faceInfos = libInfo.Interfaces;
                foreach (InterfaceInfo itemInfo in faceInfos)
                {
                    DetectMembers(list, itemInfo.Members);
                    Marshal.ReleaseComObject(itemInfo);
                }
                Marshal.ReleaseComObject(faceInfos);
            }
        }

        private void AddTypeLibToDocument(TypeLibInfo libInfo)
        {
            string guid = Utils.EncodeGuid(libInfo.GUID);

            var node = (from a in _document.Descendants("Components").Elements("Component")
                        where a.Attribute("GUID").Value.Equals(guid) &&
                              a.Attribute("Name").Value.Equals(libInfo.Name) &&
                              a.Attribute("MajorVersion").Value.Equals(libInfo.MajorVersion.ToString()) &&
                              a.Attribute("MinorVersion").Value.Equals(libInfo.MinorVersion.ToString())
                        select a).FirstOrDefault();

            if (null == node)
            {
                node = new XElement("Component",
                            new XAttribute("Name", libInfo.Name),
                            new XAttribute("ContainingFile", libInfo.ContainingFile),
                            new XAttribute("Key", Utils.NewEncodedGuid()),
                            new XAttribute("GUID", guid),
                            new XAttribute("HelpFile", Utils.ValidatePath(libInfo.HelpFile)),
                            new XAttribute("MajorVersion", libInfo.MajorVersion),
                            new XAttribute("MinorVersion", libInfo.MinorVersion),
                            new XAttribute("LCID", libInfo.LCID),
                            new XAttribute("Description", Utils.GetTypeLibDescription(libInfo)),
                            new XAttribute("Version", libInfo.Name.Substring(0, 2).ToUpper() + "1"),
                            new XAttribute("SysKind", Convert.ToInt32(libInfo.SysKind)));

                var components = (_document.Descendants("Components")).FirstOrDefault();
                components.Add(node);
            }
        }

        /// <summary>
        /// load all typelib files
        /// </summary>
        /// <param name="files"></param>
        /// <returns>list of loaded typelib</returns>
        private List<TypeLibInfo> LoadComponents(string[] files)
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
        
        private bool IsTargetInterfaceType(TypeKinds kind, bool wantDispatch)
        {
            if (true == wantDispatch)
            {
                if (kind == TypeKinds.TKIND_DISPATCH)
                    return true;
                else
                    return false;

            }
            else
            {
                if (kind == TypeKinds.TKIND_DISPATCH)
                    return false;
                else
                    return true;
            }
        }
        private XElement GetComponent(TypeLibInfo libInfo)
        {
            string guid = Utils.EncodeGuid(libInfo.GUID);
            var component = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Elements("Components").Elements("Component")
                             where a.Attribute("GUID").Value.Equals(guid) &&
                                   a.Attribute("Name").Value.Equals(libInfo.Name) &&
                                   a.Attribute("MajorVersion").Value.Equals(libInfo.MajorVersion.ToString()) &&
                                   a.Attribute("MinorVersion").Value.Equals(libInfo.MinorVersion.ToString())
                             select a).FirstOrDefault();
            return component;
        }
        
        private XElement GetProject(string name)
        {
            var project = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Elements("Projects").Elements("Project")
                           where a.Attribute("Name").Value.Equals(name,StringComparison.InvariantCultureIgnoreCase)
                           select a).FirstOrDefault();
            return project;
        }

        private void AddGuidToInterface(TLI.InterfaceInfo itemInterface, XElement guids)
        {
            string key = Utils.EncodeGuid(itemInterface.GUID);

            var guid = (from a in guids.Elements("GUID")
                        where a.Attribute("Guid").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();

            if (null == guid)
            {
                guid = new XElement("GUID",
                            new XElement("RefComponents"),
                            new XAttribute("Guid", key));
                guids.Add(guids);
            }
        }

        private void AddComponentKeyToRefComponent(XElement component, XElement refNode)
        {
            string key = component.Attributes("Key").FirstOrDefault().Value;

            var comp = (from a in refNode.Elements("Ref")
                        where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();

            if (null == comp)
            {
                comp = new XElement("Ref",
                            new XAttribute("Key", key));

                refNode.Add(comp);
            }

        }

        private XElement CreateInterfaceNode(XElement faces, TLI.InterfaceInfo itemInterface)
        {
            var faceNode = (from a in faces.Elements()
                            where a.Attribute("Name").Value.Equals(itemInterface.Name, StringComparison.InvariantCultureIgnoreCase)
                            select a).FirstOrDefault();

            if (null == faceNode)
            {
                faceNode = new XElement("Interface",
                               new XElement("Methods"),
                               new XElement("Properties"),
                               new XElement("GUIDs"),
                               new XElement("Inherited"),
                               new XElement("RefComponents"),
                               new XAttribute("Name", itemInterface.Name),
                               new XAttribute("Key",  Utils.NewEncodedGuid()));
                              
                faces.Add(faceNode);
            }

            return faceNode;
        }

        private XElement CreateEnumNode(XElement enums, TLI.ConstantInfo itemConstant)
        {
            var enumNode = (from a in enums.Elements()
                            where a.Attribute("Name").Value.Equals(itemConstant.Name, StringComparison.InvariantCultureIgnoreCase)
                            select a).FirstOrDefault();
            
            if (null == enumNode)
            {
                enumNode = new XElement("Enum",
                               new XElement("Members"),
                               new XElement("RefComponents"),
                               new XAttribute("Name", itemConstant.Name),
                               new XAttribute("Key",  Utils.NewEncodedGuid()));

                enums.Add(enumNode);
            }

            return enumNode;
        }

        private XElement CreateEnumMemberNode(XElement membersNode, TLI.MemberInfo itemMember) 
        {
            var memberNode = (from a in membersNode.Elements()
                              where a.Attribute("Name").Value.Equals(itemMember.Name, StringComparison.InvariantCultureIgnoreCase)
                              select a).FirstOrDefault();

            if (null == memberNode)
            {
                memberNode = new XElement("Member",
                                new XElement("RefComponents"),
                                new XAttribute("Name", itemMember.Name),
                                new XAttribute("Value", itemMember.Value));

                membersNode.Add(memberNode);
            }

            return memberNode;
        }

        private void LoadInterfaces(List<TypeLibInfo> list, bool wantDispatch)
        {
            string wantedInterfaces = "Interfaces";
            if (wantDispatch == true)
                wantedInterfaces = "DispatchInterfaces";

            foreach (TypeLibInfo item in list)
            {
                var component = GetComponent(item);
                var project = GetProject(item.Name);
                var faces = project.Elements(wantedInterfaces).FirstOrDefault();

                TLI.Interfaces interfaces = item.Interfaces;
                foreach (TLI.InterfaceInfo itemInterface in interfaces)
                {
                    if (true == IsTargetInterfaceType(itemInterface.TypeKind, wantDispatch))
                    {
                        var faceNode = CreateInterfaceNode(faces, itemInterface);

                        TLI.Members interfaceMembers = itemInterface.Members;
                        foreach (TLI.MemberInfo itemMember in interfaceMembers)
                        {

                            Marshal.ReleaseComObject(itemMember);
                        }
                        Marshal.ReleaseComObject(interfaceMembers);

                        var guids = faceNode.Elements("GUIDs").FirstOrDefault();
                        AddGuidToInterface(itemInterface, guids);

                        var refComponents = faceNode.Elements("RefComponents").FirstOrDefault();
                        AddComponentKeyToRefComponent(component, refComponents);
                    }
                    Marshal.ReleaseComObject(itemInterface);
                }
                Marshal.ReleaseComObject(interfaces);
            }

            ScanInheritedInterfaces(list, wantDispatch);
            RemoveInheritedEntities(list, wantDispatch);

        }

        private void ScanInheritedInterfaces(List<TypeLibInfo> list, bool wantDispatch)
        {
            string wantedInterfaces = "Interfaces";
            if (wantDispatch == true)
                wantedInterfaces = "DispatchInterfaces";

            foreach (TypeLibInfo item in list)
            {
                var component = GetComponent(item);
                var project = GetProject(item.Name);
                var faces = project.Elements(wantedInterfaces).FirstOrDefault();
            }
        }

        private void RemoveInheritedEntities(List<TypeLibInfo> list, bool wantDispatch)
        {
            string wantedInterfaces = "Interfaces";
            if (wantDispatch == true)
                wantedInterfaces = "DispatchInterfaces";

            foreach (TypeLibInfo item in list)
            {
                var component = GetComponent(item);
                var project = GetProject(item.Name);
                var faces = project.Elements(wantedInterfaces).FirstOrDefault();
            }
        }

        /// <summary>
        /// load all enums in xml document
        /// </summary>
        /// <param name="typelibs"></param>
        private void LoadEnums(List<TypeLibInfo> typelibs)
        {
            foreach (TypeLibInfo item in typelibs)
            {
                var component = GetComponent(item);
                var project   = GetProject(item.Name);
                var enums     = project.Elements("Enums").FirstOrDefault();

                TLI.Constants constants = item.Constants;
                foreach (TLI.ConstantInfo itemConstant in constants)
                {
                    var enumNode = CreateEnumNode(enums, itemConstant);

                    var refComponents = enumNode.Elements("RefComponents").FirstOrDefault();
                    AddComponentKeyToRefComponent(component, refComponents);
                    
                    TLI.Members members = itemConstant.Members;
                    foreach (TLI.MemberInfo itemMember in members)
                    {
                        var membersNode = enumNode.Element("Members");
                        var memberNode = CreateEnumMemberNode(membersNode, itemMember); 
                       
                        refComponents = memberNode.Elements("RefComponents").FirstOrDefault();
                        AddComponentKeyToRefComponent(component, refComponents);

                        Marshal.ReleaseComObject(itemMember);
                    }
                    Marshal.ReleaseComObject(members);

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
                var node = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Elements("Projects").Elements("Project")
                            where a.Attribute("Name").Value.Equals(item.Name)
                            select a).FirstOrDefault();

                if (null == node)
                {
                    node = new XElement("Project",
                           new XElement("Enums"),
                           new XElement("CoClasses"),
                           new XElement("DispatchInterfaces"),
                           new XElement("Interfaces"),
                           new XElement("RefComponents"),

                           new XAttribute("Name", item.Name),
                           new XAttribute("ProjectName", "LateBindingApi." + item.Name),
                           new XAttribute("Namespace", "LateBindingApi." + item.Name),
                           new XAttribute("Key", Utils.NewEncodedGuid()));

                    var projects = _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Elements("Projects").FirstOrDefault();
                    projects.Add(node);
                }
            }
        }

        #endregion

        #region ResetDocument

        private void ResetDocument()
        {
            if (null == _document)
                _document = new XDocument();
            
            _document.RemoveNodes();

            string xmlTemplateContent = ReadText("LateBindingApi.CodeGenerator.Document.xml");
            _document = XDocument.Parse(xmlTemplateContent);

            ValidateSchema();
        }

        private void ValidateSchema()
        {
            string xsdSchemaContent = ReadText("LateBindingApi.CodeGenerator.Document.xsd");
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("http://latebindingapi.codeplex.com/XMLSchema.xsd", XmlReader.Create(new StringReader(xsdSchemaContent)));

            bool errors = false;
            _document.Validate(schemas, (o, e) => errors = true);

            if (true == errors)
            {
                string message = "XML contains schema errors.";
                throw (new XmlException(message));
            }
            
        }

        private string ReadText(string fileName)
        {
            System.IO.Stream ressourceStream = null;
            System.IO.StreamReader textStreamReader = null;
            try
            {
                string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                fileName = assemblyName + "." + fileName;
                ressourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
                if (ressourceStream == null)
                    throw (new System.IO.IOException("Error accessing resource Stream."));

                textStreamReader = new System.IO.StreamReader(ressourceStream);
                if (textStreamReader == null)
                    throw (new System.IO.IOException("Error accessing resource File."));

                string text = textStreamReader.ReadToEnd();
                ressourceStream.Close();
                textStreamReader.Close();
                return text;
            }
            catch (Exception exception)
            {
                throw (exception);
            }
        }

        #endregion 
    }
}
