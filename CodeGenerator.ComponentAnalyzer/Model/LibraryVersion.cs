using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer.Model
{
    public class LibraryVersion
    {
        public string LibraryName { get; set; }

        public string Key { get; set; }

        public string File { get; set; }

        public string HelpFile { get; set; }

        public short Major { get; set; }

        public short Minor { get; set; }

        public int LCID { get; set; }

        public string Description { get; set; }

        public string Version { get; set; }
    }
}
