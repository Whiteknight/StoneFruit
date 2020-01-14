using System;

namespace StoneFruit.Execution.Arguments
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class ArgumentIndexAttribute : Attribute
    {
        public int Index { get; }

        public ArgumentIndexAttribute(int index)
        {
            Index = index;
        }
    }
}