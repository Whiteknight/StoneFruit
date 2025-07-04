using System;
using System.Collections.Generic;
using System.Reflection;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Verb extractor which takes the name of the handler class or method, removes common suffixes
/// ('verb', 'handler', 'command') and converts the remainder to lowercase.
/// </summary>
public class ToLowerNameVerbExtractor : IVerbExtractor
{
    public IReadOnlyList<Verb> GetVerbs(Type type)
        => type != null && typeof(IHandlerBase).IsAssignableFrom(type)
            ? GetVerbs(type.Name)
            : [];

    public IReadOnlyList<Verb> GetVerbs(MethodInfo method)
        => method != null ? GetVerbs(method.Name) : [];

    private static IReadOnlyList<Verb> GetVerbs(string name)
        => [GetNameWithoutSuffixes(name)];

    private static string GetNameWithoutSuffixes(string name)
        => name
            .RemoveSuffix("verb")
            .RemoveSuffix("command")
            .RemoveSuffix("handler")
            .ToLowerInvariant();
}
