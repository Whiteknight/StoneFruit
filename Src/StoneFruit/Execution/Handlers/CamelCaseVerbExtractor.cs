using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ParserObjects;
using StoneFruit.Utility;
using static ParserObjects.Parsers;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Verb extractor to derive the verb from the name of the handler class. Common
/// suffixes are removed ('verb', 'command', 'handler'), CamelCase words are converted to
/// lower case words, and the verb may have whitespace.
/// </summary>
public class CamelCaseVerbExtractor : IVerbExtractor
{
    public IReadOnlyList<Verb> GetVerbs(Type type)
        => type.IsHandlerType()
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

        var verb = result.Value.Select(s => s.ToLowerInvariant()).ToArray();
        return [new Verb(verb)];
    }
}
