using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.TypeLibBrowser
{
    public class RegistryEntry
    { 
        #region Fields

        private string             _valueName;
        private object             _value;
        private RegistryValueKind  _valueType;

        #endregion

        #region Properties

        public object Value
        {
            get
            {
                return _value;
            }
        }

        public string Name
        {
            get 
            {
                return _valueName; 
            }
        }

        public RegistryValueKind ValueType
        {
            get
            {
                return _valueType;
            }
        }

        #endregion

        #region Construction

        internal RegistryEntry(string valueName, object value, RegistryValueKind valueType)
        {
            _valueName = valueName;
            _value      = value;
            _valueType = valueType;
        }

        #endregion
    }
}
