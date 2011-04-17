using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer
{
    public class ProjectFileFormatException :Exception
    {
        public ProjectFileFormatException(string message): base(message)
        { 
        }
    }
}
