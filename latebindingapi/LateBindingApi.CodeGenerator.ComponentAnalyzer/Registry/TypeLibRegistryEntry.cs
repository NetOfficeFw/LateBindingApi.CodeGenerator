using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer
{
    public class TypeLibRegistryEntry
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

        internal TypeLibRegistryEntry(string ValueName, object Value, RegistryValueKind ValueType)
        {
            _valueName  = ValueName;
            _value      = Value;
            _valueType  = ValueType;
        }

        #endregion
    }
}
