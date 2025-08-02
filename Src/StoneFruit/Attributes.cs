using System;

namespace StoneFruit;

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

/// <summary>
/// Specify a custom verb to use instead of automatically deriving the verb from the name of
/// the class or method.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class VerbAttribute : Attribute
{
    public VerbAttribute(params string[] verb)
    {
        Verb = verb;
    }

    public string[] Verb { get; }

    public bool Hide { get; set; }
}

/// <summary>
/// Attribute to associate a property in a class with an index number to facilitate
/// argument mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class ArgumentIndexAttribute : Attribute
{
    public int Index { get; }

    public ArgumentIndexAttribute(int index)
    {
        Index = index;
    }
}

/// <summary>
/// Attribute to associate a property in a class with a name to facilitate
/// argument mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class ArgumentNamedAttribute : Attribute
{
    public string Name { get; }

    public ArgumentNamedAttribute(string name)
    {
        Name = name;
    }
}
