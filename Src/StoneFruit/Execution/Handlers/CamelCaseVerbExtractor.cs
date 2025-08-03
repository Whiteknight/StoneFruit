using System;
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
    public Result<Verb[], Verb.Error> GetVerbs(Type type)
        => type.IsHandlerType()
            ? GetVerbs(type.Name)
            : Verb.InvalidHandler;

    public Result<Verb[], Verb.Error> GetVerbs(MethodInfo method) => GetVerbs(method.Name);

    private static Result<Verb[], Verb.Error> GetVerbs(string name)
    {
        name = name.CleanVerbName();

        if (string.IsNullOrEmpty(name))
            return Verb.NoWords;

        var camelCase = CamelCase();
        var result = camelCase.Parse(name);
        if (!result.Success)
            return Verb.IncorrectFormat;

        var verb = result.Value.Select(s => s.ToLowerInvariant()).ToArray();
        return Verb.TryCreate(verb)
            .Map(v => new[] { v });
    }
}
