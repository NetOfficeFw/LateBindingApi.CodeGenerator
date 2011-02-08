using System;

namespace LateBindingApi.Core
{
    /// <summary>
    /// Indicates Assembly is a latebinding api assembly
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LateBindingAttribute: System.Attribute
    {
        public readonly string SupportedApiVersion;

        public LateBindingAttribute(string apiVersion)
        {
            this.SupportedApiVersion = apiVersion;
        }
    }
}




