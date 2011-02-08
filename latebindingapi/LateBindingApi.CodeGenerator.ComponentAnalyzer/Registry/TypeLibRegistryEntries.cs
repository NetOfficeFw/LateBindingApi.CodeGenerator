using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer
{
    public class TypeLibRegistryEntries
    { 
        #region Fields

        private string                      _key;
     
        private List<TypeLibRegistryEntry> _list;

        #endregion

        #region Properties

        public int Count
        {
            get 
            {
                return _list.Count;
            }
        }

        public TypeLibRegistryEntry this[int i]
        {
            get 
            {
                return _list[i];
            }
        }

        public TypeLibRegistryEntry this[string Name]
        {
            get
            {
                int iCount = Count;
                for (int i = 1; i <= iCount; i++)
                {
                    TypeLibRegistryEntry entry = this[i-1];
                    if (Name.Equals(entry.Name, StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        return entry;
                    }
                }

                throw (new IndexOutOfRangeException("RegistryEntry " + Name + " not found."));
            }
        }

        /// <summary>
        /// Foreach Enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            int enumCount = Count;
            TypeLibRegistryEntry[]returnEntries = new TypeLibRegistryEntry[enumCount];

            for (int i = 0; i < enumCount; i++)
                returnEntries[i] = this[i];

            for (int i = 0; i < returnEntries.Length; i++)
            {
                yield return returnEntries[i];
            }

        }
 
        #endregion

        #region Construction

        internal TypeLibRegistryEntries(string Key)
        {

            _key    = Key;
            _list = new List<TypeLibRegistryEntry>();
            RegistryKey rk = Registry.ClassesRoot.OpenSubKey(_key, false);

            if (null != rk)
            { 
                string[] Values = rk.GetValueNames();
                foreach (string value in Values)
                {
                    TypeLibRegistryEntry entry = null;
                    RegistryValueKind rvk = rk.GetValueKind(value);
                    object o = rk.GetValue(value);
                    entry = new TypeLibRegistryEntry(value, o, rvk);
                    _list.Add(entry); 
                }
                
                rk.Close();
            }
        }

        #endregion
    }
}
