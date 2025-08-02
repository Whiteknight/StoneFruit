using System;
using System.Reflection;
using ParserObjects;
using static ParserObjects.Parsers;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Verb extractor to try and parse the class/method name as CamelCase and then convert it into
/// "spinal-case" (also known as "kebab-case").
/// </summary>
public class CamelCaseToSpinalCaseVerbExtractor : IVerbExtractor
{
    public Result<Verb[], Verb.Error> GetVerbs(Type type)
        => type != null && typeof(IHandlerBase).IsAssignableFrom(type)
            ? GetVerbs(type.Name)
            : new Verb.InvalidHandler();

    public Result<Verb[], Verb.Error> GetVerbs(MethodInfo method)
        => GetVerbs(method.Name);

    private static Result<Verb[], Verb.Error> GetVerbs(string name)
    {
        name = name.CleanVerbName();

        if (string.IsNullOrEmpty(name))
            return new Verb.NoWords();

        var camelCase = CamelCase();
        var result = camelCase.Parse(name);
        if (!result.Success)
            return new Verb.IncorrectFormat();

        var spinal = string.Join("-", result.Value).ToLowerInvariant();
        return Verb.TryCreate(spinal)
            .Map(v => new[] { v });
    }
}
