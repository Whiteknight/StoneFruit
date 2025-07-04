using System;
using System.Collections.Generic;
using System.Reflection;
using ParserObjects;
using StoneFruit.Utility;
using static ParserObjects.Parsers;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Verb extractor to try and parse the class/method name as CamelCase and then convert it into
/// "spinal-case" (also known as "kebab-case").
/// </summary>
public class CamelCaseToSpinalCaseVerbExtractor : IVerbExtractor
{
    public IReadOnlyList<Verb> GetVerbs(Type type)
        => type != null && typeof(IHandlerBase).IsAssignableFrom(type)
            ? GetVerbs(type.Name)
            : [];

    public IReadOnlyList<Verb> GetVerbs(MethodInfo method) => GetVerbs(method.Name);

    private static IReadOnlyList<Verb> GetVerbs(string name)
    {
        if (string.IsNullOrEmpty(name))
            return [];

        name = name
            .RemoveSuffix("verb")
            .RemoveSuffix("command")
            .RemoveSuffix("handler");

        if (string.IsNullOrEmpty(name))
            return [];

        var camelCase = CamelCase();
        var result = camelCase.Parse(name);
        if (!result.Success)
            return [];

        var spinal = string.Join("-", result.Value).ToLowerInvariant();
        return [new Verb(spinal)];
    }
}
