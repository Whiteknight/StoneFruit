using System;

namespace StoneFruit;

/// <summary>
/// Specifies a usage string to display in the help output for the class or method.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class UsageAttribute : Attribute
{
    public UsageAttribute(string usage)
    {
        Usage = usage;
    }

    public string Usage { get; }
}
