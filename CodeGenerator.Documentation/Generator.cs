using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using LateBindingApi.CodeGenerator.ComponentAnalyzer;
using LateBindingApi.CodeGenerator.CSharp;

namespace LateBindingApi.CodeGenerator.Documentation
{
    public class DocuGenerator : ICodeGenerator
    {
        #region Fields

        Settings _settings;
        static FakedEnumeratorManager _enumerators;
        static CustomMethodManager _customMethods;
        static CustomPropertyManager _customProperties;

        static XDocument _document;

        #endregion

        #region Construction

        public DocuGenerator()
        {
        }

        private void DoUpdate(string message)
        {
            Progress?.Report(message);
        }

        private string GetLibraryVersion(string key)
        {
            XElement node = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Libraries").Elements("Library")
                             where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();
            
            return node.Attribute("Version").Value;
        }

        private XElement GetEventInterface(string key) 
        {
            XElement node = (from a in _document.Descendants("Interface")
                            where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();
            return node;
        }

        private void GenerateProperty(XElement itemMethod, XElement docuClassItem, string prefix)
        {
            XElement newPropertyNode = null;
            List<XElement> newNodes = new List<XElement>();


            if (("set_" == prefix)&& (itemMethod.Attribute("Name").Value == "DisplayAlerts"))
            {
            }

            // version support
            foreach (XElement itemParameters in itemMethod.Elements("Parameters"))
            {
                string propName = itemMethod.Attribute("Name").Value;
                if (propName == "_NewEnum")
                    propName = "GetEnumerator";
                else
                    propName = prefix + propName;

                newPropertyNode = new XElement("Method", new XElement("Parameters", new XElement("SupportByVersion", "")), new XAttribute("Name", propName));
                docuClassItem.Element("Methods").Add(newPropertyNode);
                newNodes.Add(newPropertyNode);

                foreach (XElement itemRefLib in itemParameters.Element("RefLibraries").Elements("Ref"))
                {
                    string key = itemRefLib.Attribute("Key").Value;
                    string version = GetLibraryVersion(key);
                    XElement versionNode = (from a in newPropertyNode.Element("Parameters").Elements("SupportByVersion").Elements("Version")
                                            where a.Value.Equals(version, StringComparison.InvariantCultureIgnoreCase)
                                            select a).FirstOrDefault();
                    if (null == versionNode)
                    {
                        versionNode = new XElement("Version", version);
                        newPropertyNode.Element("Parameters").Element("SupportByVersion").Add(versionNode);
                    }
                }           
 
            }
           
            foreach (XElement itemParams in itemMethod.Elements("Parameters"))
            {
                XElement newParam = null;

                if (("set_" == prefix) && (itemParams.Elements("Parameter").Count() == 0))
                {
                    newParam = new XElement("Parameter",
                                         new XAttribute("Name", "value"),
                                         new XAttribute("Type", itemParams.Element("ReturnValue").Attribute("Type").Value),
                                         new XAttribute("IsOptional", "false"));
                    newNodes[0].Element("Parameters").Add(newParam);
                }
                else
                {
                    int i = 0;
                    foreach (XElement itemParam in itemParams.Elements("Parameter"))
                    {
                        string paramName = itemParam.Attribute("Name").Value;
                        string paramType = itemParam.Attribute("Type").Value;
                        string paramIsOptional = itemParam.Attribute("IsOptional").Value;

                        newParam = new XElement("Parameter",
                                            new XAttribute("Name", paramName),
                                            new XAttribute("Type", paramType),
                                            new XAttribute("IsOptional", paramIsOptional));

                        newNodes[i].Element("Parameters").Add(newParam);
                    }
                    i++;
                }
            }
           
        }

        private void GenerateProperties(XElement itemClass, XElement docuClassItem)
        {
            foreach (XElement itemMethod in itemClass.Element("Properties").Elements("Property"))
            {
                if (itemMethod.Attribute("InvokeKind").Value.Equals("INVOKE_PROPERTYGET", StringComparison.InvariantCultureIgnoreCase))
                {
                    GenerateProperty(itemMethod, docuClassItem, "get_");
                }
                else
                {
                    GenerateProperty(itemMethod, docuClassItem, "get_");
                    GenerateProperty(itemMethod, docuClassItem, "set_");
                }
            }
        }

        private void GenerateMethods(XElement itemClass, XElement docuClassItem)
        {
            foreach (XElement itemMethod in itemClass.Element("Methods").Elements("Method"))
            {
                foreach (XElement itemParameters in itemMethod.Elements("Parameters"))
                {
                    string methodName = itemMethod.Attribute("Name").Value;
                    if (methodName == "_NewEnum")
                        methodName = "GetEnumerator";   
                    else if (methodName == "Item")
                        methodName = "get_Item";

                    XElement newMethodNode = new XElement("Method", new XElement("Parameters"), new XAttribute("Name", methodName));

                    foreach (XElement itemParam in itemParameters.Elements("Parameter"))
                    {
                        string paramName = itemParam.Attribute("Name").Value;
                        string paramType = itemParam.Attribute("Type").Value;
                        string paramIsOptional = itemParam.Attribute("IsOptional").Value;

                        XElement newParam = new XElement("Parameter", 
                                            new XAttribute("Name", paramName), 
                                            new XAttribute("Type", paramType), 
                                            new XAttribute("IsOptional", paramIsOptional));

                        newMethodNode.Element("Parameters").Add(newParam);
                    }

                    XElement supportNode = new XElement("SupportByVersion");
                    string[] supportDocuArray = CSharpGenerator.GetSupportByVersionArray(itemParameters);

                    List<XElement> otherOverloads = MethodApi.GetOverloadsWithMoreParameters(itemParameters, itemMethod.Elements("Parameters"));
                    foreach (XElement other in otherOverloads)
                        supportDocuArray = DocumentationApi.AddParameterDocumentation(supportDocuArray, other);

                    foreach (string item in supportDocuArray)
                        supportNode.Add(new XElement("Version", item));

                    newMethodNode.Element("Parameters").Add(supportNode);
                    docuClassItem.Element("Methods").Add(newMethodNode);
                }
            }
        }
      
        private XDocument CreateWorkingCopy()
        {
            XDocument document = new XDocument(_document);
            return document;
        }


        void _job_DoWork()
        {
            DoUpdate("Create root folder");
            PathApi.ClearCreateFolder(_settings.Folder);
          
            DoUpdate("Create Copy");
            _document = CreateWorkingCopy();

            DoUpdate("Scan for missed enumerators");
            _enumerators = new FakedEnumeratorManager(this, _document);
            _enumerators.ScanForMissedEnumerators();

            DoUpdate("Scan for optional parameter methods");
            _customMethods = new CustomMethodManager(this, _document);
            _customMethods.ScanForOptionalMethods();

             DoUpdate("Scan for optional parameter properties");
             _customProperties = new CustomPropertyManager(this, _document);
             _customProperties.ScanForOptionalProperties();

            foreach (XElement projectItem in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project"))
            {
                XDocument newDocument = new XDocument();
                XElement newAssemblyNode = new XElement("Assembly", new XAttribute("Name", projectItem.Attribute("Name").Value + "Api"));
                newAssemblyNode.Add(new XElement("Types"));
                newAssemblyNode.Add(new XElement("Enums"));
                newDocument.Add(newAssemblyNode);

                DoUpdate("Create Constants Docu");
                foreach (XElement constantItem in projectItem.Element("Constants").Elements("Constant"))
                {
                    XElement newConstantNode = new XElement("Constant", new XAttribute("Name", constantItem.Attribute("Name").Value));

                    XElement enumSupportNode = new XElement("SupportByVersion");
                    foreach (XElement itemRefLib in constantItem.Element("RefLibraries").Elements("Ref"))
                    {
                        string key = itemRefLib.Attribute("Key").Value;
                        string version = GetLibraryVersion(key);
                        enumSupportNode.Add(new XElement("Version", version));
                    }
                    newConstantNode.Add(enumSupportNode);
                }

                DoUpdate("Create Enums Docu");
                foreach (XElement enumItem in projectItem.Element("Enums").Elements("Enum"))
                {
                    XElement newEnumNode = new XElement("Enum", new XAttribute("Name", enumItem.Attribute("Name").Value));
                    newEnumNode.Add(new XElement("Members"));
                    newAssemblyNode.Element("Enums").Add(newEnumNode);
                    
                    XElement enumSupportNode = new XElement("SupportByVersion");
                    foreach (XElement itemRefLib in enumItem.Element("RefLibraries").Elements("Ref"))
                    {
                        string key = itemRefLib.Attribute("Key").Value;
                        string version = GetLibraryVersion(key);
                        enumSupportNode.Add(new XElement("Version", version));
                    }
                    newEnumNode.Add(enumSupportNode);

                    foreach (XElement itemMember in enumItem.Element("Members").Elements("Member"))
                    {
                        XElement enumMember = new XElement("Member", 
                                                    new XAttribute("Name", itemMember.Attribute("Name").Value),
                                                    new XAttribute("Value", itemMember.Attribute("Value").Value));
                        
                        XElement memberSupportNode = new XElement("SupportByVersion");
                        foreach (XElement itemRefLib in itemMember.Element("RefLibraries").Elements("Ref"))
                        {
                            string key = itemRefLib.Attribute("Key").Value;
                            string version = GetLibraryVersion(key);
                            memberSupportNode.Add(new XElement("Version", version));
                        }
                        enumMember.Add(memberSupportNode);

                        newEnumNode.Element("Members").Add(enumMember);
                    }

                }
               
                DoUpdate("Create Interface Docu");
                List<XElement> listElements = new List<XElement>();
                foreach (XElement itemInterface in projectItem.Element("DispatchInterfaces").Elements("Interface"))
                    listElements.Add(itemInterface);
                foreach (XElement itemInterface in projectItem.Element("Interfaces").Elements("Interface"))
                    listElements.Add(itemInterface);

                foreach (XElement itemInterface in listElements)
                {
                    XElement newInterfaceNode = new XElement("Type", new XElement("Methods"),new XAttribute("Name", itemInterface.Attribute("Name").Value));
                    XElement supportNode = new XElement("SupportByVersion");

                    foreach (XElement itemRefLib in itemInterface.Element("RefLibraries").Elements("Ref"))
                    {
                        string key = itemRefLib.Attribute("Key").Value;
                        string version = GetLibraryVersion(key);
                        supportNode.Add(new XElement("Version", version));
                    }
                    newInterfaceNode.Add(supportNode);
 
                    GenerateProperties(itemInterface, newInterfaceNode);
                    GenerateMethods(itemInterface, newInterfaceNode);

                    newAssemblyNode.Element("Types").Add(newInterfaceNode);
                }

                DoUpdate("Create Class Docu");
                foreach (XElement itemClass in projectItem.Element("CoClasses").Elements("CoClass"))
                {
                    XElement newClassNode = new XElement("Type", new XElement("Events"), new XAttribute("Name", itemClass.Attribute("Name").Value));
                    newAssemblyNode.Element("Types").Add(newClassNode);
                    XElement supportNode = new XElement("SupportByVersion");
                    foreach (XElement itemRefLib in itemClass.Element("RefLibraries").Elements("Ref"))
                    {
                        string key = itemRefLib.Attribute("Key").Value;
                        string version = GetLibraryVersion(key);
                        supportNode.Add(new XElement("Version", version));
                    }
                    newClassNode.Add(supportNode);

                    foreach (XElement refEventInterface in itemClass.Element("EventInterfaces").Elements("Ref"))
                    {
                        string key = refEventInterface.Attribute("Key").Value;
                        XElement eventInterface = GetEventInterface(key);
                        foreach (XElement itemEventMethod in eventInterface.Element("Methods").Elements("Method"))
	                    {
                            foreach (XElement itemEvent in itemEventMethod.Elements("Parameters"))
	                        {
                                XElement addEventMethod = new XElement("Event", new XAttribute("Name", "add_" + itemEventMethod.Attribute("Name").Value));
                                XElement addSupportNode = new XElement("SupportByVersion");
                                foreach (XElement itemRefLib in itemEvent.Element("RefLibraries").Elements("Ref"))
                                {
                                    string addKey = itemRefLib.Attribute("Key").Value;
                                    string version = GetLibraryVersion(addKey);
                                    addSupportNode.Add(new XElement("Version", version));
                                }
                                addEventMethod.Add(supportNode);

                                XElement revEventMethod = new XElement("Event", new XAttribute("Name", "remove_" + itemEventMethod.Attribute("Name").Value));
                                XElement revSupportNode = new XElement("SupportByVersion");
                                foreach (XElement itemRefLib in itemEvent.Element("RefLibraries").Elements("Ref"))
                                {
                                    string removeKey = itemRefLib.Attribute("Key").Value;
                                    string version = GetLibraryVersion(removeKey);
                                    revSupportNode.Add(new XElement("Version", version));
                                }
                                revEventMethod.Add(revSupportNode);

                                newClassNode.Element("Events").Add(addEventMethod);
                                newClassNode.Element("Events").Add(revEventMethod);
                            }
	                    }
                    }
                }
 
                string fullFilePath = System.IO.Path.Combine(_settings.Folder, projectItem.Attribute("Name").Value + ".Description.xml");
                newDocument.Save(fullFilePath);
            }
        }

        #endregion
        
        #region ICodeGenerator Member

        public string Name
        {
            get { return "Docu"; }
        }

        public string Description
        {
            get { return "Creates XML Files for DeveloperToolbox Office Compatibility"; }
        }

        public Version Version
        {
            get
            {
                return new Version("1.0");
            }
        }

        public IProgress<string> Progress { get; set; }

        public System.Windows.Forms.DialogResult ShowConfigDialog(System.Windows.Forms.Control parentDialog)
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

        public Task<TimeSpan> Generate(XDocument document, CancellationToken token)
        {
            _document = document;
            return Task.Run(() =>
                {
                    var sw = Stopwatch.StartNew();
                    this._job_DoWork();
                    sw.Stop();

                    return sw.Elapsed;
                }, token);
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

        #endregion
    }
}
