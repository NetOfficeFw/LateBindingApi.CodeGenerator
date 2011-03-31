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
        XmlSchemaSet             _schemaSet;
        XmlSchema                _schema;
        
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
            ResetDocument();
        }

        #endregion
 
        #region LoadSave Methods

        /// <summary>
        /// load type library informations to a LateBindingApi.CodeGenerator.Document
        /// see LateBindingApi.CodeGenerator.Document.xsd
        /// </summary>
        /// <param name="files">typelib paths</param>
        /// <param name="addToCurrentProject">dont clear old project</param>
        public void LoadTypeLibraries(string[] files, bool addToCurrentProject)
        {
            DoUpdate("Prepare");
            _typeLibApplication = new TLIApplication();
            if (false == addToCurrentProject)
                ResetDocument();

            DoUpdate("Load Type Libraries");
            List<TypeLibInfo> types = LoadLibraries(files);

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
           
            DoUpdate("Add Inherited Interfaces");
            AddInheritedInterfacesInfo(types, "Interfaces");

            DoUpdate("Add Inherited DispatchInterfaces");
            AddInheritedInterfacesInfo(types, "DispatchInterfaces");

            DoUpdate("Load CoClasses");
            LoadCoClasses(types);

            DoUpdate("Add Inherited CoClasses");
            AddInheritedCoClassInfo(types);

            DoUpdate("Add Event Interfaces");

            DoUpdate("Add Default Interfaces");


            DoUpdate("Finsishing operations");
            ReleaseTypeLibrariesList(types);
            Marshal.ReleaseComObject(_typeLibApplication);
            _typeLibApplication = null;

            DoUpdate("Done!");
        }

        /// <summary>
        /// load project from xml file
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadProject(string fileName)
        {
            _document = XDocument.Load(fileName);
            ValidateSchema();
        }

        /// <summary>
        /// save project to xml file
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveProject(string fileName)
        {
            ValidateSchema();
            _document.Save(fileName);
        }

        #endregion
        
        #region Private Methods

        /// <summary>
        /// fires update event to client
        /// </summary>
        /// <param name="message"></param>
        private void DoUpdate(string message)
        {
            if (null != Update)
                Update(this, message); 
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
                CoClasses classInfos = libInfo.CoClasses;
                foreach (CoClassInfo itemInfo in classInfos)
                {
                    DetectMembersTypesDefinedInExternalLibrary(list, itemInfo.DefaultInterface.Members);
                    Marshal.ReleaseComObject(itemInfo);
                }
                Marshal.ReleaseComObject(classInfos);

                Interfaces faceInfos = libInfo.Interfaces;
                foreach (InterfaceInfo itemInfo in faceInfos)
                {
                    DetectMembersTypesDefinedInExternalLibrary(list, itemInfo.Members);
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
            var node = (from a in _document.Elements("Libraries").Elements("Library")
                        where a.Attribute("GUID").Value.Equals(guid) &&
                              a.Attribute("Name").Value.Equals(libInfo.Name) &&
                              a.Attribute("Major").Value.Equals(libInfo.MajorVersion.ToString()) &&
                              a.Attribute("Minor").Value.Equals(libInfo.MinorVersion.ToString())
                        select a).FirstOrDefault();

            if (null == node)
            {
                node = new XElement("Library",
                            new XAttribute("Name", libInfo.Name),
                            new XAttribute("File", libInfo.ContainingFile),
                            new XAttribute("Key", Utils.NewEncodedGuid()),
                            new XAttribute("GUID", guid),
                            new XAttribute("HelpFile", Utils.RemoveBadChars(libInfo.HelpFile)),
                            new XAttribute("Major", libInfo.MajorVersion),
                            new XAttribute("Minor", libInfo.MinorVersion),
                            new XAttribute("LCID", libInfo.LCID),
                            new XAttribute("Description", TypeDescriptor.GetTypeLibDescription(libInfo)),
                            new XAttribute("Version", libInfo.Name.Substring(0, 2).ToUpper() + "1"),
                            new XAttribute("SysKind", Convert.ToInt32(libInfo.SysKind)));

                var components = (_document.Descendants("Libraries")).FirstOrDefault();
                components.Add(node);
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
                var library    = GetLibraryNode(item);
                var project   = GetProjectNode(item.Name);
                var enums     = project.Elements("Enums").FirstOrDefault();

                TLI.Constants constants = item.Constants;
                foreach (TLI.ConstantInfo itemConstant in constants)
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
                    node = new XElement("Project",
                               new XElement("Enums"),
                               new XElement("CoClasses"),
                               new XElement("DispatchInterfaces"),
                               new XElement("Interfaces"),
                               new XElement("RefLibraries"),
                               new XAttribute("Name",        item.Name),
                               new XAttribute("Namespace",   "LateBindingApi." + item.Name),
                               new XAttribute("Key",         Utils.NewEncodedGuid()));

                    var projects = _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Elements("Projects").FirstOrDefault();
                    projects.Add(node);
                }
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
                    
                    // if (itemInterface.Name != "AddIns")
                    //   continue;
                    
                    if (true == TypeDescriptor.IsTargetInterfaceType(itemInterface.TypeKind, wantDispatch))
                    {
                        var faceNode = CreateInterfaceNode(faces, itemInterface);

                        List<TLI.MemberInfo> listMembers = TypeDescriptor.GetFilteredMembers(itemInterface);
                        foreach (TLI.MemberInfo itemMember in listMembers)
                        {
                            if (true == TypeDescriptor.IsInterfaceMethod(itemMember))
                            {
                                //if (itemMember.Name == "Add")
//{ 
                                    var methodNode = MethodHandler.CreateMethodNode(itemMember, faceNode);
                                    AddDispatchIdToEntityNode(library, methodNode, itemMember.MemberId.ToString());
                                    var refNode = methodNode.Elements("RefLibraries").FirstOrDefault();
                                    AddLibraryKeyToRefLibraries(library, refNode);
                                    MethodHandler.AddMethod(library, methodNode, itemMember);
                                //}
                            }
                            else if (true == TypeDescriptor.IsInterfaceProperty(itemMember))
                            {
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
                    
                    // default 
                    // event
                    // inherited

                    var refComponents = classNode.Elements("RefLibraries").FirstOrDefault();
                    AddLibraryKeyToRefLibraries(library, refComponents);

                    Marshal.ReleaseComObject(itemClass);
                }
                Marshal.ReleaseComObject(itemClasses);
            }
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
                                   a.Attribute("Major").Value.Equals(libInfo.MajorVersion.ToString()) &&
                                   a.Attribute("Minor").Value.Equals(libInfo.MinorVersion.ToString())
                             select a).FirstOrDefault();
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

            if (null == faceNode)
            {
                faceNode = new XElement("Interface",
                               new XElement("Methods"),
                               new XElement("Properties"),
                               new XElement("DispIds"),
                               new XElement("Inherited"),
                               new XElement("RefLibraries"),
                               new XAttribute("Name", itemInterface.Name),
                               new XAttribute("Key",  Utils.NewEncodedGuid()));

                faces.Add(faceNode);
            }

            return faceNode;
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
        private void DetectMembersTypesDefinedInExternalLibrary(List<TypeLibInfo> list, Members members)
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
        /// add component key to refs node
        /// </summary>
        /// <param name="component"></param>
        /// <param name="refNode"></param>
        private void AddInterfaceKeyToInherited(XElement faceNode, string key)
        {

            // check Interface exists 
            XElement inheritedsNode = faceNode.Element("Inherited");
 
            var node = (from a in inheritedsNode.Elements("Ref")
                        where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();

            if (null == node)
            {
                node = new XElement("Ref",
                            new XAttribute("Key", key));

                inheritedsNode.Add(node);
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
                            AddInterfaceKeyToInherited(faceNode, key);

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
        /// scan all interfaces in typelibs and info to document about inherited interfaces
        /// all interfaces must be listed in document before call this method
        /// </summary>
        /// <param name="list"></param>
        /// <param name="wantDispatch"></param>
        private void AddInheritedCoClassInfo(List<TypeLibInfo> list)
        {
            foreach (TypeLibInfo item in list)
            {
                var library = GetLibraryNode(item);
                var project = GetProjectNode(item.Name);
                var faces = project.Elements("CoClasses").FirstOrDefault();

                TLI.CoClasses classes = item.CoClasses;
                foreach (TLI.CoClassInfo itemClass in classes)
                {
                    List<InterfaceInfo> inheritedList = TypeDescriptor.GetInheritedInterfaces(itemClass);
                    if (inheritedList.Count > 0)
                    {
                        XElement faceNode = GetClassNode(item, itemClass);
                        foreach (InterfaceInfo itemInherited in inheritedList)
                        {
                            XElement inheritedInterface = GetInterfaceNodeFromInheritedInfo(itemInherited);
                            string key = inheritedInterface.Attribute("Key").Value;
                            AddInterfaceKeyToInherited(faceNode, key);

                            Marshal.ReleaseComObject(itemInherited);
                        }
                        inheritedList.Clear();
                    }

                    Marshal.ReleaseComObject(itemClass);
                }
                Marshal.ReleaseComObject(classes);
            }
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

        }
         
        #endregion 
    }
}
