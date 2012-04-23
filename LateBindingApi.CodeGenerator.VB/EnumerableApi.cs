using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.VB
{
    internal static class EnumerableApi
    {
        internal enum EnumeratorType
        {
            NoEnum =0,
            PropertyEnum=1,
            MethodEnum=2
        }

        private static string _proxyEnumerator;
        private static string _nativeEnumerator;
        private static string _fakedEnumerator;

        private static string _proxyEnumeratorT;
        private static string _nativeEnumeratorT;
        private static string _fakedEnumeratorT;

        private static string ProxyEnumerator
        {
            get
            {
                if(null == _proxyEnumerator)
                    _proxyEnumerator = RessourceApi.ReadString("Enumerator.ProxyEnumerator.txt");
                return _proxyEnumerator;
            }
        }

        private static string NativeEnumerator
        {
            get
            {
                if (null == _nativeEnumerator)
                    _nativeEnumerator = RessourceApi.ReadString("Enumerator.NativeEnumerator.txt");
                return _nativeEnumerator;
            }
        }

        private static string FakedEnumerator
        {
            get
            {
                if (null == _fakedEnumerator)
                    _fakedEnumerator = RessourceApi.ReadString("Enumerator.FakedEnumerator.txt");
                return _fakedEnumerator;
            }
        }
        
        private static string ProxyEnumeratorT
        {
            get
            {
                if (null == _proxyEnumeratorT)
                    _proxyEnumeratorT = RessourceApi.ReadString("Enumerator.ProxyEnumeratorT.txt");
                return _proxyEnumeratorT;
            }
        }

        private static string NativeEnumeratorT
        {
            get
            {
                if (null == _nativeEnumeratorT)
                    _nativeEnumeratorT = RessourceApi.ReadString("Enumerator.NativeEnumeratorT.txt");
                return _nativeEnumeratorT;
            }
        }

        private static string FakedEnumeratorT
        {
            get
            {
                if (null == _fakedEnumeratorT)
                    _fakedEnumeratorT = RessourceApi.ReadString("Enumerator.FakedEnumeratorT.txt");
                return _fakedEnumeratorT;
            }
        }
        /// <summary>
        /// returns enumerator node
        /// </summary>
        /// <param name="interfaceNode"></param>
        /// <returns></returns>
        internal static XElement GetEnumNode(XElement interfaceNode)
        {
            XElement enumeratorNode = (from a in interfaceNode.Element("Methods").Elements("Method")
                                       where a.Attribute("Name").Value.Equals("_NewEnum")
                                       select a).FirstOrDefault();
            if (null == enumeratorNode)
            {
                enumeratorNode = (from a in interfaceNode.Element("Properties").Elements("Property")
                                  where a.Attribute("Name").Value.Equals("_NewEnum")
                                  select a).FirstOrDefault();
                if (null != enumeratorNode)
                    return enumeratorNode;
            }
            else
                return enumeratorNode;

            throw (new Exception("Enumerator not exists"));
        }

        /// <summary>
        /// returns info enumerator is property method or doesnt exists
        /// </summary>
        /// <param name="interfaceNode"></param>
        /// <returns></returns>
        internal static EnumeratorType GetEnumeratorType(XElement interfaceNode)
        {
            XElement enumeratorNode = (from a in interfaceNode.Element("Methods").Elements("Method")
                                       where a.Attribute("Name").Value.Equals("_NewEnum")
                                       select a).FirstOrDefault();
            if (null == enumeratorNode)
            {
                enumeratorNode = (from a in interfaceNode.Element("Properties").Elements("Property")
                                  where a.Attribute("Name").Value.Equals("_NewEnum")
                                  select a).FirstOrDefault();
                if (null == enumeratorNode)
                   return EnumeratorType.NoEnum;
                else
                    return EnumeratorType.PropertyEnum; 
            }
            else
                return EnumeratorType.MethodEnum; 
        }
        
        /// <summary>
        /// returns info interface has an IEnumerator
        /// </summary>
        /// <param name="interfaceNode"></param>
        /// <returns></returns>
        internal static bool HasEnumerator(XElement interfaceNode)
        {
             XElement enumeratorNode = (from a in interfaceNode.Element("Methods").Elements("Method")
                                       where a.Attribute("Name").Value.Equals("_NewEnum")
                                       select a).FirstOrDefault();
             if (null == enumeratorNode)
             {
                 enumeratorNode = (from a in interfaceNode.Element("Properties").Elements("Property")
                                   where a.Attribute("Name").Value.Equals("_NewEnum")
                                   select a).FirstOrDefault();
                 if (null == enumeratorNode)
                     return false;
                 else
                     return true;
             }
             else
                 return true;
        }

        /// <summary>
        /// returns info interface has a _Default Item
        /// </summary>
        /// <param name="interfaceNode"></param>
        /// <returns></returns>
        internal static bool HasDefaultItem(XElement interfaceNode)
        {
            return (GetDefaultItemNode(interfaceNode) !=null);
        }

        private static XElement GetDefaultItemNode(XElement itemFace)
        {
            XElement node = null;

            foreach (XElement itemMethod in itemFace.Element("Properties").Elements("Property"))
            {
                node = (from a in itemMethod.Element("DispIds").Elements("DispId")
                        where a.Attribute("Id").Value.Equals("0", StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();
                if (null != node)
                    return itemMethod;
            }

            foreach (XElement itemMethod in itemFace.Element("Methods").Elements("Method"))
            {
                node = (from a in itemMethod.Element("DispIds").Elements("DispId")
                        where a.Attribute("Id").Value.Equals("0", StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();
                if (null != node)
                    return itemMethod;
            }

            return null;
        }

        internal static bool HasCustomAttribute(XElement enumNode)
        {
            foreach (XAttribute item in enumNode.Attributes())
            {
                if ((item.Name == "IsCustom") && (item.Value == "true"))
                    return true;
            }
            return false;
        }

        private static string GetThisReturnType(XElement faceNode, string defaultValue)
        {
            XElement thisNode = GetDefaultItemNode(faceNode);
            if (null == thisNode)
                return defaultValue;
        
            if ("COMVariant" == thisNode.Element("Parameters").Element("ReturnValue").Attribute("Type").Value)
                return "Object";

            string qualifier = GetQualifier(faceNode, thisNode.Element("Parameters").Element("ReturnValue"));
            if (qualifier != "")
                return qualifier + "." + thisNode.Element("Parameters").Element("ReturnValue").Attribute("Type").Value;
            else
                return thisNode.Element("Parameters").Element("ReturnValue").Attribute("Type").Value;
        }

        /// <summary>
        ///  add enumerator code
        /// </summary>
        /// <param name="faceNode"></param>
        /// <param name="content"></param>
        internal static void AddEnumerator(XElement faceNode, ref string content)
        {        
            XElement enumNode = GetEnumNode(faceNode);
            XElement returnType = enumNode.Element("Parameters").Element("ReturnValue");
            string targetReturnType = GetThisReturnType(faceNode, returnType.Attribute("Type").Value);

            string versionSummary = VBGenerator.GetSupportByVersionString("", enumNode);
            string versionAttribute = VBGenerator.GetSupportByVersionAttribute(enumNode); 
            content = content.Replace("%enumerableSpace%", "Imports System.Collections\r\n");

            if (targetReturnType == "COMObject")
                content = content.Replace("%enumerable%", " \r\n Implements IEnumerable(" + "Of Object" + ")");
            else
                content = content.Replace("%enumerable%", " \r\n Implements IEnumerable(Of " + targetReturnType + ")");

            versionSummary = "''' <summary>\r\n" + "\t\t" + "''' " + versionSummary + "\r\n";
            if (HasCustomAttribute(enumNode))
                versionSummary += "\t\t''' This is a custom enumerator from NetOffice\r\n";
            versionSummary += "\t\t''' </summary>\r\n";

            // get enumerator
            string enumString = "";
            if (HasCustomAttribute(enumNode))
            {
                if (targetReturnType.Equals("COMObject", StringComparison.InvariantCultureIgnoreCase))
                {
                    enumString = FakedEnumeratorT.Replace("%version%", versionSummary + "\t\t" + versionAttribute).Replace("%Type%", "object");
                    enumString += FakedEnumerator.Replace("%version%", versionSummary + "\t\t" + versionAttribute);
                }
                else
                {
                    enumString = FakedEnumeratorT.Replace("%version%", versionSummary + "\t\t" + versionAttribute).Replace("%Type%", targetReturnType);
                    enumString += FakedEnumerator.Replace("%version%", versionSummary + "\t\t" + versionAttribute);
                }
            }
            else if("true" == returnType.Attribute("IsComProxy").Value)
            {
                if (targetReturnType.Equals("COMObject", StringComparison.InvariantCultureIgnoreCase))
                {
                    enumString = ProxyEnumeratorT.Replace("%version%", versionSummary + "\t\t" + versionAttribute).Replace("%Type%", "object");
                    enumString += ProxyEnumerator.Replace("%version%", versionSummary + "\t\t" + versionAttribute);
                }
                else
                {
                    enumString = ProxyEnumeratorT.Replace("%version%", versionSummary + "\t\t" + versionAttribute).Replace("%Type%", targetReturnType);
                    enumString += ProxyEnumerator.Replace("%version%", versionSummary + "\t\t" + versionAttribute);
                }
            }
            else
            {
                enumString = NativeEnumeratorT.Replace("%version%", versionSummary + "\t\t" + versionAttribute).Replace("%Type%", targetReturnType);
                enumString += NativeEnumerator.Replace("%version%", versionSummary + "\t\t" + versionAttribute);
            }

            // get call type
            EnumeratorType enumType = GetEnumeratorType(faceNode);
            switch (enumType)
            {
                case EnumeratorType.PropertyEnum:
                    enumString = enumString.Replace("%AsWhat%", "AsProperty");
                    break;
                case EnumeratorType.MethodEnum:
                    enumString = enumString.Replace("%AsWhat%", "AsMethod");
                    break;
                default:
                    throw (new Exception("Interface has no Enumerator"));
            }

            // get type and check array
            string type = returnType.Attribute("Type").Value;
           
            if(returnType.Attribute("IsComProxy").Value == "true")
                 type = "COMObject";

            if("COMVariant" == type)
                type = "object";
        
            if ("true" == returnType.Attribute("IsArray").Value)
                type += "()";

            // get type qualifiers
            if (type != "COMObject")
            { 
                string qualifier = GetQualifier(faceNode, returnType);
                if(qualifier != "")
                    type = qualifier  + type;
            }
            /*
            if(targetReturnType == "object")
                enumString = enumString.Replace("%Type%", targetReturnType);
            else
            */
            enumString = enumString.Replace("%Type%", type);

            content += enumString;
        }

        /// <summary>
        ///  remove enumerator %% marker
        /// </summary>
        /// <param name="content"></param>
        internal static void RemoveEnumeratorMarker(ref string content)
        {
            content = content.Replace("%enumerableSpace%", "");
            content = content.Replace("%enumerable%", "");
        }

        /// <summary>
        /// get qualifier of type
        /// </summary>
        /// <param name="faceNode"></param>
        /// <param name="returnType"></param>
        /// <returns></returns>
        internal static string GetQualifier(XElement faceNode, XElement returnType)
        {
            if(("COMObject" == returnType.Attribute("Type").Value) || ("COMVariant" == returnType.Attribute("Type").Value))
                return "";

            if ("true" == returnType.Attribute("IsEnum").Value)
            {
                if ("true" == returnType.Attribute("IsExternal").Value)
                {
                    string refProjectKey = returnType.Element("ProjectKey").Value;
                    if ("" != refProjectKey)
                    { 
                        XElement projectNode = (from a in returnType.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project")
                                              where a.Attribute("Key").Value.Equals(refProjectKey)
                                              select a).FirstOrDefault();
                        return projectNode.Attribute("Namespace").Value + ".Enums";
                    }
                }
                else
                {
                    return faceNode.Parent.Parent.Attribute("Namespace").Value + ".Enums";
                }
            }
            else if ("true" == returnType.Attribute("IsComProxy").Value)
            {
                if ("true" == returnType.Attribute("IsExternal").Value)
                {
                    string refProjectKey = returnType.Element("ProjectKey").Value;
                    if ("" != refProjectKey)
                    {
                        XElement projectNode = (from a in returnType.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project")
                                                where a.Attribute("Key").Value.Equals(refProjectKey)
                                                select a).FirstOrDefault();
                        return projectNode.Attribute("Namespace").Value;
                    }
                }
                else
                {
                    return  faceNode.Parent.Parent.Attribute("Namespace").Value;
                }
            }

            return "";
        }
    }
}
