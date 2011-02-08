using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer
{
    public class TypeLibRegistryKeys
    {  
        #region Fields

        private string                   _key;
        private List<TypeLibRegistryKey> _list;
        
        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public TypeLibRegistryKey this[int i]
        {
            get
            {
                return _list[i];
            }
        }

        public TypeLibRegistryKey this[string Name]
        {
            get
            {
                int iCount = Count;
                for (int i = 1; i <= iCount; i++)
                {
                    TypeLibRegistryKey entry = this[i - 1];
                    if (Name.Equals(entry.Name, StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        return entry;
                    }
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
            TypeLibRegistryKey[] returnKeys = new TypeLibRegistryKey[enumCount];

            for (int i = 0; i < enumCount; i++)
                returnKeys[i] = this[i];

            for (int i = 0; i < returnKeys.Length; i++)
            {
                yield return returnKeys[i];
            }

        }

        #endregion

        #region Construction

        internal TypeLibRegistryKeys( string Key)
        {
           
            _key    = Key;
            _list   = new List<TypeLibRegistryKey>();

            RegistryKey rk = Registry.ClassesRoot.OpenSubKey(_key, false);
            

            if (null != rk)
            { 
                string[] Subkeys = rk.GetSubKeyNames();
                foreach (string subKey in Subkeys)
                {
                    TypeLibRegistryKey newKey = new TypeLibRegistryKey(_key + "\\" + subKey);
                    _list.Add(newKey);

                }
                rk.Close();
            }
        }

        #endregion
    }
}
