using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer.Model
{
    public class LibraryDependency
    {
        public int LibraryDependencyId { get; set; }

        public string LibraryName { get; set; }

        public int LibraryVersionId { get; set; }
    }
}
