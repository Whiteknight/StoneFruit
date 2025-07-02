using System;

namespace StoneFruit.Execution.Arguments;

/// <summary>
/// Attribute to associate a property in a class with an index number to facilitate
/// argument mapping
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