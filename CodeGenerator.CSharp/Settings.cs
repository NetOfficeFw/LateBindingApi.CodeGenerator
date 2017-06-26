using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    public class Settings
    {
        #region Fields

        string _folder;
        bool   _convertOptionalsToObject;
        bool   _convertParamNamesToCamelCase;
        bool   _removeRefAttribute;
        bool   _createDocu;
        string _framework;
        bool   _addTestApp;
        bool   _openFolder;
        bool  _useSigning;
        bool _addDocumentationLinks = true;
        string _linkFilePath;
        string _signPath;

        #endregion

        #region Properties

        public bool VBOptimization{get;set;}

        public string SignPath
        {
            get
            {
                return _signPath;
            }
            set
            {
                _signPath = value;
            }
        }

        public string LinkFilePath
        {
            get
            {
                return _linkFilePath;
            }
            set
            {
                _linkFilePath = value;
            }

        }
        public bool AddDocumentationLinks
        {
            get
            {
                return _addDocumentationLinks;
            }
            set
            {
                _addDocumentationLinks = value;
            }
        }

        public bool UseSigning
        {
            get
            {
                return _useSigning;
            }
            set
            {
                _useSigning = value;
            }
        }

        public bool AddTestApp
        {
            get
            {
                return _addTestApp;
            }
            set
            {
                _addTestApp = value;
            }
        }

        public bool CreateXmlDocumentation
        {
            get
            {
                return _createDocu;
            }
            set
            {
                _createDocu = value;
            }
        }

        public bool ConvertOptionalsToObject
        {
            get
            {
                return _convertOptionalsToObject;
            }
            set
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
            set
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
            set
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
            set
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
            set
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
            set
            {
                _openFolder = value;
            }
        }

        #endregion
    }
}
