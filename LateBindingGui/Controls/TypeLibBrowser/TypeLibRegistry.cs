using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.TypeLibBrowser
{
    public class TypeLibRegistry
    {   
        #region Constants

        private static readonly string _RootKey = @"TypeLib";

        #endregion

        #region Fields

        private static RegistryKey     _key;
        private static RegistryEntries _entries;

        #endregion

        #region Properties

        public static bool Exists
        {
            get
            {
                bool retValue = false;
                Microsoft.Win32.RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(_RootKey, false);
                if (regKey != null)
                {
                    regKey.Close();
                    retValue = true;
                }

                return retValue;
            }
        }

        public static RegistryKey Key
        {
            get
            {
                if (null == _key)
                    _key = new RegistryKey(_RootKey);

                return _key;
            }
        }

        public static RegistryEntries Entries
        {

            get
            {
                if (null == _entries)
                    _entries = new RegistryEntries(_RootKey);

                return _entries;
            }
        }

        #endregion
    }
}
