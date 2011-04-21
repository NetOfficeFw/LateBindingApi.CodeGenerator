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
    /// offers add method for methods
    /// </summary>
    internal static class MethodHandler
    {    
        /// <summary>
        /// create new method node or get existing
        /// </summary>
        /// <param name="itemMember"></param>
        /// <param name="faceNode"></param>
        /// <returns></returns>
        internal static XElement CreateMethodNode(TLI.MemberInfo itemMember, XElement faceNode)
        {
            var methodsNode = faceNode.Elements("Methods").FirstOrDefault();

            // check method exists
            var node = (from a in methodsNode.Elements()
                        where a.Attribute("Name").Value.Equals(itemMember.Name, StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();

            if (null == node)
            {
                node = new XElement("Method",
                               new XElement("DispIds"),
                               new XElement("RefLibraries"),
                               new XAttribute("Name", itemMember.Name),
                               new XAttribute("Key", Utils.NewEncodedGuid()));

                methodsNode.Add(node);
            }

            return node;
        }

        /// <summary>
        /// add method definition(s)
        /// </summary>
        /// <param name="itemMember"></param>
        /// <param name="methodNode"></param>
        internal static void AddMethod(XElement libraryNode, XElement methodNode, TLI.MemberInfo itemMember)
        {
            AddParametersToMethodNode(libraryNode, methodNode, itemMember);
        }

        /// <summary>
        /// add a method definition
        /// </summary>
        /// <param name="libraryNode"></param>
        /// <param name="methodNode"></param>
        /// <param name="itemMember"></param>
        /// <param name="withOptionalParameters"></param>
        internal static void AddParametersToMethodNode(XElement libraryNode, XElement methodNode, TLI.MemberInfo itemMember)
        {
            // check defintion exists
            XElement parametersNode = GetParametersNode(methodNode, itemMember);
            if (null == parametersNode)
            {
                VarTypeInfo returnTypeInfo = itemMember.ReturnType;
                string returnTypeName = TypeDescriptor.FormattedType(returnTypeInfo, true);
                parametersNode = new XElement("Parameters",
                                    new XElement("ReturnValue",
                                        new XAttribute("Type", returnTypeName),
                                        new XAttribute("TypeKind",   TypeInfo(returnTypeInfo.TypeInfo)),
                                        new XAttribute("IsComProxy", TypeDescriptor.IsCOMProxy(returnTypeInfo).ToString().ToLower()),
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
                    string paramTypeName = TypeDescriptor.FormattedType(paramTypeInfo, false);
                    XElement parameterNode = new XElement("Parameter",
                                    new XAttribute("Name",       paramInfo.Name),
                                    new XAttribute("Type",       paramTypeName),
                                    new XAttribute("TypeKind",   TypeInfo(paramTypeInfo.TypeInfo)),
                                    new XAttribute("IsExternal", paramTypeInfo.IsExternalType.ToString().ToLower()),
                                    new XAttribute("IsComProxy", TypeDescriptor.IsCOMProxy(paramTypeInfo).ToString().ToLower()),
                                    new XAttribute("IsOptional", paramInfo.Optional.ToString().ToLower()),
                                    new XAttribute("IsEnum",     TypeDescriptor.IsEnum(paramTypeInfo).ToString().ToLower()),
                                    new XAttribute("IsRef",      TypeDescriptor.IsRef(paramTypeInfo).ToString().ToLower()),
                                    new XAttribute("IsArray",    TypeDescriptor.IsArray(paramTypeInfo).ToString().ToLower()),
                                    new XAttribute("IsNative",   TypeDescriptor.IsNative(paramTypeName).ToString().ToLower()),
                                    new XAttribute("TypeKey",    TypeDescriptor.GetTypeKey(libraryNode.Document, paramTypeInfo)),
                                    new XAttribute("ProjectKey", TypeDescriptor.GetProjectKey(libraryNode.Document, paramTypeInfo)),
                                    new XAttribute("LibraryKey", TypeDescriptor.GetLibraryKey(libraryNode.Document, paramTypeInfo)));
                    
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
        internal static XElement GetParametersNode(XElement methodNode, TLI.MemberInfo itemMember)
        {
            int targetParamsCount = itemMember.Parameters.Count;
           
            // list all definitions with target params count
            var parameters = (from a in methodNode.Elements("Parameters")
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

                    if(false == isEqual)
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
        internal static string TypeInfo(TypeInfo typeInfo)
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
