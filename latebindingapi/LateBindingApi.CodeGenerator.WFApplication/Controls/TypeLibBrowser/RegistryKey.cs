using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.TypeLibBrowser
{
    public class RegistryKey
    {  
        #region Fields

        private string                  _key;
        private string                  _name;

        private RegistryEntries _entries = null;
        private RegistryKeys    _subKeys = null;

        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return _name;
            }
        }

       
        public RegistryEntries Entries
        {
            get
            {
                return _entries;
            }
        }
       

        public RegistryKeys Keys
        {
            get
            {
                return _subKeys;
            }
        }
       
        #endregion

        #region Construction

        internal RegistryKey(string rootKey)
        {

            _key = rootKey;
            _name = rootKey.Substring(rootKey.LastIndexOf(@"\") + 1);

            _entries = new RegistryEntries(_key);
            _subKeys = new RegistryKeys(_key);

        }

        #endregion
    }
}
