using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    class InheritedInterfaceManager
    { 
        CSharpGenerator _parent;
        XDocument _document;

        internal InheritedInterfaceManager(CSharpGenerator parent, XDocument document)
        {
            _parent = parent;
            _document = document;   
        }

        private void UpdateSupportVersionInfo(XElement entity, List<XElement> belowInterfaces, bool isProperty)
        {
            List<XElement> matchList = new List<XElement>();

            string targetName = entity.Attribute("Name").Value;
            foreach (XElement itemParameters in entity.Elements("Parameters"))
            {
                matchList.Clear();

                int countOfParams = itemParameters.Elements("Parameter").Count();

                foreach (XElement belowInterface in belowInterfaces)
                {
                    if (isProperty)
                    {
                        var properties = (from a in belowInterface.Element("Properties").Elements("Property")
                                          where a.Attribute("Name").Value.Equals(targetName, StringComparison.InvariantCultureIgnoreCase)
                                          select a);
                        foreach (XElement item in properties)
                        {
                            var parametersNodes = (from a in item.Elements("Parameters") where a.Elements("Parameter").Count() == countOfParams select a);

                            foreach (var targetParams in parametersNodes)
                            {
                                matchList.Add(targetParams);
                            }
                        }
                    }
                    else
                    {
                        var properties = (from a in belowInterface.Element("Methods").Elements("Method")
                                          where a.Attribute("Name").Value.Equals(targetName, StringComparison.InvariantCultureIgnoreCase)
                                          select a);
                        foreach (XElement item in properties)
                        {
                            var parametersNodes = (from a in item.Elements("Parameters") where a.Elements("Parameter").Count() == countOfParams select a);

                            foreach (var targetParams in parametersNodes)
                            {
                                matchList.Add(targetParams);
                            }
                        }
                    }
                }

                List<XElement> refMatches = GetRefList(matchList);
                foreach (XElement itemRef in refMatches)
                {
                    bool found = false;
                    foreach (XElement item in itemParameters.Element("RefLibraries").Elements("Ref"))
                    {
                        if (itemRef.Attribute("Key").Value == item.Attribute("Key").Value)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        itemParameters.Element("RefLibraries").Add(new XElement("Ref", new XAttribute("Key", itemRef.Attribute("Key").Value)));
                    }
                }

            }

   
        }

        List<XElement> GetRefList(List<XElement> matchList)
        {
            List<XElement> refList = new List<XElement>();

            foreach (XElement matchParameters in matchList)
            {
                List<XElement> newRefList = new List<XElement>();
                foreach (XElement refItem in matchParameters.Element("RefLibraries").Elements("Ref"))
                {
                    bool found = false;
                    foreach (XElement item in refList)
                    {
                        if (item.Attribute("Key").Value == refItem.Attribute("Key").Value)
                        {
                            found = true;
                            break;
                        }
                    }
                    if(!found)
                        refList.Add(refItem);
                }
            }

            return refList;

        }

        private void UpdateSupportByVersionInformation(XElement faceNode, List<XElement> refBelowFaces)
        {
            List<XElement> belowInterfaces = new List<XElement>();
            foreach (XElement item in refBelowFaces)
            {
                XElement face = CSharpGenerator.GetInterfaceOrClassFromKey(item.Attribute("Key").Value);
                belowInterfaces.Add(face);
            }

            foreach (XElement property in faceNode.Element("Properties").Elements("Property"))
                UpdateSupportVersionInfo(property, belowInterfaces, true);


            foreach (XElement property in faceNode.Element("Methods").Elements("Method"))
                UpdateSupportVersionInfo(property, belowInterfaces, false);
        }

        public void ValidateMultipleCoClassInherited()
        {
            var projects = _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project");
            foreach (XElement project in projects)
            {
                if (project.Attribute("Ignore").Value.Equals("true",StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var coClasses = project.Element("CoClasses").Elements("CoClass");

                foreach (XElement coClass in coClasses)
                {
                    if (coClass.Element("Inherited").Elements("Ref").Count() > 0)
                    {
                        List<XElement> list = new List<XElement>();
                        foreach (XElement item in coClass.Element("Inherited").Elements("Ref"))
                            list.Add(item);

                        for (int i = list.Count - 1; i > 0; i--)
                        {
                            XElement item = list[i];
                            XElement itemInherit = list[i - 1];

                            XElement face = CSharpGenerator.GetInterfaceOrClassFromKey(item.Attribute("Key").Value);
                            XElement faceInherit = CSharpGenerator.GetInterfaceOrClassFromKey(itemInherit.Attribute("Key").Value);

                            int faceEnitityCount = face.Element("Properties").Elements("Property").Count() + face.Element("Methods").Elements("Method").Count();
                            int faceInheriEnitityCount = faceInherit.Element("Properties").Elements("Property").Count() + faceInherit.Element("Methods").Elements("Method").Count();

                            if ((faceEnitityCount > 0 && faceInheriEnitityCount > 0) && (face.Attribute("IsEventInterface").Value != "true" && faceInherit.Attribute("IsEventInterface").Value != "true"))
                            {
                                if (face.Element("Inherited").Elements("Ref").Count() == faceInherit.Element("Inherited").Elements("Ref").Count())
                                {
                                    List<XElement> listFace = new List<XElement>();
                                    List<XElement> listInherit = new List<XElement>();

                                    foreach (XElement refKey in face.Element("Inherited").Elements("Ref"))
                                        listFace.Add(refKey);
                                    foreach (XElement refKey in faceInherit.Element("Inherited").Elements("Ref"))
                                        listInherit.Add(refKey);

                                    bool conditionOkay = true;
                                    for (int y = 0; y < listFace.Count; y++)
                                    {
                                        string key1 = listFace[y].Attribute("Key").Value;
                                        string key2 = listInherit[y].Attribute("Key").Value;
                                        if (key1 != key2)
                                        {
                                            conditionOkay = false;
                                            break;
                                        }
                                    }

                                    if (conditionOkay)
                                    {
                                        List<XElement> listBelowFaces = new List<XElement>();
                                        foreach (XElement itemNode in list)
                                        {
                                            if (itemNode.Attribute("Key").Value == face.Attribute("Key").Value)
                                                break;
                                            listBelowFaces.Add(itemNode);
                                        }
                                        UpdateSupportByVersionInformation(face, listBelowFaces);
                                        Console.WriteLine("Interface {0} inherites now {1}", face.Attribute("Name").Value, faceInherit.Attribute("Name").Value);
                                        face.Element("Inherited").RemoveNodes();
                                        XElement newRefNode = new XElement("Ref", new XAttribute("Key", faceInherit.Attribute("Key").Value));
                                        face.Element("Inherited").Add(newRefNode);
                                    }
                                }
                            }
                             
                        }

                    }

                }
            }
        }

    }
}
