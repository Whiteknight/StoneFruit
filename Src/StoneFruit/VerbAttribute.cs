using System;

namespace StoneFruit;

/// <summary>
/// Specify a custom verb to use instead of automatically deriving the verb from the name of
/// the class or method
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
