using System;

namespace StoneFruit
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class GroupAttribute : Attribute
    {
        public GroupAttribute(string group)
        {
            Group = group;
        }

        public string Group { get; }
    }
}
