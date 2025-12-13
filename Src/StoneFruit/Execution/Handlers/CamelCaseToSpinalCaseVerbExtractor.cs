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
    public Result<Verb[], Verb.Error> GetVerbs(Type type)
        => type.IsHandlerType()
            ? GetVerbs(type.Name)
            : Verb.InvalidHandler;

    public Result<Verb[], Verb.Error> GetVerbs(MethodInfo method)
        => GetVerbs(method.Name);

    private static Result<Verb[], Verb.Error> GetVerbs(string name)
        => GetVerbsClean(name.CleanVerbName());

    private static Result<Verb[], Verb.Error> GetVerbsClean(string name)
        => string.IsNullOrEmpty(name)
            ? (Result<Verb[], Verb.Error>)Verb.NoWords
            : CamelCase()
                .Parse(name)
                .Match(
                    MakeVerbs,
                    () => Verb.IncorrectFormat);

    private static Result<Verb[], Verb.Error> MakeVerbs(IEnumerable<string> value)
    {
        var spinal = string.Join("-", value).ToLowerInvariant();
        return Verb.TryCreate(spinal)
            .Map(v => new[] { v });
    }
}
