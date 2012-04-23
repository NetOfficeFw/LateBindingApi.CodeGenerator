using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.Documentation
{
    class Settings
    {
        #region Fields

        string _folder;
        
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

        #endregion
    }
}
