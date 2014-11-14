﻿using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using COMTypes = System.Runtime.InteropServices.ComTypes;

namespace LateBindingApi.Core
{
    /// <summary>
    /// represents a managed COM proxy 
    /// </summary>
    public class COMObject : IDisposable 
    {
        #region Fields

        /// <summary>
        ///  returns parent proxy object
        /// </summary>
        protected internal COMObject            _parentObject;

        /// <summary>
        /// returns Type of native proxy
        /// </summary>
        protected internal Type                 _instanceType;

        /// <summary>
        /// returns the native wrapped proxy
        /// </summary>
        protected internal object               _underlyingObject;
        
        /// <summary>
        /// returns instance is an enumerator
        /// </summary>
        protected internal bool                 _isEnumerator;        

        /// <summary>
        /// returns instance is currently in diposing progress
        /// </summary>
        protected internal volatile bool        _isCurrentlyDisposing;
       
        /// <summary>
        /// returns instance is diposed means unusable
        /// </summary>
        protected internal volatile bool        _isDisposed;
        
        /// <summary>
        ///  child instance List
        /// </summary>
        protected internal List<COMObject>      _listChildObjects    = new List<COMObject>();

        /// <summary>
        /// list of runtime supported entities
        /// </summary>
        Dictionary<string, string>             _listSupportedEntities;

        #endregion

        #region Construction

        /// <summary>
        /// creates instance and replace the given replacedObject in proxy management
        /// all created childs from replacedObject are now childs from the new instance
        /// </summary>
        /// <param name="replacedObject"></param>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public COMObject(COMObject replacedObject)
        {
            // copy proxy
            _underlyingObject = replacedObject.UnderlyingObject;
            _parentObject     = replacedObject.ParentObject;
            _instanceType     = replacedObject.InstanceType;

            // copy childs
            foreach (IObject item in replacedObject.ListChildObjects)
                AddChildObject(item);

            // remove old object from parent chain
            if (null != replacedObject.ParentObject)
            {
                COMObject parentObject = replacedObject.ParentObject;
                parentObject.RemoveChildObject(replacedObject);
                
                // add himself as child to parent object
                parentObject.AddChildObject(this);
            }

            Factory.RemoveObjectFromList(replacedObject);
            Factory.AddObjectToList(this);
        }
        
        /// <summary>
        /// creates new instance with given proxy
        /// </summary>
        /// <param name="comProxy"></param>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public COMObject(object comProxy)
        {
            _underlyingObject = comProxy;
            _instanceType = comProxy.GetType();

            Factory.AddObjectToList(this);
        }

        /// <summary>
        /// creates new instance with given proxy and parent info
        /// </summary>
        /// <param name="parentObject"></param>
        /// <param name="comProxy"></param>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public COMObject(COMObject parentObject, object comProxy)
        {
            _parentObject = parentObject;
            _underlyingObject = comProxy;
            _instanceType = comProxy.GetType();

            if (null!= parentObject)
                _parentObject.AddChildObject(this);

            Factory.AddObjectToList(this);
        }

        /// <summary>
        /// creates new instance with given proxy, parent info and info instance is an enumerator
        /// </summary>
        /// <param name="parentObject"></param>
        /// <param name="comProxy"></param>
        ///  <param name="isEnumerator"></param>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public COMObject(COMObject parentObject, object comProxy, bool isEnumerator)
        {
            _parentObject = parentObject;
            _underlyingObject = comProxy;
            _isEnumerator = isEnumerator;
            _instanceType = comProxy.GetType();

            if (null != parentObject)
                _parentObject.AddChildObject(this);

            Factory.AddObjectToList(this);
        }


        /// <summary>
        /// creates new instance with given proxy, type info and parent info
        /// </summary>
        /// <param name="parentObject"></param>
        /// <param name="comProxy"></param>
        /// <param name="comProxyType"></param>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public COMObject(COMObject parentObject, object comProxy, Type comProxyType)
        {
            _parentObject = parentObject;
            _underlyingObject = comProxy;
            _instanceType = comProxyType;

            if(null!=parentObject)
                _parentObject.AddChildObject(this);

            Factory.AddObjectToList(this);
        }

        /// <summary>
        /// creates a new instace with progid
        /// </summary>
        /// <param name="progId">registered ProgID</param>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public COMObject(string progId)
        {
            CreateFromProgId(progId);
            Factory.AddObjectToList(this);
        }

        /// <summary>
        /// not usable stub constructor
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public COMObject()
        {
            Factory.AddObjectToList(this);
        }

        #endregion

        #region COMObject Properties

        /// <summary>
        /// returns the native wrapped proxy
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public object UnderlyingObject
        {
            get
            {
                return _underlyingObject;
            }
        }

        /// <summary>
        /// returns class name of native wrapped proxy
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string UnderlyingTypeName
        {
            get
            {
                return TypeDescriptor.GetClassName(_underlyingObject);
            }
        }

        /// <summary>
        /// returns instance is diposed means unusable
        /// </summary>
        public bool IsDisposed
        {
            get 
            {
                return _isDisposed;
            }
        }

        /// <summary>
        /// returns Type of native proxy
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public Type InstanceType
        {
            get
            {
                return _instanceType;
            }
        }

        /// <summary>
        /// returns parent proxy object
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public COMObject ParentObject
        {
            get
            {
                return _parentObject;
            }
            set
            {
                _parentObject = value;
            }
        }

        /// <summary>
        /// returns instance is currently in diposing progress
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public bool IsCurrentlyDisposing
        {
            get
            {
                return _isCurrentlyDisposing;
            }
        }
        
        /// <summary>
        /// returns instance export events
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public bool IsEventBind
        {
            get 
            {
                return (null != (this as IEventBinding));
            }
        }

        /// <summary>
        /// returns event bridge is advised
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public bool EventBridgeInitialized
        {
            get
            {
                IEventBinding bindInfo = this as IEventBinding;
                if (null != bindInfo)
                    return bindInfo.EventBridgeInitialized;
                else
                    return false;
            }
        }

        /// <summary>
        ///  retuns instance has one or more event recipients
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public bool HasEventRecipients
        {
            get
            {
                IEventBinding bindInfo = this as IEventBinding;
                if(null!=bindInfo)
                    return bindInfo.HasEventRecipients;
                else
                    return false;
            }
        }

        /// <summary>
        ///  child instance List
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        internal List<COMObject> ListChildObjects
        {
            get
            {
                return _listChildObjects;
            }
        }

        #endregion

        #region COMObject Methods
 
        /// <summary>
        ///  returns information the proxy provides a method or property with given name at runtime
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool SupportEntity(string name)
        {
            string outValue = null;
            bool result = _listSupportedEntities.TryGetValue("Property-" +name, out outValue);
            if(result)
                return true;

            return _listSupportedEntities.TryGetValue("Method-" + name, out outValue);
        }

        /// <summary>
        /// returns information the proxy provides a property with given name at runtime
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool SupportProperty(string propertyName)
        {
            string outValue = null;
            return _listSupportedEntities.TryGetValue("Property-" + propertyName, out outValue);
        }

        /// <summary>
        /// returns information the proxy provides a method with given name at runtime
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public bool SupportMethod(string methodName)
        {
            if (null == _listSupportedEntities)
                _listSupportedEntities = GetSupportedEntities(_underlyingObject);

            string outValue = null;
            return _listSupportedEntities.TryGetValue("Method-" + methodName, out outValue);
        }

        /// <summary>
        /// create object from progid
        /// </summary>
        /// <param name="progId"></param>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public void CreateFromProgId(string progId)
        {
            _instanceType = System.Type.GetTypeFromProgID(progId);
            if (null == _instanceType)
                throw (new ArgumentException("progId not found. " + progId));

            _underlyingObject = Activator.CreateInstance(_instanceType);
        }

        /// <summary>
        /// add object to child list
        /// </summary>
        /// <param name="childObject"></param>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public void AddChildObject(IObject childObject)
        {
            if (childObject is COMObject)
                _listChildObjects.Add((COMObject)childObject);

            throw new ArgumentException("childObject is an unkown type.");
        }

        /// <summary>
        /// add object to child list
        /// </summary>
        /// <param name="childObject"></param>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public void AddChildObject(COMObject childObject)
        {
            _listChildObjects.Add(childObject);
        }

        /// <summary>
        /// remove object to child list
        /// </summary>
        /// <param name="childObject"></param>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public void RemoveChildObject(COMObject childObject)
        {
            _listChildObjects.Remove(childObject);
        }

        /// <summary>
        /// release com proxy
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public void ReleaseCOMProxy()
        {
            // release himself from COM Runtime System
            if (null != _underlyingObject)
            {
                if (_isEnumerator) 
                {
                    ICustomAdapter adapter = _underlyingObject as ICustomAdapter;
                    Marshal.ReleaseComObject(adapter.GetUnderlyingObject());
                }
                else
                {
                    Marshal.ReleaseComObject(_underlyingObject);
                }
                Factory.RemoveObjectFromList(this);
                _underlyingObject = null;
            }
        }
     
        #endregion

        #region IDisposable Members

        /// <summary>
        /// dispose instance and all child instances
        /// </summary>
        /// <param name="disposeEventBinding">dispose event exported proxies with one or more event recipients</param>
        public void Dispose(bool disposeEventBinding)
        {
            // in case object export events and 
            // disposeEventBinding == true we dont remove the object from parents child list
            bool removeFromParent = true;

            // set disposed flag
            _isCurrentlyDisposing = true;

            // in case of object implements also event binding we dispose them
            IEventBinding eventBind = this as IEventBinding;
            if (disposeEventBinding)
            {
                if (null != eventBind)
                    eventBind.DisposeSinkHelper();
            }
            else
            {
                if ((null != eventBind) && (eventBind.EventBridgeInitialized))
                    removeFromParent = false;
            }

            // child proxy dispose
            DisposeChildInstances(disposeEventBinding);

            // remove himself from parent childlist
            if ((null != _parentObject) && (true == removeFromParent))
            {
                _parentObject.RemoveChildObject(this);
                _parentObject = null;
            }

            // release proxy
            ReleaseCOMProxy();

            // clear supportList
            if (null != _listSupportedEntities)
            { 
                _listSupportedEntities.Clear();
                _listSupportedEntities = null;
            }

            _isDisposed = true;
            _isCurrentlyDisposing = false;
        }

        /// <summary>
        /// dispose instance and all child instances
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// dispose all child instances
        /// </summary>
        /// <param name="disposeEventBinding">dispose proxies with events and one or more event recipients</param>
        public void DisposeChildInstances(bool disposeEventBinding)
        {
            // release all childs and clear list
            foreach (COMObject itemObject in _listChildObjects)
            {
                itemObject.ParentObject = null;
                itemObject.Dispose(disposeEventBinding);
            }
            _listChildObjects.Clear();
        }

        /// <summary>
        /// dispose all child instances
        /// </summary>
        public void DisposeChildInstances()
        {
            // release all childs and clear list
            foreach (COMObject itemObject in _listChildObjects)
            {
                itemObject.ParentObject = null;
                itemObject.Dispose();
            }
            _listChildObjects.Clear();
        }

        #endregion

        #region Static Methods

        private static Dictionary<string, string> GetSupportedEntities(object comProxy)
        {
            Dictionary<string, string> supportList = new Dictionary<string, string>();
            IDispatch dispatch = comProxy as IDispatch;
            if (null == dispatch)
                throw new COMException("Unable to cast underlying proxy to IDispatch.");

            COMTypes.ITypeInfo typeInfo = dispatch.GetTypeInfo(0, 0);
            if(null == typeInfo)
                throw new COMException("GetTypeInfo returns null.");

            IntPtr typeAttrPointer = IntPtr.Zero;
            typeInfo.GetTypeAttr(out typeAttrPointer);

            COMTypes.TYPEATTR typeAttr = (COMTypes.TYPEATTR)Marshal.PtrToStructure(typeAttrPointer, typeof(COMTypes.TYPEATTR));
            for (int i = 0; i < typeAttr.cFuncs; i++)
            {
                string strName, strDocString, strHelpFile;
                int dwHelpContext;
                IntPtr funcDescPointer = IntPtr.Zero;
                System.Runtime.InteropServices.ComTypes.FUNCDESC funcDesc;
                typeInfo.GetFuncDesc(i, out funcDescPointer);
                funcDesc = (COMTypes.FUNCDESC)Marshal.PtrToStructure(funcDescPointer, typeof(System.Runtime.InteropServices.ComTypes.FUNCDESC));

                switch (funcDesc.invkind)
                {
                    case System.Runtime.InteropServices.ComTypes.INVOKEKIND.INVOKE_PROPERTYGET:
                    case System.Runtime.InteropServices.ComTypes.INVOKEKIND.INVOKE_PROPERTYPUT:
                    case System.Runtime.InteropServices.ComTypes.INVOKEKIND.INVOKE_PROPERTYPUTREF:
                        typeInfo.GetDocumentation(funcDesc.memid, out strName, out strDocString, out dwHelpContext, out strHelpFile);
                        supportList.Add("Property-" + strName, strDocString);
                        break;
                    case System.Runtime.InteropServices.ComTypes.INVOKEKIND.INVOKE_FUNC:
                        typeInfo.GetDocumentation(funcDesc.memid, out strName, out strDocString, out dwHelpContext, out strHelpFile);
                        supportList.Add("Method-" + strName, strDocString);
                        break;
                }

                typeInfo.ReleaseFuncDesc(funcDescPointer);
            }

            typeInfo.ReleaseTypeAttr(typeAttrPointer);
            Marshal.ReleaseComObject(typeInfo);

            return supportList;
        }

        #endregion
    }
}
