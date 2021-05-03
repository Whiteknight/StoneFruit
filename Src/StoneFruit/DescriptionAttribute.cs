using System;

namespace StoneFruit
{
    /// <summary>
    /// Specifies a human-readable description of the handler class or method, for display in the
    /// help output.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}
