using System;

namespace StoneFruit;

/// <summary>
/// Specifies a Group for a Handler class or method, when it appears in the help output.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class GroupAttribute : Attribute
{
    public GroupAttribute(string group)
    {
        Group = group;
    }

    public string Group { get; }
}
