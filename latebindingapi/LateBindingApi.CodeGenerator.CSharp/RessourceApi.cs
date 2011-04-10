using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class RessourceApi
    {
        internal static string ReadString(string path)
        {
            string fileName = "LateBindingApi.CodeGenerator.CSharp." + path;

            System.IO.Stream ressourceStream;
            System.IO.StreamReader textStreamReader;
            try
            {
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

        internal static void WriteBinaryToFile(byte[] binary, string fileName)
        {
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
            fs.Write(binary, 0, binary.Length);
            fs.Close();
        }

        internal static byte[] ReadBinaryFromResource(string resourceName)
        {
            resourceName = "LateBindingApi.CodeGenerator.CSharp." + resourceName;

            System.IO.Stream ressourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            byte[] binary = new byte[ressourceStream.Length];
            ressourceStream.Read(binary, 0, binary.Length);
            ressourceStream.Close();
            return binary;
        }
    }
}
