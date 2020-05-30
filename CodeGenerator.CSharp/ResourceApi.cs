using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class ResourceApi
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static string ReadString(string path)
        {
            string fileName = "LateBindingApi.CodeGenerator.CSharp." + path;

            System.IO.Stream resourceStream;
            System.IO.StreamReader textStreamReader;
            try
            {
                resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
                if (resourceStream == null)
                    throw (new System.IO.IOException("Error accessing resource Stream."));

                textStreamReader = new System.IO.StreamReader(resourceStream);
                if (textStreamReader == null)
                    throw (new System.IO.IOException("Error accessing resource File."));

                string text = textStreamReader.ReadToEnd();
                resourceStream.Close();
                textStreamReader.Close();
                return text;
            }
            catch (Exception exception)
            {
                throw (exception);
            }
        }

        internal static IEnumerable<ResourceToolInfo> GetToolsForApplication(string applicationName)
        {
            string namespacePrefix = "LateBindingApi.CodeGenerator.CSharp.Tools." + applicationName;
            string namespacePrefixDialogs = "LateBindingApi.CodeGenerator.CSharp.Tools." + applicationName + ".Dialogs";

            var assembly = Assembly.GetExecutingAssembly();
            var allResources = assembly.GetManifestResourceNames();

            var toolsResources = allResources.Where(name =>
                name.StartsWith(namespacePrefix, StringComparison.OrdinalIgnoreCase)
                && name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
                && !name.StartsWith(namespacePrefixDialogs, StringComparison.OrdinalIgnoreCase)
                );

            foreach (var resource in toolsResources)
            {
                var filename = resource.Substring(namespacePrefix.Length + 1);
                using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(resource)))
                {
                    var content = reader.ReadToEnd();

                    var sb = new StringBuilder(filename);
                    int dot;
                    while ((dot = filename.IndexOf('.')) != filename.LastIndexOf('.'))
                    {
                        sb[dot] = '\\';
                        filename = sb.ToString();
                    }

                    var info = new ResourceToolInfo();
                    info.Filename = filename;
                    info.Content = content;

                    yield return info;
                }
            }
        }

        internal static void WriteBinaryToFile(byte[] binary, string fileName)
        {
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
            fs.Write(binary, 0, binary.Length);
            fs.Close();
        }

        internal static byte[] ReadBinaryFromResource(string resourceName)
        {
            resourceName = "LateBindingApi.CodeGenerator.CSharp." + resourceName;

            System.IO.Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            byte[] binary = new byte[resourceStream.Length];
            resourceStream.Read(binary, 0, binary.Length);
            resourceStream.Close();
            return binary;
        }
    }
}
