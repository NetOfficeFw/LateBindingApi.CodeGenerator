using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer.Model
{
    public class Library
    {
        /// <summary>
        /// Library name. A unique identifier.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Type Library unique ID.
        /// </summary>
        public string GUID { get; set; }
    }
}
