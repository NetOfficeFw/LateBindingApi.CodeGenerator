using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath; 
using System.Xml;
using System.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal class DubletteManager
    {
        CSharpGenerator _parent;
        XDocument _document;
        XDocument _dublettes;

        public XDocument DublettesDocument
        {
            get
            {
                return _dublettes;
            }
        }

        internal DubletteManager(CSharpGenerator parent, XDocument document)
        {
            _parent = parent;
            _document = document;   
        }

        public bool IsDuplicatedReturnValue(XElement returnValue)
        {
            string typeKey = returnValue.Attribute("TypeKey").Value;
            if (string.IsNullOrEmpty(typeKey))
                return false;

            XElement interfaceNode = CSharpGenerator.GetInterfaceOrClassFromKey(typeKey); 
            XElement dispNode = interfaceNode.Element("DispIds").Elements("DispId").FirstOrDefault();
            string id = dispNode.Attribute("Id").Value;

            XElement node = (from a in _dublettes.Element("Document").Elements("Interface")
                             where a.Attribute("Id").Value.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();
            return (node != null);        

        }

        public bool IsDuplicated(string id)
        {
            XElement node = (from a in _dublettes.Element("Document").Elements("Interface")
                             where a.Attribute("Id").Value.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                                         select a).FirstOrDefault();
            return (node!=null);        
        }

        public void ScanForDublettes()
        {
            _dublettes = new XDocument();
            _dublettes.Add(new XElement("Document"));

            var interfaces = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project").
                              Elements("DispatchInterfaces").Elements("Interface") select a);
        
            foreach (XElement itemFace in interfaces)
            {              
                foreach(XElement itemId in itemFace.Element("DispIds").Elements("DispId"))
                {
                    string id = itemId.Attribute("Id").Value;
                    List<XElement> dublettesFace = GetDublettes(itemFace, id);
                    foreach (XElement dubItem in dublettesFace)
                    {
                        XElement node = (from a in _dublettes.Element("Document").Elements("Interface")
                                         where a.Attribute("Name").Value.Equals(itemFace.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase)
                                         select a).FirstOrDefault();
                        if (null == node)
                        {
                            _dublettes.Element("Document").Add(new XElement("Interface", 
                                new XAttribute("Name", itemFace.Attribute("Name").Value),
                                new XAttribute("Host", GetProjectNode(itemFace).Attribute("Name").Value),
                                new XAttribute("Id", id),
                                new XAttribute("Type", "Interface")                                
                                ));

                            node = (from a in _dublettes.Element("Document").Elements("Interface")
                                    where a.Attribute("Name").Value.Equals(itemFace.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase)
                                    select a).FirstOrDefault();
                                                    
                        }

                        node.Add( new XElement("Same", 
                                  new XAttribute("Name", dubItem.Parent.Parent.Attribute("Name").Value),
                                  new XAttribute("Host", GetProjectNode(dubItem).Attribute("Name").Value)
                                  ));                       
                    }
                }
            }
        }

        private XElement GetProjectNode(XElement subNode)
        {
            XElement node = subNode.Parent;
            while (node.Name != "Project")
                 node = node.Parent;

            return node;
         }

        private List<XElement> GetDublettes(XElement node, string id)
        {
            string projectName = GetProjectNode(node).Attribute("Name").Value;

            List<XElement> result = new List<XElement>();
            var dublettesFace = _document.XPathSelectElements("/LateBindingApi.CodeGenerator.Document/Solution/Projects/Project/" + "Interfaces" + "/Interface/DispIds/DispId[@Id='" + id + "']");
            foreach (XElement item in dublettesFace)
            {
                string itemProjectName = GetProjectNode(item).Attribute("Name").Value;
                if (!projectName.Equals(itemProjectName, StringComparison.InvariantCultureIgnoreCase))
                    result.Add(item);
            }

            dublettesFace = _document.XPathSelectElements("/LateBindingApi.CodeGenerator.Document/Solution/Projects/Project/" + "DispatchInterfaces" + "/Interface/DispIds/DispId[@Id='" + id + "']");
            foreach (XElement item in dublettesFace)
            {
                string itemProjectName = GetProjectNode(item).Attribute("Name").Value;
                if (!projectName.Equals(itemProjectName, StringComparison.InvariantCultureIgnoreCase))
                    result.Add(item);
            }

            return result;
        }
    }
}
