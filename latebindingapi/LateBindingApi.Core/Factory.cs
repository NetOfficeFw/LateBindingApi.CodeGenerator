﻿using System;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using COMTypes = System.Runtime.InteropServices.ComTypes;

namespace LateBindingApi.Core
{
    #region IDispatch

    [Guid("00020400-0000-0000-c000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDispatch
    {
        int GetTypeInfoCount();
        System.Runtime.InteropServices.ComTypes.ITypeInfo GetTypeInfo([MarshalAs(UnmanagedType.U4)] int iTInfo,[MarshalAs(UnmanagedType.U4)] int lcid);

        [PreserveSig]
        int GetIDsOfNames(ref Guid riid, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] rgsNames, int cNames, int lcid, [MarshalAs(UnmanagedType.LPArray)] int[] rgDispId);
        
        [PreserveSig]
        int Invoke(int dispIdMember, ref Guid riid, [MarshalAs(UnmanagedType.U4)] int lcid, [MarshalAs(UnmanagedType.U4)]  
                                                        int dwFlags, ref System.Runtime.InteropServices.ComTypes.DISPPARAMS pDispParams, 
                                                        [Out, MarshalAs(UnmanagedType.LPArray)] object[] pVarResult, 
                                                        ref System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo, 
                                                        [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] pArgErr);
    }
    
    #endregion

    public static class Factory
    {
        private static List<COMVariant> _globalObjectList = new List<COMVariant>();

        public delegate void ProxyCountChangedHandler(int proxyCount);
        public static event ProxyCountChangedHandler ProxyCountChanged; 

        internal static void AddObjectToList(COMObject proxy)
        {
            _globalObjectList.Add(proxy);

            if (null!=ProxyCountChanged)
                ProxyCountChanged(_globalObjectList.Count);
        }

        internal static void RemoveObjectFromList(COMObject proxy)
        {
            _globalObjectList.Remove(proxy);

            if (null != ProxyCountChanged)
                ProxyCountChanged(_globalObjectList.Count);
        }

        /// <summary>
        /// Returns count count of open proxies
        /// </summary>
        public static int ProxyCount
        {
            get             
            {
                return _globalObjectList.Count;
            }            
        }

        #region Fields

        private static List<IFactoryInfo>       _factoryList = new List<IFactoryInfo>();
        private static Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();

        #endregion
        
        #region COMObject
  
        /// <summary>
        /// creates a new COMObject based on classType of comProxy 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="comProxy"></param>
        /// <returns></returns>
        public static COMObject CreateObjectFromComProxy(COMObject caller, object comProxy)
        { 
            if (null == comProxy)
                return null;
           
            IFactoryInfo factoryInfo = GetFactoryInfo(comProxy);
           
            string className = TypeDescriptor.GetClassName(comProxy);
            string fullClassName = factoryInfo.Namespace + "." + className;

            // create new classType
            Type comProxyType = comProxy.GetType();
            COMObject newObject = CreateObjectFromComProxy(factoryInfo, caller, comProxy, comProxyType, className, fullClassName);
            
            return newObject;
        }

        private static COMObject CreateObjectFromComProxy(IFactoryInfo factoryInfo, COMObject caller, object comProxy, Type comProxyType, string className, string fullClassName)
        {
            Type classType = null;
            if (true == _typeCache.TryGetValue(fullClassName, out classType))
            {
                // cached classType
                object newClass = Activator.CreateInstance(classType, new object[] { caller, comProxy });
                return (COMObject)newClass;
            }
            else
            {
                // create new classType
                classType = factoryInfo.Assembly.GetType(fullClassName);
                if (null == classType)
                    throw new ArgumentException("Class not exists: " + fullClassName);

                _typeCache.Add(fullClassName, classType);
                COMObject newClass = (COMObject)Activator.CreateInstance(classType, new object[] { caller, comProxy, comProxyType });
                return newClass;
            }
        }
        
        /// <summary>
        ///  creates a new COMVariant array based on type of comVariant[]
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="comVariant"></param>
        /// <returns></returns>
        public static COMObject[] CreateObjectArrayFromComProxy(COMObject caller, object[] comProxyArray)
        {
            if (null == comProxyArray)
                return null;

            Type comVariantType = null;
            COMObject[] newVariantArray = new COMObject[comProxyArray.Length];
            for (int i = 0; i < comProxyArray.Length; i++)
            {
                comVariantType = comProxyArray[i].GetType();
                IFactoryInfo factoryInfo = GetFactoryInfo(comProxyArray[i]);
                string className = TypeDescriptor.GetClassName(comProxyArray[i]);
                string fullClassName = factoryInfo.Namespace + "." + className;
                newVariantArray[i] = CreateObjectFromComProxy(factoryInfo, caller, comProxyArray[i], comVariantType, className, fullClassName);
            }
            return newVariantArray;
        }

        #endregion

        #region COMVariant

        /// <summary>
        ///  creates a new COMVariant based on type of comVariant 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="comVariant"></param>
        /// <returns></returns>
        public static COMVariant CreateVariantFromComProxy(COMObject caller, object comVariant)
        {
            if (null == comVariant)
                return null;
            
            Type comVariantType = comVariant.GetType();
            if (false == comVariantType.IsCOMObject)
            {
                COMVariant newVariant = new COMVariant(caller, comVariant, comVariantType);
                return newVariant;
            }
            else
            {
                IFactoryInfo factoryInfo = GetFactoryInfo(comVariant);
                string className = TypeDescriptor.GetClassName(comVariant);
                string fullClassName = factoryInfo.Namespace + "." + className;
                COMObject newObject = CreateObjectFromComProxy(factoryInfo, caller, comVariant, comVariantType, className, fullClassName);
                return newObject;
            }
        }

        /// <summary>
        ///  creates a new COMVariant array based on type of comVariant[]
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="comVariant"></param>
        /// <returns></returns>
        public static COMVariant[] CreateVariantArrayFromComProxy(COMObject caller, object[] comVariantArray)
        {
            if (null == comVariantArray)
                return null;

            Type comVariantType = null;
            COMVariant[] newVariantArray = new COMVariant[comVariantArray.Length];
            for (int i = 0; i < comVariantArray.Length; i++)
            {
                comVariantType = comVariantArray[i].GetType();
                if (false == comVariantType.IsCOMObject)
                {
                    newVariantArray[i] = new COMVariant(caller, comVariantArray[i], comVariantType);
                }
                else
                {
                    IFactoryInfo factoryInfo = GetFactoryInfo(comVariantArray[i]);
                    string className = TypeDescriptor.GetClassName(comVariantArray[i]);
                    string fullClassName = factoryInfo.Namespace + "." + className;
                    newVariantArray[i] = CreateObjectFromComProxy(factoryInfo, caller, comVariantArray[i], comVariantType, className, fullClassName);
                }                    
             }
             return newVariantArray;        
        }

        #endregion

        /// <summary>
        /// Must be called from client assembly for COMVariant and COMObject Support
        /// Recieve FactoryInfos from all loaded LateBindingApi based Assemblies
        /// </summary>
        public static void Initialize()
        {
            _factoryList.Clear();
            Assembly callingAssembly = System.Reflection.Assembly.GetCallingAssembly();
            foreach (AssemblyName item in callingAssembly.GetReferencedAssemblies())
            {
                Assembly itemAssembly = Assembly.Load(item);                 
                object[] attributes = itemAssembly.GetCustomAttributes(true);
                foreach (object itemAttribute in attributes)
                {
                    string fullnameAttribute = itemAttribute.GetType().FullName;
                    if (fullnameAttribute == "LateBindingApi.Core.LateBindingAttribute")
                    {
                        Type factoryInfoType = itemAssembly.GetType(item.Name + ".Utils.ProjectInfo");
                        IFactoryInfo factoryInfo = Activator.CreateInstance(factoryInfoType) as IFactoryInfo;
                        _factoryList.Add(factoryInfo);
                    }
                }
            }
        }
        
        /// <summary>
        /// get the guid from type lib there is the type defined
        /// </summary>
        /// <param name="comProxy"></param>
        /// <returns></returns>
        private static Guid GetParentLibraryGuid(object comProxy)
        {
            Guid returnGuid = Guid.Empty; 

            IDispatch dispatcher = (IDispatch)comProxy;
            COMTypes.ITypeInfo typeInfo = dispatcher.GetTypeInfo(0, 0);
            COMTypes.ITypeLib parentTypeLib = null;

            int i = 0;
            typeInfo.GetContainingTypeLib(out parentTypeLib, out i);
            
            IntPtr attributesPointer = IntPtr.Zero;
            parentTypeLib.GetLibAttr(out attributesPointer);
           
            COMTypes.TYPEATTR attributes = (COMTypes.TYPEATTR)Marshal.PtrToStructure(attributesPointer, typeof(COMTypes.TYPEATTR));
            returnGuid = attributes.guid; 
            
            parentTypeLib.ReleaseTLibAttr(attributesPointer);
            Marshal.ReleaseComObject(parentTypeLib);
            Marshal.ReleaseComObject(typeInfo);

            return returnGuid;
        }
        
        /// <summary>
        /// get wrapper class factory info 
        /// </summary>
        /// <param name="comProxy"></param>
        /// <returns></returns>
        private static IFactoryInfo GetFactoryInfo(object comProxy)
        {
            if (_factoryList.Count == 0)
                throw (new LateBindingApiException("Factory are not initialized with generated LateBindingApi assemblies."));
            
            Guid targetGuid = GetParentLibraryGuid(comProxy);
        
            foreach (IFactoryInfo item in _factoryList)
            {
                if (true == targetGuid.Equals(item.ComponentGuid))
                    return item;

            }

            throw new LateBindingApiException(TypeDescriptor.GetClassName(comProxy) + " class not found in LateBindingApi.");
        }
    }
}
