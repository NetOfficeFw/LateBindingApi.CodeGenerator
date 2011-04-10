using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal class Settings
    {
        #region Fields

        string _folder;
        bool   _openFolder;
        
        #endregion

        #region Properties

        public string Folder
        {
            get 
            {
                return _folder;
            }
            internal set
            {
                _folder = value;
            }
        }

        public bool OpenFolder
        {
            get
            {
                return _openFolder;
            }
            internal set
            {
                _openFolder = value;
            }
        }
        #endregion

    }
}
