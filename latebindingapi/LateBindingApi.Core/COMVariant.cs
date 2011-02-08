using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace LateBindingApi.Core
{   
    /// <summary>
    /// Represents a unknown type also known as Variant 
    /// </summary>
    public class COMVariant : IObject
    {
        #region Fields

        protected internal COMObject  _parentObject;
        protected internal Type       _instanceType;
        protected internal object     _underlyingObject;

        #endregion

        #region Construction
        
        public COMVariant()
        {
 
        }
        public COMVariant(object unkownObject)
        {
            _underlyingObject = unkownObject;
            _instanceType = unkownObject.GetType();
        }

        public COMVariant(COMObject parentObject, object unkownObject)
        {
            _parentObject = parentObject;
            _underlyingObject = unkownObject;
            _instanceType = unkownObject.GetType();

            if((true == _instanceType.IsCOMObject) && (null != parentObject))
                parentObject.AddChildObject(this);
        }

        public COMVariant(COMObject parentObject, object unkownObject, Type unkownObjectType)
        {
            _parentObject = parentObject;
            _underlyingObject = unkownObject;
            _instanceType = unkownObjectType;
            
            if((true == _instanceType.IsCOMObject) && (null!=parentObject))
                parentObject.AddChildObject(this);
        }

        #endregion

        #region IObject Members

        public object UnderlyingObject
        {
            get 
            {
                return _underlyingObject;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public Type InstanceType
        {
            get
            {
                return _instanceType;
            }
        }

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

        #endregion

        #region COMVariant Members

        /// <summary>
        /// UnderlyingObject is a com proxy
        /// </summary>
        public bool IsCOMProxy
        {
            get
            {
                return _instanceType.IsCOMObject;
            }
        }

        /// <summary>
        /// name of UnderlyingObject type
        /// </summary>
        public string TypeName
        {
            get
            {
                if (IsCOMProxy == true)
                {
                    return TypeDescriptor.GetClassName(_underlyingObject);
                }
                else
                {
                    return _instanceType.ToString();
                }
            }
        }
        
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if ((true == IsCOMProxy) && (null != _underlyingObject))
            {
                Marshal.ReleaseComObject(_underlyingObject);
                _underlyingObject = null;
            }
        }

        #endregion

        #region Static Helper
        
        public static COMVariant ToCOMVariant(bool value)
        {
            return new COMVariant(value);
        }

        public static COMVariant ToCOMVariant(string value)
        {
            return new COMVariant(value);
        }

        public static COMVariant ToCOMVariant(Int32 value)
        {
            return new COMVariant(value);
        }

        public static COMVariant ToCOMVariant(Double value)
        {
            return new COMVariant(value);
        }

        public static COMVariant ToCOMVariant(object value)
        {
            if (null == value)
                throw (new ArgumentNullException("value"));

            Type valueType = value.GetType();
            if (true == valueType.IsCOMObject)
                throw (new ArgumentException("value is COMObject"));

            return new COMVariant(value);
        }

        #endregion
    }
}