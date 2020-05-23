using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer.Model
{
    public class OfficeProduct
    {
        public string Name { get; set; }

        public string VersionName { get; set; }

        public string ReleaseName { get; set; }

        public IEnumerable<string> Libraries { get; set; }
    }
}
