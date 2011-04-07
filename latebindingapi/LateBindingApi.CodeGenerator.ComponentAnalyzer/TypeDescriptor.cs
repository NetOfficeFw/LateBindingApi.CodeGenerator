using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Xml.Linq;
using TLI;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer
{
    internal static class TypeDescriptor
    {

        /// <summary>
        /// returns typeInfo is COMProxy or not
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        internal static bool IsCOMProxy(VarTypeInfo typeInfo)
        {
            if (TliVarType.VT_DISPATCH == typeInfo.VarType)
                return true;

            if (null == typeInfo.TypeInfo)
                return false;

            if ((typeInfo.TypeInfo.TypeKind == TypeKinds.TKIND_DISPATCH) ||
               (typeInfo.TypeInfo.TypeKind == TypeKinds.TKIND_INTERFACE) ||
               (typeInfo.TypeInfo.TypeKind == TypeKinds.TKIND_COCLASS))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// returns typeInfo is Enum or not
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        internal static bool IsEnum(VarTypeInfo typeInfo)
        {
            if (typeInfo.TypeInfo == null)
                return false;

            if (typeInfo.TypeInfo.TypeKind  == TypeKinds.TKIND_ENUM)
                return true;
            else
                return false;
        }

        /// <summary>
        /// returns typeInfo is call by reference or call by value
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        internal static bool IsRef(VarTypeInfo typeInfo)
        {
            if( (typeInfo.VarType == TliVarType.VT_EMPTY) || ( typeInfo.VarType == TliVarType.VT_ARRAY) )
            {
                if (typeInfo.PointerLevel > 1)
                    return true;
                else
                    return false;
            }
            else
            {
                if (typeInfo.PointerLevel > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// returns typeInfo is Array or not
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        internal static bool IsArray(VarTypeInfo typeInfo)
        {
            if (typeInfo.VarType == TliVarType.VT_ARRAY)
                return true;
            else
            {
                bool isKnown = Enum.IsDefined(typeInfo.VarType.GetType(), typeInfo.VarType);
                return !isKnown;
            }
        }
        
        /// <summary>
        /// returns typeName is scalar or not
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        internal static bool IsNative(string typeName)
        {
            switch (typeName)
            {
                case "int":
                case "Int16":
                case "Int32":
                case "Int64":
                case "Single":
                case "double":
                case "Double":                
                case "string":
                case "String":
                case "bool":
                case "float":
                    return true;
            }
            return false;
        }

        /// <summary>
        /// get key attribute from typeInfo if exists
        /// </summary>
        /// <param name="document"></param>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        internal static string GetTypeKey(XDocument document, VarTypeInfo typeInfo)
        {
            TypeLibInfo libInfo = null;
            if (true == typeInfo.IsExternalType)
                libInfo = typeInfo.TypeLibInfoExternal;
            else
            {
                if (null == typeInfo.TypeInfo)
                    return "";

                libInfo = typeInfo.TypeInfo.Parent;
            }

            // look for lib
            string guid = Utils.EncodeGuid(libInfo.GUID);
            var libNode = (from a in document.Elements("LateBindingApi.CodeGenerator.Document").Elements("Libraries").Elements("Library")
                           where a.Attribute("GUID").Value.Equals(guid) &&
                                 a.Attribute("Name").Value.Equals(libInfo.Name) &&
                                 a.Attribute("Major").Value.Equals(libInfo.MajorVersion.ToString()) &&
                                 a.Attribute("Minor").Value.Equals(libInfo.MinorVersion.ToString())
                           select a).FirstOrDefault();

            Marshal.ReleaseComObject(libInfo);

            // look for project
            string libName = libNode.Attribute("Name").Value;
            var projectNode = (from a in document.Elements("LateBindingApi.CodeGenerator.Document").Elements("Solution").Elements("Projects").Elements("Project")
                               where a.Attribute("Name").Value.Equals(libName)
                               select a).FirstOrDefault();
            
            // look for DispatchInterface
            string typeName = typeInfo.TypeInfo.Name;
            var dispatchNode = (from a in projectNode.Elements("DispatchInterfaces").Elements("Interface")
                           where a.Attribute("Name").Value.Equals(typeName)
                           select a).FirstOrDefault();
            if (null != dispatchNode)
                return dispatchNode.Attribute("Key").Value;

            // look for Interface
            var faceNode = (from a in projectNode.Elements("Interfaces").Elements("Interface")
                                where a.Attribute("Name").Value.Equals(typeName)
                                select a).FirstOrDefault();
            if (null != faceNode)
                return faceNode.Attribute("Key").Value;

            // look for CoClass
            var classNode = (from a in projectNode.Elements("CoClasses").Elements("CoClass")
                            where a.Attribute("Name").Value.Equals(typeName)
                            select a).FirstOrDefault();
            if (null != classNode)
                return classNode.Attribute("Key").Value;

            // look for "Enum"
            var enumNode = (from a in projectNode.Elements("Enums").Elements("Enum")
                             where a.Attribute("Name").Value.Equals(typeName)
                             select a).FirstOrDefault();
            if (null != enumNode)
                return enumNode.Attribute("Key").Value;
            
            // type not found
            return "";
        }

        /// <summary>
        /// get key attribute from typeInfo parent project if exists
        /// </summary>
        /// <param name="document"></param>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        internal static string GetProjectKey(XDocument document, VarTypeInfo typeInfo)
        {
            TypeLibInfo libInfo = null;
            if (true == typeInfo.IsExternalType)
                libInfo = typeInfo.TypeLibInfoExternal;
            else
            {
                if (null == typeInfo.TypeInfo)
                    return "";

                libInfo = typeInfo.TypeInfo.Parent;
            }

            // look for lib
            string guid = Utils.EncodeGuid(libInfo.GUID);
            var libNode = (from a in document.Elements("LateBindingApi.CodeGenerator.Document").Elements("Libraries").Elements("Library")
                        where a.Attribute("GUID").Value.Equals(guid) &&
                              a.Attribute("Name").Value.Equals(libInfo.Name) &&
                              a.Attribute("Major").Value.Equals(libInfo.MajorVersion.ToString()) &&
                              a.Attribute("Minor").Value.Equals(libInfo.MinorVersion.ToString())
                        select a).FirstOrDefault();

            // look for project
            string name = libNode.Attribute("Name").Value;
            var projectNode = (from a in document.Elements("LateBindingApi.CodeGenerator.Document").Elements("Solution").Elements("Projects").Elements("Project")
                               where a.Attribute("Name").Value.Equals(name)
                           select a).FirstOrDefault();

            Marshal.ReleaseComObject(libInfo);

            string returnKey = "";
            if (null != projectNode)
                returnKey = projectNode.Attribute("Key").Value;

            return returnKey;
        }

        /// <summary>
        /// get key attribute from typeInfo parent lib if exists
        /// </summary>
        /// <param name="document"></param>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        internal static string GetLibraryKey(XDocument document, VarTypeInfo typeInfo)
        {
            TypeLibInfo libInfo = null;
            if (true == typeInfo.IsExternalType)
                libInfo = typeInfo.TypeLibInfoExternal;
            else
            {
                if (null == typeInfo.TypeInfo)
                    return "";

                libInfo = typeInfo.TypeInfo.Parent;
            }

            string guid = Utils.EncodeGuid(libInfo.GUID);

            var node = (from a in document.Elements("LateBindingApi.CodeGenerator.Document").Elements("Libraries").Elements("Library")
                        where a.Attribute("GUID").Value.Equals(guid) &&
                              a.Attribute("Name").Value.Equals(libInfo.Name) &&
                              a.Attribute("Major").Value.Equals(libInfo.MajorVersion.ToString()) &&
                              a.Attribute("Minor").Value.Equals(libInfo.MinorVersion.ToString())
                        select a).FirstOrDefault();

            Marshal.ReleaseComObject(libInfo);

            string returnKey ="";
            if(null!=node)
                returnKey =node.Attribute("Key").Value;

            return returnKey;
        }

        /// <summary>
        /// check TypeKinds interface type
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="wantDispatch"></param>
        /// <returns></returns>
        internal static bool IsTargetInterfaceType(TypeKinds kind, bool wantDispatch)
        {
            if (true == wantDispatch)
            {
                if (kind == TypeKinds.TKIND_DISPATCH)
                    return true;
                else
                    return false;
            }
            else
            {
                if (kind == TypeKinds.TKIND_DISPATCH)
                    return false;
                else
                    return true;
            }
        }
        
        /// <summary>
        /// checks memberInfo is based on IDispatch or IUnkown
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        internal static bool IsIDispatchOrIUnkownMethod(TLI.MemberInfo memberInfo)
        {
            switch (memberInfo.Name)
            {
                case "GetTypeInfoCount":
                case "GetTypeInfo":
                case "GetIDsOfNames":
                case "Invoke":
                    return true;
            }

            switch (memberInfo.Name)
            {
                case "AddRef":
                case "Release":
                case "QueryInterface":
                    return true;
            }

            return false;
        }

        /// <summary>
        /// checks memberInfo is method
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        internal static bool IsInterfaceMethod(TLI.MemberInfo memberInfo)
        {
            bool isImpliedMethod = IsIDispatchOrIUnkownMethod(memberInfo);
            if (true == isImpliedMethod)
                return false;

            if (memberInfo.DescKind == DescKinds.DESCKIND_FUNCDESC)
            {
                if ((memberInfo.InvokeKind != InvokeKinds.INVOKE_PROPERTYGET) &&
                    (memberInfo.InvokeKind != InvokeKinds.INVOKE_PROPERTYPUT) &&
                    (memberInfo.InvokeKind != InvokeKinds.INVOKE_PROPERTYPUTREF))
                    return true;
                else
                    return false;
            }
            else
            {
                if (memberInfo.InvokeKind == InvokeKinds.INVOKE_FUNC)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// checks memberInfo is property
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        internal static bool IsInterfaceProperty(TLI.MemberInfo memberInfo)
        {
            if (memberInfo.DescKind == DescKinds.DESCKIND_FUNCDESC)
            {
                if ((memberInfo.InvokeKind == InvokeKinds.INVOKE_PROPERTYGET) ||
                    (memberInfo.InvokeKind == InvokeKinds.INVOKE_PROPERTYPUT) ||
                    (memberInfo.InvokeKind == InvokeKinds.INVOKE_PROPERTYPUTREF))
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// returns info member is defined in a inherited interface
        /// </summary>
        /// <param name="itemMember"></param>
        /// <param name="impliedList"></param>
        /// <returns></returns>
        internal static bool IsInheritedMember(TLI.MemberInfo itemMember, List<TLI.InterfaceInfo> impliedList)
        {
            string name = itemMember.Name;

            foreach (TLI.InterfaceInfo item in impliedList)
            {
                TLI.Members members = item.Members;
                foreach (TLI.MemberInfo member in members)
                {
                    if (true == member.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Marshal.ReleaseComObject(member);
                        Marshal.ReleaseComObject(members);
                        return true;
                    }

                    Marshal.ReleaseComObject(member);
                }
                Marshal.ReleaseComObject(members);
            }

            return false;
        }
     
        /// <summary>
        /// returns friendly formatted type info
        /// converts COM datatype info to kown .net type & LateBindingApi types
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        internal static string FormattedType(VarTypeInfo typeInfo, bool convertUnkownToApiTypes)
        { 
            switch (typeInfo.VarType)
            {
                case TliVarType.VT_EMPTY:        // Type in Component
                case TliVarType.VT_ARRAY:
                    return typeInfo.TypeInfo.Name;
                case TliVarType.VT_CY:
                    return "float";
                case TliVarType.VT_DATE:
                    return "DateTime";
                case TliVarType.VT_UI1:
                    return "byte";
                case TliVarType.VT_R4:
                    return "Single";
                case TliVarType.VT_I1:
                case TliVarType.VT_I2:
                case TliVarType.VT_UI2:
                    return "Int16";
                case TliVarType.VT_I4:
                case TliVarType.VT_UI4:
                case TliVarType.VT_INT:
                case TliVarType.VT_HRESULT:
                    return "Int32";
                case TliVarType.VT_I8:
                case TliVarType.VT_UI8:
                    return "Int64";
                case TliVarType.VT_R8:          
                    return "Double";
                case TliVarType.VT_DECIMAL:
                    return "decimal";
                case TliVarType.VT_UINT:
                    return "UIntPtr";
                case TliVarType.VT_LPSTR:
                case TliVarType.VT_LPWSTR:
                case TliVarType.VT_BSTR:         
                    return "string";
                case TliVarType.VT_BOOL:        
                    return "bool";
                case TliVarType.VT_VOID:
                    return "void";
                case TliVarType.VT_UNKNOWN:
                case TliVarType.VT_DISPATCH:
                    if(true == convertUnkownToApiTypes)
                        return "COMProxy";
                    else
                        return "object";
                case TliVarType.VT_VARIANT:
                    if (true == convertUnkownToApiTypes)
                        return "COMVariant";
                    else
                        return "object";
                /*Unkown*/  
                 case TliVarType.VT_FILETIME:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_BLOB:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_STREAM:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_STORAGE:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_STREAMED_OBJECT:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_STORED_OBJECT:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_BLOB_OBJECT:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_CF:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_CLSID:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_VECTOR:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_BYREF:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_RESERVED:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_PTR:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_SAFEARRAY:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_CARRAY:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_USERDEFINED:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_RECORD:
                    return typeInfo.VarType.ToString();
                case TliVarType.VT_ERROR:
                    return typeInfo.VarType.ToString();
                default:
                    int i = Convert.ToInt32(typeInfo.VarType);
                    switch (i)
                    {
                        case 8194:
                            return "Int16";
                        case 8195:
                            return "Int32";
                        case 8196:
                            return "Single";
                        case 8197:
                            return "Double";
                        case 8198:
                            return "float";
                        case 8199:
                            return "DateTime";
                        case 8200:
                            return "String";
                        case 8201:
                            if (true == convertUnkownToApiTypes)
                                return "COMProxy";
                            else
                                return "object";
                        case 8204:
                            if (true == convertUnkownToApiTypes)
                                return "COMVariant";
                            else
                                return "object";
                        case 8203:
                            return "bool";
                        case 8209:
                            return "byte";
                        default:
                            return i.ToString();
                    }
            }
        }

        /// <summary>
        /// return all inherited interfaces from interface
        /// exlude IDispatch and IUnkown
        /// </summary>
        /// <param name="itemInterface"></param>
        /// <returns></returns>
        internal static List<TLI.InterfaceInfo> GetInheritedInterfaces(TLI.InterfaceInfo itemInterface)
        {
            List<TLI.InterfaceInfo> returnList = new List<InterfaceInfo>();

            TLI.Interfaces impliedInterfaces = null;

            // looks in vtable first
            InterfaceInfo vtable = itemInterface.VTableInterface;
            if (null != vtable)
            {
                impliedInterfaces = itemInterface.VTableInterface.ImpliedInterfaces;
                foreach (TLI.InterfaceInfo itemImplied in impliedInterfaces)
                {
                    if (("IDispatch" != itemImplied.Name) && ("IUnknown" != itemImplied.Name))
                        returnList.Add(itemImplied);
                    else
                        Marshal.ReleaseComObject(itemImplied);
                }

                Marshal.ReleaseComObject(impliedInterfaces);
                Marshal.ReleaseComObject(vtable);
            }

            // looks in dispatch
            impliedInterfaces = itemInterface.ImpliedInterfaces;
            foreach (TLI.InterfaceInfo itemImplied in impliedInterfaces)
            {
                if (("IDispatch" != itemImplied.Name) && ("IUnknown" != itemImplied.Name))
                    returnList.Add(itemImplied);
                else
                    Marshal.ReleaseComObject(itemImplied);
            }
            Marshal.ReleaseComObject(impliedInterfaces);

            return returnList;
        }

        /// <summary>
        /// return all inherited interfaces from itemClass
        /// exlude IDispatch and IUnkown
        /// </summary>
        /// <param name="itemInterface"></param>
        /// <returns></returns>
        internal static List<TLI.InterfaceInfo> GetInheritedInterfaces(TLI.CoClassInfo itemClass)
        {
            List<TLI.InterfaceInfo> returnList = new List<InterfaceInfo>();

            TLI.Interfaces impliedInterfaces = null;

            // looks in Interfaces
            impliedInterfaces = itemClass.Interfaces;
            foreach (TLI.InterfaceInfo itemImplied in impliedInterfaces)
            {
                if (("IDispatch" != itemImplied.Name) && ("IUnknown" != itemImplied.Name))
                    returnList.Add(itemImplied);
                else
                    Marshal.ReleaseComObject(itemImplied);
            }
            Marshal.ReleaseComObject(impliedInterfaces);

            return returnList;
        }
        
        /// <summary>
        /// return all event interfaces from itemClass
        /// exlude IDispatch and IUnkown
        /// </summary>
        /// <param name="itemInterface"></param>
        /// <returns></returns>
        internal static List<TLI.InterfaceInfo> GetEventInterfaces(TLI.CoClassInfo itemClass)
        {
            List<TLI.InterfaceInfo> returnList = new List<InterfaceInfo>();

            TLI.InterfaceInfo itemImplied = itemClass.DefaultEventInterface;
            if (null == itemImplied)
                return returnList;

            if (("IDispatch" != itemImplied.Name) && ("IUnknown" != itemImplied.Name))
                returnList.Add(itemImplied);
            else
                Marshal.ReleaseComObject(itemImplied);

            return returnList;
        }

        /// <summary>
        /// return all default interfaces from itemClass
        /// exlude IDispatch and IUnkown
        /// </summary>
        /// <param name="itemInterface"></param>
        /// <returns></returns>
        internal static List<TLI.InterfaceInfo> GetDefaultInterfaces(TLI.CoClassInfo itemClass)
        {
            List<TLI.InterfaceInfo> returnList = new List<InterfaceInfo>();

            TLI.InterfaceInfo itemImplied = itemClass.DefaultInterface;
            if (null == itemImplied)
                return returnList;
 
            if (("IDispatch" != itemImplied.Name) && ("IUnknown" != itemImplied.Name))
                returnList.Add(itemImplied);
            else
                Marshal.ReleaseComObject(itemImplied);

            return returnList;
        }
        
        /// <summary>
        /// Get members from interface
        /// exclude IDispatch Members, IUnkown Members and, any other members there comes from inherited interfaces
        /// </summary>
        /// <param name="itemInterface"></param>
        /// <returns></returns>
        internal static List<TLI.MemberInfo> GetFilteredMembers(TLI.InterfaceInfo itemInterface)
        {
            List<TLI.MemberInfo> returnList = new List<MemberInfo>();

            List<TLI.InterfaceInfo> inheritedList = GetInheritedInterfaces(itemInterface);

            TLI.Members interfaceMembers = itemInterface.Members;
            foreach (TLI.MemberInfo itemMember in interfaceMembers)
            {
                bool memberIsInherited = IsInheritedMember(itemMember, inheritedList);
                if (false == memberIsInherited)
                    returnList.Add(itemMember);
            }
            Marshal.ReleaseComObject(interfaceMembers);

            foreach (TLI.InterfaceInfo item in inheritedList)
                Marshal.ReleaseComObject(item);

            inheritedList.Clear();

            return returnList;
        }

        /// <summary>
        /// returns typelib description from registry
        /// </summary>
        /// <param name="libInfo"></param>
        /// <returns></returns>
        internal static string GetTypeLibDescription(TLI.TypeLibInfo libInfo)
        {
            string majorVersion = libInfo.MajorVersion.ToString();
            string minorVersion = libInfo.MinorVersion.ToString();
            string guid         = libInfo.GUID.ToString();

            string description  = "<NoDescription>";
            string version      = string.Format("{0}.{1}", majorVersion, minorVersion);
            string regKey       = string.Format("TypeLib\\{0}\\{1}", guid, version);

            RegistryKey rk = Registry.ClassesRoot.OpenSubKey(regKey, false);
            if (null != rk)
            {
                string[] values = rk.GetValueNames();
                if (values.Length > 0)
                    description = rk.GetValue(values[0]) as string;
                rk.Close();
            }

            return description;
        }
    }
}