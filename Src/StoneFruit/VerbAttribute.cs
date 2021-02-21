using System;

namespace StoneFruit
{
    /// <summary>
    /// Specify a custom verb to use if the correct verb cannot be automatically determined
    /// from the class name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class VerbAttribute : Attribute
    {
        public VerbAttribute(params string[] verb)
        {
            Verb = verb;
        }

        public Verb Verb { get; }

        public bool Hide { get; set; }
    }
}
