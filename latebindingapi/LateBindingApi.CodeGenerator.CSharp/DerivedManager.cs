using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal class DerivedManager
    {
        CSharpGenerator _parent;
        XDocument _document;
        XDocument _derived;

        internal DerivedManager(CSharpGenerator parent, XDocument document)
        {
            _parent = parent;
            _document = document;   
        }

        public void ScanForDerived()
        {
            _derived = new XDocument();
            _derived.Add(new XElement("Document"));
            
            ScanForDerived("DispatchInterfaces", "Interface");
            ScanForDerived("Interfaces", "Interface");
            ScanForDerived("CoClasses", "CoClass");
        }

        private void ScanForDerived(string elements, string element)
        {

            var interfaces = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project").
                                 Elements(elements).Elements(element)
                              select a);

            foreach (XElement itemFace in interfaces)
            {
                foreach (XElement itemRef in itemFace.Element("Inherited").Elements("Ref"))
                {
                    string key = itemRef.Attribute("Key").Value;
                    XElement face = CSharpGenerator.GetInterfaceOrClassFromKey(key);
                    AddType(face);
                }
            }
        }

        private void AddType(XElement type)
        {
            string id = type.Attribute("Key").Value;
            XElement node = (from a in _derived.Element("Document").Elements("Type")
                             where a.Attribute("Key").Value.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();
            if (null == node)
            {
                string name = type.Attribute("Name").Value;
                _derived.Element("Document").Add(new XElement("Type", new XAttribute("Name", name), new XAttribute("Key", id)));
            }
        }

        public bool IsDerivedReturnValue(XElement returnValue)
        {
            string typeKey = returnValue.Attribute("TypeKey").Value;
            if (string.IsNullOrEmpty(typeKey))
                return false;

            XElement interfaceNode = CSharpGenerator.GetInterfaceOrClassFromKey(typeKey);
            string id = interfaceNode.Attribute("Key").Value;
            return IsDerived(id);
        }

        public bool IsDerived(string id)
        {
            XElement node = (from a in _derived.Element("Document").Elements("Type")
                             where a.Attribute("Key").Value.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();
            return (node != null);
        }
    }
}
