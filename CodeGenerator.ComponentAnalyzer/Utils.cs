using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer
{
    /// <summary>
    /// offers various helper functions
    /// </summary>
    internal static class Utils
    {   
        #region Resource Methods

        /// <summary>
        /// Read text from resource file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static string ReadTextFileFromResource(string resourcePath)
        {
            System.IO.Stream resourceStream = null;
            System.IO.StreamReader textStreamReader = null;
            try
            {
                string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                resourcePath = assemblyName + "." + resourcePath;
                resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                if (resourceStream == null)
                    throw (new System.IO.IOException("Error accessing resource Stream."));

                textStreamReader = new System.IO.StreamReader(resourceStream);
                if (textStreamReader == null)
                    throw (new System.IO.IOException("Error accessing resource File."));

                string text = textStreamReader.ReadToEnd();
                return text;
            }
            catch (Exception exception)
            {
                throw (exception);
            }
            finally
            {
                if(null!=textStreamReader)
                    textStreamReader.Close();
                if (null != resourceStream)
                    resourceStream.Close();
            }
        }
        
        #endregion

        #region Xml Methods

        /// <summary>
        /// Gets friendly inner xml from node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static String GetFormattedXml(XmlNode node)
        {
            string result = "";
            
            MemoryStream memStream = new MemoryStream();
            XmlTextWriter textWriter = new XmlTextWriter(memStream, Encoding.UTF8);
            textWriter.Formatting = Formatting.Indented;
            node.WriteContentTo(textWriter);
            textWriter.Flush();
            memStream.Flush();
            memStream.Position = 0;

            StreamReader sReader = new StreamReader(memStream);   
            String FormattedXML = sReader.ReadToEnd();
            result = FormattedXML;
            memStream.Close();
            textWriter.Close();

            return result;
        }

        #endregion

        #region Guid Methods

        /// <summary>
        /// returns a new xml encoded guid as string
        /// </summary>
        /// <returns></returns>
        public static string NewEncodedGuid()
        {
            return XmlConvert.EncodeName(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// xml encode guid and remove the chars"{}" if exists
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string EncodeGuid(string guid)
        {
            return XmlConvert.EncodeName(guid.Replace("{", "").Replace("}", ""));
        }

        #endregion

        #region IO Methods

        /// <summary>
        /// Removes bad chars from filePath
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string RemoveBadChars(string filePath)
        {
            if (filePath == null)
                return "";

            string validateFilePath = filePath.Replace("\0", "");

            char[] badChars = System.IO.Path.GetInvalidPathChars();
            char replaceChar  = new char();
            
            foreach (char item in badChars)
                validateFilePath = validateFilePath.Replace(item, replaceChar);

            return validateFilePath;
        }

        #endregion
    }
}
