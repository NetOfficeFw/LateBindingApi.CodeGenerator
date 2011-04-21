using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class AliasApi
    {
        internal static string ConvertTypeDefsToString(XElement projectNode, XElement defsNode)
        {
            string result = "";
            foreach (var item in defsNode.Elements("Alias"))
            {
                result += ConvertAliasToString(projectNode, item);
            }

            return result;
        }

        private static string ConvertAliasToString(XElement projectNode, XElement aliasNode)
        {
            string version = CSharpGenerator.GetSupportByLibraryAttribute(aliasNode) + "\r\n";
            string name = aliasNode.Attribute("Name").Value;
            string intrinsic = aliasNode.Attribute("Intrinsic").Value;
            string line = name + " as " + intrinsic + "\r\n\r\n";
            return version + line;
        }

    }
}
