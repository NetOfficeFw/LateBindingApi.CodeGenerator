using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace LateBindingApi.Core
{
    /// <summary>
    /// Represents a managed COMProxy 
    /// </summary>
    public class COMObject : COMVariant
    {
        #region Fields

        protected internal bool             _isDisposed;
        protected internal List<COMObject>  _listChildObjects  = new List<COMObject>();
        protected internal List<COMVariant> _listChildVariants = new List<COMVariant>();

        #endregion

        #region Construction

        public COMObject(COMVariant replacedObject)
        {
            if (false == replacedObject.IsCOMProxy)
                throw new ArgumentException("argument is not a com proxy");

            // remove variant from parent and add himself
            COMObject parent = replacedObject.ParentObject;
            parent.RemoveChildObject(replacedObject);
            parent.AddChildObject(this);

            // store members
            _parentObject     = parent;
            _underlyingObject = replacedObject.UnderlyingObject;
            _instanceType     = replacedObject.InstanceType;
        }

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

        }

        public COMObject(object comProxy)
        {
            _underlyingObject = comProxy;
            _instanceType = comProxy.GetType();
        }

        public COMObject(COMObject parentObject, object comProxy)
        {
            _parentObject = parentObject;
            _underlyingObject = comProxy;
            _instanceType = comProxy.GetType();

            if (null!= parentObject)
                _parentObject.AddChildObject(this);
        }

        public COMObject(COMObject parentObject, object comProxy, Type comProxyType)
        {
            _parentObject = parentObject;
            _underlyingObject = comProxy;
            _instanceType = comProxyType;

            if(null!=parentObject)
                _parentObject.AddChildObject(this);
        }

        public COMObject()
        {

        }

        #endregion

        #region IObject Properties

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public List<COMObject> ListChildObjects
        {
            get
            {
                return _listChildObjects;
            }
        }

        #endregion

        #region COMObject Properties

        public bool IsDisposed
        {
            get 
            {
                return _isDisposed;
            }
        }
        #endregion

        #region COMObject Methods

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public void CreateFromProgId(string progId)
        {
            _instanceType = System.Type.GetTypeFromProgID(progId);
            if (null == _instanceType)
                throw (new ArgumentException("progId not found. " + progId));

            _underlyingObject = Activator.CreateInstance(_instanceType);
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public void AddChildObject(IObject childObject)
        {
            if (childObject is COMObject)
                _listChildObjects.Add((COMObject)childObject);
            else if (childObject is COMVariant)
                _listChildVariants.Add((COMVariant)childObject);

            throw new ArgumentException("childObject is an unkown type.");
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public void AddChildObject(COMObject childObject)
        {
            _listChildObjects.Add(childObject);
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public void AddChildObject(COMVariant childObject)
        {
           _listChildVariants.Add(childObject);
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public void ReleaseCOMProxy()
        {
            IEventBinding typeEvent = this as IEventBinding;
            if (null != typeEvent)
                typeEvent.DisposeSinkHelper();

            // remove himself from parent childlist
            if (null != _parentObject)
            {
                _parentObject.RemoveChildObject(this);
                _parentObject = null;
            }

            // finally release himself
            if (null != _underlyingObject)
            {
                if (_underlyingObject is ICustomAdapter)
                {
                    // enumerator
                    ICustomAdapter adapter = (ICustomAdapter)_underlyingObject;
                    Marshal.ReleaseComObject(adapter.GetUnderlyingObject());
                }
                else
                {
                    Marshal.ReleaseComObject(_underlyingObject);
                }
                _underlyingObject = null;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public void RemoveChildObject(COMObject childObject)
        {
            _listChildObjects.Remove(childObject);
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public void RemoveChildObject(COMVariant childObject)
        {
            _listChildVariants.Remove(childObject);
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public void RemoveChildObjects()
        {
            // release all unkown childs and clear list
            foreach (COMVariant itemObject in _listChildVariants)
            {
                itemObject.Dispose();
            }
            _listChildVariants.Clear();

            // string name = TypeDescriptor.GetClassName(_underlyingObject);
 
            // release all childs and clear list
            foreach (COMObject itemObject in _listChildObjects)
            {
                //string childName = TypeDescriptor.GetClassName(itemObject.UnderlyingObject);

                itemObject.ParentObject = null;
                itemObject.Dispose();
                //itemObject.RemoveChildObjects();
                //itemObject.ReleaseCOMProxy();
            }
            _listChildObjects.Clear();
        }

        #endregion

        #region IDisposable Members

        public new void Dispose()
        {
            _isDisposed = true;

            // in case of object implements also event binding we dispose them
            IEventBinding eventBind = this as IEventBinding;
            if (null != eventBind)
                eventBind.DisposeSinkHelper();
            
            RemoveChildObjects();
            ReleaseCOMProxy();
        }

        #endregion
    }
}
