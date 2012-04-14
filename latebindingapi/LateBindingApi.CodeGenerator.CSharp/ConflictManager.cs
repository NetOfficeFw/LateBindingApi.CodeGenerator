using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    class ConflictManager
    {
        CSharpGenerator _parent;
        XDocument _document;
        XDocument _derived;

        internal ConflictManager(CSharpGenerator parent, XDocument document)
        {
            _parent = parent;
            _document = document;
        }

        public void ScanForConflicts()
        {
            _derived = new XDocument();
            _derived.Add(new XElement("Document"));

            ScanForNameConflicts("DispatchInterfaces", "Interface");
            ScanForNameConflicts("Interfaces", "Interface");

            ScanForOptionalConflicts("DispatchInterfaces", "Interface");
            ScanForOptionalConflicts("Interfaces", "Interface");
        }

        private void AddConflict(XElement element, string conflict)
        {
            XAttribute conflictAttribute = null;
            foreach (XAttribute attribute in element.Attributes())
            {
                if (attribute.Name == conflict)
                {
                    conflictAttribute = attribute;
                    break;
                }
            }
            if (null == conflictAttribute)
            {
                conflictAttribute = new XAttribute(conflict, "true");
                element.Add(conflictAttribute);
            }
            else
            {
                conflictAttribute.Value = "true";
            }
        }

        private void ScanForOptionalConflicts(string elements, string element)
        {
            var interfaces = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project").
                                 Elements(elements).Elements(element)
                              select a);

            foreach (XElement itemFace in interfaces)
            {
                foreach (XElement itemMethod in itemFace.Element("Properties").Elements("Property"))
                {
                    IEnumerable<XElement> listParameters = itemMethod.Elements("Parameters");
                    foreach (XElement itemParameters in listParameters)
                    {                        
                        int paramCountWithoutOptionals = ParameterApi.GetParamsCount(itemParameters, false);
                        int paramCountWithOptionals = ParameterApi.GetParamsCount(itemParameters, true);
                        if (paramCountWithoutOptionals == 0 && paramCountWithOptionals > 0)
                        {
                            Console.WriteLine("Optional Conflict found: " + itemMethod.Attribute("Name").Value);

                            AddConflict(itemMethod, "IsOptionalConflict");
                            AddConflict(itemParameters, "IsOptionalConflict");
                            AddConflict(itemFace, "IsOptionalConflict");
                        }
                    }
                }
            }

        }
       
        private void ScanForNameConflicts(string elements, string element)
        {
            var interfaces = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project").
                                 Elements(elements).Elements(element)
                              select a);

            foreach (XElement itemFace in interfaces)
            {
                foreach (XElement itemMethod in itemFace.Element("Methods").Elements("Method"))
                {
                    if (itemMethod.Attribute("Name").Value.Equals(itemFace.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Console.WriteLine("Name Conflict found: " + itemMethod.Attribute("Name").Value);
                        AddConflict(itemMethod, "IsNameConflict");
                        AddConflict(itemFace, "IsNameConflict");

                        IEnumerable<XElement> listParameters = itemMethod.Elements("Parameters");
                        foreach (XElement itemParameters in listParameters)
                        {
                                Console.WriteLine("Name Conflict found: " + itemMethod.Attribute("Name").Value);
                                AddConflict(itemParameters, "IsNameConflict");
                        }
                    }
                }
            }

            foreach (XElement itemFace in interfaces)
            {
                foreach (XElement itemMethod in itemFace.Element("Properties").Elements("Property"))
                {
                    if (itemMethod.Attribute("Name").Value.Equals(itemFace.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Console.WriteLine("Name Conflict found: " + itemMethod.Attribute("Name").Value);
                        AddConflict(itemMethod, "IsNameConflict");
                        AddConflict(itemFace, "IsNameConflict");

                        IEnumerable<XElement> listParameters = itemMethod.Elements("Parameters");
                        foreach (XElement itemParameters in listParameters)
                        {
                            Console.WriteLine("Name Conflict found: " + itemMethod.Attribute("Name").Value);
                            AddConflict(itemParameters, "IsNameConflict");
                        }
                    }
                }
            }

        }
        
        private XElement HasCount(XElement itemFace)
        {
            XElement node = (from a in itemFace.Element("Properties").Elements("Property")
                             where a.Attribute("Name").Value.Equals("Count", StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();
            if (null != node)
            {
                string type = (node.Element("Parameters").Element("ReturnValue").Attribute("Type").Value);
                if ("Int32" == type)
                    return node;
            }

            node = (from a in itemFace.Element("Methods").Elements("Method")
                    where a.Attribute("Name").Value.Equals("Count", StringComparison.InvariantCultureIgnoreCase)
                    select a).FirstOrDefault();
            if (null != node)
                return node;

            return null;
        }

        private XElement HasEnum(XElement itemFace)
        {
            XElement node = (from a in itemFace.Element("Properties").Elements("Property")
                             where a.Attribute("Name").Value.Equals("_NewEnum", StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();
            if (null != node)
                return node;

            node = (from a in itemFace.Element("Methods").Elements("Method")
                    where a.Attribute("Name").Value.Equals("_NewEnum", StringComparison.InvariantCultureIgnoreCase)
                    select a).FirstOrDefault();
            if (null != node)
                return node;

            return null;
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

        private XElement GetTypeByName(XElement projectNode, string name)
        {

            XElement node = (from a in projectNode.Element("DispatchInterfaces").Elements("Interface")
                             where a.Attribute("Name").Value.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();

            if (null != node)
                return node;

            node = (from a in projectNode.Element("Interfaces").Elements("Interface")
                    where a.Attribute("Name").Value.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                    select a).FirstOrDefault();

            if (null != node)
                return node;

            node = (from a in projectNode.Element("CoClasses").Elements("CoClass")
                    where a.Attribute("Name").Value.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                    select a).FirstOrDefault();

            if (null != node)
                return node;

            throw new Exception("name not found " + name);
        }

        private bool HasAttriute(XElement node, string attributeName)
        {
            foreach (XAttribute item in node.Attributes())
            {
                if (item.Name == attributeName)
                    return true;
            }

            return false;
        }

        private XElement GetProjectNode(XElement returnValue)
        {
            XElement projectNode = returnValue.Parent;
            while (projectNode.Name != "Project")
                projectNode = projectNode.Parent;

            return projectNode;
        }

        public bool IsDerivedReturnValue(XElement returnValue)
        {
            string typeKey = returnValue.Attribute("TypeKey").Value;
            if (string.IsNullOrEmpty(typeKey))
            {
                if (returnValue.Attribute("IsExternal").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    return false;

                XElement projectNode = GetProjectNode(returnValue);
                XElement interfaceNode = GetTypeByName(projectNode, returnValue.Attribute("Type").Value as string);
                string id = interfaceNode.Attribute("Key").Value;
                return IsDerived(id);
            }
            else
            {
                XElement interfaceNode = CSharpGenerator.GetInterfaceOrClassFromKey(typeKey);
                string id = interfaceNode.Attribute("Key").Value;
                return IsDerived(id);
            }
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
