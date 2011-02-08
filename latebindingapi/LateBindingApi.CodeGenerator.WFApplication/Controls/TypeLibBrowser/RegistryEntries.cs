using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.TypeLibBrowser
{
    public class RegistryEntries
    { 
        #region Fields

        private string                      _key;
        private List<RegistryEntry>         _list;

        #endregion

        #region Properties

        public int Count
        {
            get 
            {
                return _list.Count;
            }
        }

        public RegistryEntry this[int i]
        {
            get 
            {
                return _list[i];
            }
        }

        public RegistryEntry this[string name]
        {
            get
            {
                int iCount = Count;
                for (int i = 1; i <= iCount; i++)
                {
                    RegistryEntry entry = this[i-1];
                    if (name.Equals(entry.Name, StringComparison.CurrentCultureIgnoreCase) == true)
                        return entry;
                }

                throw (new IndexOutOfRangeException("RegistryEntry " + name + " not found."));
            }
        }

        /// <summary>
        /// Foreach Enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            int enumCount = Count;
            RegistryEntry[]returnEntries = new RegistryEntry[enumCount];

            for (int i = 0; i < enumCount; i++)
                returnEntries[i] = this[i];

            for (int i = 0; i < returnEntries.Length; i++)
            {
                yield return returnEntries[i];
            }

        }
 
        #endregion

        #region Construction

        internal RegistryEntries(string key)
        {
            _key = key;
            _list = new List<RegistryEntry>();
            Microsoft.Win32.RegistryKey regkKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(_key, false);
            if (null != regkKey)
            {
                string[] Values = regkKey.GetValueNames();
                foreach (string value in Values)
                {
                    RegistryEntry entry = null;
                    RegistryValueKind rvk = regkKey.GetValueKind(value);
                    object o = regkKey.GetValue(value);
                    entry = new RegistryEntry(value, o, rvk);
                    _list.Add(entry); 
                }

                regkKey.Close();
            }
        }

        #endregion
    }
}
