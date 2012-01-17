using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.VB
{
    class CustomMethodManager
    {
        VBGenerator _parent;
        XDocument _document;
        XDocument _derived;

        internal CustomMethodManager(VBGenerator parent, XDocument document)
        {
            _parent = parent;
            _document = document;
        }

        public void ScanForOptionalMethods()
        {
            _derived = new XDocument();
            _derived.Add(new XElement("Document"));

            ScanForDerived("DispatchInterfaces", "Interface");
            ScanForDerived("Interfaces", "Interface");
        }

        private XElement GetMethodOverload(XElement itemMethod, IEnumerable<XElement> listParameters, int paramsCount)
        {
            foreach (XElement itemParameters in itemMethod.Elements("Parameters"))
            {
                if (itemParameters.Elements("Parameter").Count() == paramsCount)
                    return itemParameters;
            }

            foreach (XElement itemParameters in listParameters)
            {
                if (itemParameters.Elements("Parameter").Count() == paramsCount)
                    return itemParameters;
            }
            return null;
        }

        private XElement CloneParametersNode(XElement itemParameters, int parametersCount)
        {
            XElement itemMethod = itemParameters.Parent;
            XElement newParameters = new XElement("Parameters", new XAttribute("IsCustom","true"));

            if (itemParameters.Element("ReturnValue") != null)
            {
                newParameters.Add(new XElement("ReturnValue"));
                foreach (XAttribute item in itemParameters.Element("ReturnValue").Attributes())
                    newParameters.Element("ReturnValue").Add(new XAttribute(item.Name, item.Value));
            }

            newParameters.Add(new XElement("RefLibraries"));
            foreach (XElement item in itemParameters.Element("RefLibraries").Elements("Ref"))
            {
                XElement newRef = new XElement(item.Name, item.Value);
                newRef.Add(new XAttribute("Key", item.Attribute("Key").Value));
                newParameters.Element("RefLibraries").Add(newRef);
            }

            int i = 1;
            foreach (XElement item in itemParameters.Elements("Parameter"))
            {
                if (i > parametersCount)
                    break;
                XElement newParameter = new XElement("Parameter");
                foreach (XAttribute itemAttribute in item.Attributes())
                    newParameter.Add(new XAttribute(itemAttribute.Name, itemAttribute.Value));
                newParameters.Add(newParameter);
                i++;
            }

            return newParameters;
        }

        private void ScanForDerived(string elements, string element)
        {

            var interfaces = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project").
                                 Elements(elements).Elements(element)
                              select a);

            foreach (XElement itemFace in interfaces)
            {
                foreach (XElement itemMethod in itemFace.Element("Methods").Elements("Method"))
                {
                    if (itemMethod.Attribute("Name").Value == "Item")
                        continue;

                    List<XElement> newParameters = new List<XElement>();
                    IEnumerable<XElement> listParameters = itemMethod.Elements("Parameters");
                    foreach (XElement itemParameters in listParameters)
                    {
                        IEnumerable<XElement> nonOptionalParamNodes = (from a in itemParameters.Elements("Parameter")
                                                                    where a.Attribute("IsOptional").Value.Equals("false", StringComparison.InvariantCultureIgnoreCase)
                                                                    select a);
                        int nonOptionalsCount = nonOptionalParamNodes.Count();

                        IEnumerable<XElement> optionalParamNodes = (from a in itemParameters.Elements("Parameter")
                                         where a.Attribute("IsOptional").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase)
                                         select a);
                        int optionalsCount = optionalParamNodes.Count();

                        if (optionalsCount > 0)
                        {
                            for (int i = nonOptionalsCount; i < (nonOptionalsCount+optionalsCount); i++)
                            {
                                XElement existingMethodOverload = GetMethodOverload(itemMethod, newParameters, i);
                                if (null == existingMethodOverload)
                                { 
                                    XElement newParameter = CloneParametersNode(itemParameters, i);
                                    newParameters.Add(newParameter);
                                }
                            }
                        }
                    }

                    foreach (XElement item in newParameters)
                        itemMethod.Add(item);
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
                string type = (node.Element("Parameters").Element("ReturnValue").Attribute("Type").Value) ;
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

        private XElement HasItem(XElement itemFace)
        {
            XElement node = (from a in itemFace.Element("Properties").Elements("Property")
                             where a.Attribute("Name").Value.Equals("Item", StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();
            if (null != node)
                return node;

            node = (from a in itemFace.Element("Methods").Elements("Method")
                    where a.Attribute("Name").Value.Equals("Item", StringComparison.InvariantCultureIgnoreCase)
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
                XElement interfaceNode = VBGenerator.GetInterfaceOrClassFromKey(typeKey);
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
