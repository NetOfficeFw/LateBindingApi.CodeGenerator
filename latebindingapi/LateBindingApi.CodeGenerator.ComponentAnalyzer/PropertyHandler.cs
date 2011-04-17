using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using TLI;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer
{ 
    /// <summary>
    /// offers add method for properties
    /// </summary>
    internal static class PropertyHandler
    { 
        /// <summary>
        /// create new property node or get existing
        /// </summary>
        /// <param name="itemMember"></param>
        /// <param name="faceNode"></param>
        /// <returns></returns>
        internal static XElement CreatePropertyNode(TLI.MemberInfo itemMember, XElement faceNode)
        {
            var propertiesNode = faceNode.Elements("Properties").FirstOrDefault();

            // check method exists
            var node = (from a in propertiesNode.Elements()
                        where a.Attribute("Name").Value.Equals(itemMember.Name, StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();

            if (null == node)
            {
               
 
                node = new XElement("Property",
                               new XElement("DispIds"),
                               new XElement("RefLibraries"),
                               new XAttribute("Name", itemMember.Name),
                               new XAttribute("InvokeKind", ""),
                               new XAttribute("Key", Utils.NewEncodedGuid()));

                propertiesNode.Add(node);
            }

            // add invoke kind
            InvokeKinds kinds = itemMember.InvokeKind;
            string kindExists = node.Attribute("InvokeKind").Value;
            switch (kinds)
            {
                case InvokeKinds.INVOKE_PROPERTYGET:
                    if ((kindExists != "INVOKE_PROPERTYPUT") && (kindExists != "INVOKE_PROPERTYPUTREF"))
                        node.Attribute("InvokeKind").Value = kinds.ToString();
                    break;
                case InvokeKinds.INVOKE_PROPERTYPUT:
                case InvokeKinds.INVOKE_PROPERTYPUTREF:
                    node.Attribute("InvokeKind").Value = kinds.ToString();
                    break;
                case InvokeKinds.INVOKE_UNKNOWN:
                    node.Attribute("InvokeKind").Value ="INVOKE_PROPERTYPUT";
                    break;
            }

            return node;
        }

        /// <summary>
        /// add property definition(s)
        /// </summary>
        /// <param name="itemMember"></param>
        /// <param name="propertyNode"></param>
        internal static void AddProperty(XElement libraryNode, XElement propertyNode, TLI.MemberInfo itemMember)
        {
            AddParametersToPropertyNode(libraryNode, propertyNode, itemMember);
        }
        
        /// <summary>
        /// add a property definition
        /// </summary>
        /// <param name="libraryNode"></param>
        /// <param name="methodNode"></param>
        /// <param name="itemMember"></param>
        /// <param name="withOptionalParameters"></param>
        internal static void AddParametersToPropertyNode(XElement libraryNode, XElement methodNode, TLI.MemberInfo itemMember)
        {
            // check defintion exists
            XElement parametersNode = GetParametersNode(methodNode, itemMember);
            if (null == parametersNode)
            {
                VarTypeInfo returnTypeInfo = itemMember.ReturnType;
                string returnTypeName = TypeDescriptor.FormattedType(returnTypeInfo, true);
                parametersNode = new XElement("Parameters",
                                    new XElement("ReturnValue",
                                        new XAttribute("Type",       returnTypeName),
                                        new XAttribute("TypeKind",   TypeInfo(returnTypeInfo.TypeInfo)),
                                        new XAttribute("IsComProxy", TypeDescriptor.IsCOMProxy(returnTypeInfo)),
                                        new XAttribute("IsExternal", returnTypeInfo.IsExternalType.ToString().ToLower()),
                                        new XAttribute("IsEnum",     TypeDescriptor.IsEnum(returnTypeInfo).ToString().ToLower()),
                                        new XAttribute("IsArray",    TypeDescriptor.IsArray(returnTypeInfo).ToString().ToLower()),
                                        new XAttribute("IsNative",   TypeDescriptor.IsNative(returnTypeName).ToString().ToLower()),
                                        new XAttribute("TypeKey",    TypeDescriptor.GetTypeKey(libraryNode.Document, returnTypeInfo)),
                                        new XAttribute("ProjectKey", TypeDescriptor.GetProjectKey(libraryNode.Document, returnTypeInfo)),
                                        new XAttribute("LibraryKey", TypeDescriptor.GetLibraryKey(libraryNode.Document, returnTypeInfo))),
                                    new XElement("RefLibraries"));

                Marshal.ReleaseComObject(itemMember.ReturnType);

                Parameters parameters = itemMember.Parameters;
                foreach (ParameterInfo paramInfo in itemMember.Parameters)
                {
                    VarTypeInfo paramTypeInfo = paramInfo.VarTypeInfo;
                    string paramTypeName = TypeDescriptor.FormattedType(paramTypeInfo,false);
                    XElement parameterNode = new XElement("Parameter",
                                    new XAttribute("Name",          paramInfo.Name),
                                    new XAttribute("Type",          paramTypeName),
                                    new XAttribute("TypeKind",      TypeInfo(paramTypeInfo.TypeInfo)),
                                    new XAttribute("IsExternal",    paramTypeInfo.IsExternalType.ToString()),
                                    new XAttribute("IsComProxy",    TypeDescriptor.IsCOMProxy(paramTypeInfo).ToString()),
                                    new XAttribute("IsOptional",    paramInfo.Optional.ToString().ToLower()),
                                    new XAttribute("IsEnum",        TypeDescriptor.IsEnum(paramTypeInfo).ToString().ToLower()),
                                    new XAttribute("IsRef",         TypeDescriptor.IsRef(paramTypeInfo).ToString().ToLower()),
                                    new XAttribute("IsArray",       TypeDescriptor.IsArray(paramTypeInfo).ToString().ToLower()),
                                    new XAttribute("IsNative",      TypeDescriptor.IsNative(paramTypeName).ToString().ToLower()),
                                    new XAttribute("TypeKey",       TypeDescriptor.GetTypeKey(libraryNode.Document, paramTypeInfo)),
                                    new XAttribute("ProjectKey",    TypeDescriptor.GetProjectKey(libraryNode.Document, paramTypeInfo)),
                                    new XAttribute("LibraryKey",    TypeDescriptor.GetLibraryKey(libraryNode.Document, paramTypeInfo)));

                    Marshal.ReleaseComObject(paramTypeInfo);

                    parametersNode.Add(parameterNode);
                    Marshal.ReleaseComObject(paramInfo);
                }
                Marshal.ReleaseComObject(parameters);

                methodNode.Add(parametersNode);
            }

            var refsNode = parametersNode.Elements("RefLibraries").FirstOrDefault();

            string key = libraryNode.Attributes("Key").FirstOrDefault().Value;

            // check refComponent exists 
            var lib = (from a in refsNode.Elements("Ref")
                       where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                       select a).FirstOrDefault();

            if (null == lib)
            {
                lib = new XElement("Ref",
                            new XAttribute("Key", key));

                refsNode.Add(lib);
            }
        }

        /// <summary>
        /// get existing parameters definition if exists or null
        /// </summary>
        /// <param name="methodNode"></param>
        /// <param name="itemMember"></param>
        /// <param name="withOptionals"></param>
        /// <returns></returns>
        internal static XElement GetParametersNode(XElement propertyNode, TLI.MemberInfo itemMember)
        {
            int targetParamsCount = itemMember.Parameters.Count;
    
            // list all definitions with target params count
            var parameters = (from a in propertyNode.Elements("Parameters")
                              where a.Elements("Parameter").Count() == targetParamsCount
                              select a);

            // special case zero parameters
            if (0 == targetParamsCount)
                return parameters.FirstOrDefault();

            // check for equal definition
            foreach (var itemParams in parameters)
            {
                int i = 0;
                bool isEqual = false;
                foreach (var itemParam in itemParams.Elements("Parameter"))
                {
                    ParameterInfo paramInfo = itemMember.Parameters[(short)(i + 1)];
                    if (true == paramInfo.Name.Equals(itemParam.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase))
                        isEqual = true;

                    Marshal.ReleaseComObject(paramInfo);
                    i++;

                    if (false == isEqual)
                        break;
                }

                if (true == isEqual)
                    return itemParams;
            }

            return null;
        }
        
        /// <summary>
        /// safe string wrapper
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        private static string TypeInfo(TypeInfo typeInfo)
        {            
            if (null != typeInfo)
            {
                string info = typeInfo.TypeKind.ToString();
                Marshal.ReleaseComObject(typeInfo);
                return info;
            }
            else
                return "";
        }
    }
}
