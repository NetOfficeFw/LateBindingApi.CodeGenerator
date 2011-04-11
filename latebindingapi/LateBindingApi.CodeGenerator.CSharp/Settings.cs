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
        bool   _convertOptionalsToObject;
        bool   _convertParamNamesToCamelCase;
        bool   _removeRefAttribute;
        string _framework;
        bool   _openFolder;
        
        #endregion

        #region Properties

        public bool ConvertOptionalsToObject
        {
            get
            {
                return _convertOptionalsToObject;
            }
            internal set
            {
                _convertOptionalsToObject = value;
            }
        }


        public bool ConvertParamNamesToCamelCase
        {
            get
            {
                return _convertParamNamesToCamelCase;
            }
            internal set
            {
                _convertParamNamesToCamelCase = value;
            }
        }

        public bool RemoveRefAttribute
        {
            get
            {
                return _removeRefAttribute;
            }
            internal set
            {
                _removeRefAttribute = value;
            }
        }

        public string Framework
        {
            get
            {
                return _framework;
            }
            internal set
            {
                _framework = value;
            }
        }

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
