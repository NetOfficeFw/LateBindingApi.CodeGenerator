using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer
{
    public class TypeLibRegistry
    {   
        #region Constants

        private static readonly string _RootKey = @"TypeLib";

        #endregion

        #region Fields

        private static TypeLibRegistryKey _key;

        private static TypeLibRegistryEntries _entries;

        #endregion

        #region Properties

        public static bool Exists
        {
            get
            {
                bool retValue = false;
                RegistryKey rk = Registry.ClassesRoot.OpenSubKey(_RootKey, false);
                if (rk != null)
                {
                    rk.Close();
                    retValue = true;
                }

                return retValue;
            }
        }

        public static TypeLibRegistryKey Key
        {
            get
            {
                if (null == _key)
                {
                    _key = new TypeLibRegistryKey(_RootKey);
                }
                return _key;
            }
        }

        public static TypeLibRegistryEntries Entries
        {

            get
            {

                if (null == _entries)
                {
                    _entries = new TypeLibRegistryEntries(_RootKey);
                }
                return _entries;
            }
        }

        #endregion
    }
}
