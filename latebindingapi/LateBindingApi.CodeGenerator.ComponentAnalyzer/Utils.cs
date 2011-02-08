using System;
using System.Net;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Collections.Generic;
using System.Text;
using TLI;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer
{
    internal class Utils
    {   
       
        #region Ressource Methods

        internal static System.IO.Stream ReadStreamFromRessource(string fileName)
        {
            System.IO.Stream ressourceStream = null;
            try
            {
                string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                fileName = assemblyName + "." + fileName;
                ressourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
                if (ressourceStream == null)
                    throw (new System.IO.IOException("Error accessing resource Stream."));

                return ressourceStream;
            }
            catch (Exception exception)
            {
                throw (exception);
            }
        }

        internal static string ReadTextFileFromRessource(string fileName)
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

        #region Xml Methods

        public static String PrintXML(XmlNode node)
        {
            string result = "";
            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode); 
            writer.Formatting = Formatting.Indented;
            node.WriteContentTo(writer);        
            writer.Flush();         
            mStream.Flush();        
            mStream.Position = 0;       
            StreamReader sReader = new StreamReader(mStream);   
            String FormattedXML = sReader.ReadToEnd();
            result = FormattedXML;
            mStream.Close();
            writer.Close();
            return result;
        } 

        public static void AddSchema(XmlDocument document, Stream xsdContent)
        {
            document.Schemas.Add(null, XmlReader.Create(xsdContent));
        }

        public static void AddAttachedSchema(XmlDocument document)
        {
            if (false == HasSchemaLocation(document))
                throw (new XmlException("no schmema in document"));

            Stream schemaStream = GetSchemaStream(document);
            document.Schemas.Add(null, XmlReader.Create(schemaStream));
            schemaStream.Close();
        }

        public static bool HasSchemaLocation(XmlDocument document)
        {
            XmlNode declarationNode = document.DocumentElement;
            if (null == declarationNode)
                throw (new ArgumentException("document contains no root node."));

            foreach (XmlAttribute itemAttribute in declarationNode.Attributes)
            {
                if (true == itemAttribute.Name.EndsWith("schemaLocation", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetSchemaPath(XmlDocument document)
        {
            string returnValue = "";

            XmlNode declarationNode = document.DocumentElement;
            if (null == declarationNode)
                throw (new ArgumentException("document contains no root node."));

            foreach (XmlAttribute itemAttribute in declarationNode.Attributes)
            {
                if (true == itemAttribute.Name.EndsWith("schemaLocation", StringComparison.InvariantCultureIgnoreCase))
                {
                    int spacePostion = itemAttribute.InnerText.IndexOf(" ");
                    returnValue = itemAttribute.InnerText.Substring(spacePostion + 1);
                    break;
                }
            }

            return returnValue;
        }

        public static Stream GetSchemaStream(XmlDocument document)
        {
            string path = GetSchemaPath(document);
            if (true == path.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
            {
                WebRequest request = HttpWebRequest.Create(path);
                WebResponse response = request.GetResponse();
                return response.GetResponseStream();
            }
            else
            {
                FileStream xmlFile = new FileStream(path, FileMode.Open);
                return xmlFile;
            }
        }

        public static Stream GetSchemaStream(string path)
        {
            if (true == path.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
            {
                WebRequest request = HttpWebRequest.Create(path);
                WebResponse response = request.GetResponse();
                return response.GetResponseStream();
            }
            else
            {
                FileStream xmlFile = new FileStream(path, FileMode.Open);
                return xmlFile;
            }
        }

        public static string FirstCharUpper(string expression)
        {
            return expression.Substring(0, 1).ToUpper() + expression.Substring(1);
        }

        public static XmlAttribute AddAtrributeToNode(XmlNode node, string name, string value)
        {
            XmlAttribute attribute = node.OwnerDocument.CreateAttribute(name);
            attribute.InnerText = value;
            node.Attributes.Append(attribute);
            return attribute;
        }

        public static XmlNode AddChildToNode(XmlNode node, string childNodeName)
        {
            XmlNode newNode = node.OwnerDocument.CreateElement(childNodeName);
            node.AppendChild(newNode);
            return newNode;
        }

        public static XmlNode AddChildToNode(XmlNode node, string childNodeName, string innerText)
        {
            XmlNode newNode = node.OwnerDocument.CreateElement(childNodeName);
            newNode.InnerText = innerText;
            node.AppendChild(newNode);
            return newNode;
        }

        public static string NewEncodedGuid()
        {
            return XmlConvert.EncodeName(Guid.NewGuid().ToString());
        }

        public static string GetTypeLibDescription(TLI.TypeLibInfo libInfo)
        {
            string majorVersion = libInfo.MajorVersion.ToString();
            string minorVersion = libInfo.MinorVersion.ToString();
            string guid = libInfo.GUID.ToString();

            string description = "<NoDescription>";
            string version = string.Format("{0}.{1}", majorVersion, minorVersion);
            string regKey = string.Format("TypeLib\\{0}", guid);
            TypeLibRegistryKey key = new TypeLibRegistryKey(regKey);
            foreach (TypeLibRegistryKey itemKey in key.Keys)
            {
                if ((itemKey.Name == version) && (itemKey.Entries.Count > 0))
                {
                    description = (string)itemKey.Entries[0].Value;
                    break;
                }
            }

            return description;
        }

        #endregion

        public static string DecodeGuid(string guid)
        {
            return XmlConvert.DecodeName(guid.Replace("{", "").Replace("}", ""));
        }

        public static string EncodeGuid(string guid)
        {
            return XmlConvert.EncodeName(guid.Replace("{", "").Replace("}", ""));
        }

        public static string ValidatePath(string filePath)
        {
            if (filePath == null)
                return "";

            string validateFilePath = filePath.Replace("\0", "");
            return validateFilePath;
        }

        #region XPath Methods

        public static string ChildNodeParamExpression(params string[] values)
        {
            string returnvalue = "[";
            for (int i = 0; i < values.Length; i += 2)
            {
                string name = values[i];
                string value = values[i + 1];
                string line = name + "='" + value + "'";

                if ((i + 2) < values.Length)
                    line += " and ";

                returnvalue += line;
            }
            returnvalue += "]";

            return returnvalue;
        }

        public static string AttributeParamExpression(params string[] values)
        {
            string returnvalue = "[";
            for (int i = 0; i < values.Length; i += 2)
            {
                string name = values[i];
                string value = values[i + 1];
                string line = "@" + name + "='" + value + "'";

                if ((i + 2) < values.Length)
                    line += " and ";

                returnvalue += line;
            }
            returnvalue += "]";

            return returnvalue;
        }

        #endregion

        #region Dependency Methods

        public static XmlDocument DetectDependencies(string[] fileNames)
        {
            TLIApplication typeLibApplication = new TLIApplication();

            XmlDocument returnDocument = new XmlDocument();
            returnDocument.AppendChild(returnDocument.CreateElement("Root"));

            foreach (string fileName in fileNames)
            {
                DetectDependencies(typeLibApplication, fileName, returnDocument);
            }

            Marshal.ReleaseComObject(typeLibApplication); 

            return returnDocument; 
        }

        private static void DetectDependencies(TLIApplication typeLibApplication, string fileName, XmlDocument returnDocument)
        {
            TypeLibInfo libInfo = typeLibApplication.TypeLibInfoFromFile(fileName);

            CoClasses classInfos = libInfo.CoClasses;
            foreach (CoClassInfo itemInfo in classInfos)
            {
                DetectMembers(typeLibApplication, fileName, returnDocument, itemInfo.DefaultInterface.Members);
                Marshal.ReleaseComObject(itemInfo);
            }
            Marshal.ReleaseComObject(classInfos);

            Interfaces faceInfos = libInfo.Interfaces;
            foreach (InterfaceInfo itemInfo in faceInfos)
            {
                DetectMembers(typeLibApplication, fileName, returnDocument, itemInfo.Members);
                Marshal.ReleaseComObject(itemInfo);
            }
            Marshal.ReleaseComObject(faceInfos);

            Marshal.ReleaseComObject(libInfo);
        }

        private static void DetectMembers(TLIApplication typeLibApplication, string fileName, XmlDocument returnDocument, Members members)
        {
            foreach (MemberInfo itemMember in members)
            {
                VarTypeInfo typeInfo = itemMember.ReturnType;
                if ((typeInfo != null) && (true == typeInfo.IsExternalType))
                {

                    XmlNode fileNode = null;
                    foreach (XmlNode itemNode in returnDocument.FirstChild.ChildNodes)
                    {
                        if (itemNode.Attributes["File"].InnerText == typeInfo.TypeLibInfoExternal.ContainingFile)
                        {
                            fileNode = itemNode;
                            break;
                        }
                    }
                    if (null == fileNode)
                    {
                        fileNode = returnDocument.CreateElement("Component");
                        XmlAttribute attribute = returnDocument.CreateAttribute("File");
                        attribute.InnerText = typeInfo.TypeLibInfoExternal.ContainingFile;
                        fileNode.Attributes.Append(attribute);
                        attribute = returnDocument.CreateAttribute("Name");
                        attribute.InnerText = typeInfo.TypeLibInfoExternal.Name;
                        fileNode.Attributes.Append(attribute);
                        returnDocument.FirstChild.AppendChild(fileNode);
                        AddDependencyComponentNode(fileNode, fileName);
                        DetectDependencies(typeLibApplication, typeInfo.TypeLibInfoExternal.ContainingFile, returnDocument);
                    }
                    else
                    {
                        AddDependencyComponentNode(fileNode, fileName);
                    }
                }
                if (null != typeInfo)
                    Marshal.ReleaseComObject(typeInfo);
            }
        }

        private static void AddDependencyComponentNode(XmlNode fileNode, string fileName)
        {
            foreach (XmlNode item in fileNode.ChildNodes)
            {
                if (item.Attributes["File"].InnerText == fileName)
                    return;
            }

            XmlNode childNode = fileNode.OwnerDocument.CreateElement("Component");
            XmlAttribute attribute = fileNode.OwnerDocument.CreateAttribute("File");
            attribute.InnerText = fileName;
            childNode.Attributes.Append(attribute);
            fileNode.AppendChild(childNode);
        }

        private static Members GetMembers(TLI.TypeInfo itemInfo)
        {
            try
            {
                /*
                 * bad stuff of course :-( not all members are accessable
                 */
                return itemInfo.Members;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
