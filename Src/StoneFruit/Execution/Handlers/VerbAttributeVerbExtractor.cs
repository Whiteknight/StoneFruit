using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Extracts a list of Verbs from VerbAttribute annotations on the class or method.
/// </summary>
public class VerbAttributeVerbExtractor : IVerbExtractor
{
    public IReadOnlyList<Verb> GetVerbs(Type type)
        => type != null && typeof(IHandlerBase).IsAssignableFrom(type)
            ? GetVerbsInternal(type.GetCustomAttributes<VerbAttribute>())
            : [];

    public IReadOnlyList<Verb> GetVerbs(MethodInfo method)
        => GetVerbsInternal(method?.GetCustomAttributes<VerbAttribute>() ?? []);

    private static IReadOnlyList<Verb> GetVerbsInternal(IEnumerable<VerbAttribute> attrs)
        => [.. attrs.Select(a => a.Verb)];
}
