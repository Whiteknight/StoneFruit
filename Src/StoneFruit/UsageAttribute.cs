using System;

namespace StoneFruit
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class UsageAttribute : Attribute
    {
        public UsageAttribute(string usage)
        {
            Usage = usage;
        }

        public string Usage { get; }
    }
}
