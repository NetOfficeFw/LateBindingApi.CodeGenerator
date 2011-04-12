using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class EnumerableApi
    {
        internal enum EnumeratorType
        {
            NoEnum =0,
            PropertyEnum=1,
            MethodEnum=2
        }

        private static string _ProxyEnumerator;
        private static string _NativeEnumerator;

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
        internal static EnumeratorType GetEnumType(XElement interfaceNode)
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

        internal static void AddEnumerator(XElement faceNode, ref string content)
        {
            XElement returnType = GetEnumNode(faceNode).Element("Parameters").Element("ReturnValue");

            string versionAttribute = CSharpGenerator.GetSupportByLibraryAttribute(faceNode); 
            content = content.Replace("%enumerableSpace%", "using System.Collections;\r\n");
            content = content.Replace("%enumerable%", " ,IEnumerable");

            if (null == _ProxyEnumerator)
                _ProxyEnumerator = RessourceApi.ReadString("Enumerator.ProxyEnumerator.txt");

            if (null == _NativeEnumerator)
                _NativeEnumerator = RessourceApi.ReadString("Enumerator.NativeEnumerator.txt");

            // get enumerator
            string enumString = "";
            if ("true" == returnType.Attribute("IsComProxy").Value)
                enumString = _ProxyEnumerator.Replace("%version%", versionAttribute);
            else
                enumString = _NativeEnumerator.Replace("%version%", versionAttribute);
          
            // get call type
            EnumeratorType enumType = GetEnumType(faceNode);
            switch (enumType)
            {
                case EnumeratorType.PropertyEnum:
                    enumString = enumString.Replace("%Call%", "PropertyGet");
                    break;
                case EnumeratorType.MethodEnum:
                    enumString = enumString.Replace("%Call%", "MethodReturn");
                    break;
                default:
                    throw (new Exception("Interface has no Enumerator"));
            }

            // get type and check array
            string type = returnType.Attribute("Type").Value;
            if ("true" == returnType.Attribute("IsArray").Value)
                type += "[]";

            // get type optionals
            if ("true" == returnType.Attribute("IsEnum").Value)
            {
                if ("true" == returnType.Attribute("IsExternal").Value)
                {
                    type = "externalprojectnamespace.enums." + type;
                }
                else
                {
                    type = faceNode.Parent.Parent.Attribute("Namespace").Value + ".Enums." + type;
                }
            }
            else if ("true" == returnType.Attribute("IsComProxy").Value)
            {
                if (("COMObject" == returnType.Attribute("Type").Value) || ("COMObject" == returnType.Attribute("Type").Value) )
                {
 
                }
                else if ("true" == returnType.Attribute("IsExternal").Value)
                {
                    type = "externalprojectnamespace." + type;
                }
                else
                {
                    type = faceNode.Parent.Parent.Attribute("Namespace").Value + type;
                }
            }
           
            enumString = enumString.Replace("%Type%", type);

            content += enumString;
        }

        internal static void RemoveEnumeratorMarker(ref string content)
        {
            content = content.Replace("%enumerableSpace%", "");
            content = content.Replace("%enumerable%", "");
        }
    }
}
