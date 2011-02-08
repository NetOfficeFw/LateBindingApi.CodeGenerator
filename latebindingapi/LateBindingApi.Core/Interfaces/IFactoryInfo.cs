using System;
using System.Reflection; 
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace LateBindingApi.Core
{
    public interface IFactoryInfo
    {
        string Prefix { get; }
        string Namespace { get; }
        Guid ComponentGuid { get; }
        Assembly Assembly { get; }
    }
}
