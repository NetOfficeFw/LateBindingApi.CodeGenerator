using System;
using System.Collections.Generic;

namespace LateBindingApi.CodeGenerator.CSharp
{
    public class Settings
    {
        public Settings()
        {
            this.AddDocumentationLinks = true;
            this.SupportByVersionSpacing = 1;
        }

        // ReSharper disable once InconsistentNaming
        public bool VBOptimization { get; set; }

        public string SignPath { get; set; }

        public string LinkFilePath { get; set; }

        public bool AddDocumentationLinks { get; set; }

        public bool UseSigning { get; set; }

        public bool AddTestApp { get; set; }

        public bool CreateXmlDocumentation { get; set; }

        public bool ConvertOptionalsToObject { get; set; }

        public bool ConvertParamNamesToCamelCase { get; set; }

        public bool RemoveRefAttribute { get; set; }

        public string Framework { get; set; }

        public string Folder { get; set; }

        public bool OpenFolder { get; set; }

        public int SupportByVersionSpacing { get; set; }

        public IEnumerable<string> IgnoredProjects { get; set; }
    }
}
