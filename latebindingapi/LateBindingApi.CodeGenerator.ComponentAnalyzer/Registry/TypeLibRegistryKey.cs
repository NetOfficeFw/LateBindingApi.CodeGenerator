using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer
{
    public class TypeLibRegistryKey
    {  
        #region Fields

        private string                  _key;
        private string                  _name;

        private TypeLibRegistryEntries _entries = null;
        private TypeLibRegistryKeys    _subKeys = null;

        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return _name;
            }
        }

       
        public TypeLibRegistryEntries Entries
        {
            get
            {
                return _entries;
            }
        }
       

        public TypeLibRegistryKeys Keys
        {
            get
            {
                return _subKeys;
            }
        }
       
        #endregion

        #region Construction

        internal TypeLibRegistryKey(string RootKey)
        {
              
            _key = RootKey;
            _name = RootKey.Substring(RootKey.LastIndexOf(@"\") + 1);

            _entries = new TypeLibRegistryEntries(_key);
            _subKeys = new TypeLibRegistryKeys(_key);

        }

        #endregion
    }
}
