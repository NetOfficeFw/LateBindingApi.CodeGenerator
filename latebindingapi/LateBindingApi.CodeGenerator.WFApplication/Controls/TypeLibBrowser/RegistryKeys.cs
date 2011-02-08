using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.TypeLibBrowser
{
    public class RegistryKeys
    {  
        #region Fields

        private string                   _key;
        private List<RegistryKey>       _list;
        
        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public RegistryKey this[int i]
        {
            get
            {
                return _list[i];
            }
        }

        public RegistryKey this[string name]
        {
            get
            {
                int iCount = Count;
                for (int i = 1; i <= iCount; i++)
                {
                    RegistryKey entry = this[i - 1];
                    if (name.Equals(entry.Name, StringComparison.CurrentCultureIgnoreCase) == true)
                        return entry;
                }
                return null;
            }
        }

        /// <summary>
        /// Foreach Enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            int enumCount = this.Count;
            RegistryKey[] returnKeys = new RegistryKey[enumCount];

            for (int i = 0; i < enumCount; i++)
                returnKeys[i] = this[i];

            for (int i = 0; i < returnKeys.Length; i++)
            {
                yield return returnKeys[i];
            }

        }

        #endregion

        #region Construction

        internal RegistryKeys(string key)
        {

            _key = key;
            _list   = new List<RegistryKey>();

            Microsoft.Win32.RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(_key, false);
            if (null != regKey)
            {
                string[] Subkeys = regKey.GetSubKeyNames();
                foreach (string subKey in Subkeys)
                {
                    RegistryKey newKey = new RegistryKey(_key + "\\" + subKey);
                    _list.Add(newKey);

                }
                regKey.Close();
            }
        }

        #endregion
    }
}
