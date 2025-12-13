using System;
using System.Reflection;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Verb extractor which takes the name of the handler class or method, removes common suffixes
/// ('verb', 'handler', 'command') and converts the remainder to lowercase.
/// </summary>
public class ToLowerNameVerbExtractor : IVerbExtractor
{
    public Result<Verb[], Verb.Error> GetVerbs(Type type)
        => type.IsHandlerType()
            ? GetVerbs(type.Name)
            : Verb.InvalidHandler;

    public Result<Verb[], Verb.Error> GetVerbs(MethodInfo method)
        => method != null
            ? GetVerbs(method.Name)
            : Verb.InvalidHandler;

    private static Result<Verb[], Verb.Error> GetVerbs(string name)
        => Validate
            .IsNotNullOrEmpty(
                name
                    .CleanVerbName()
                    .ToLowerInvariant())
            .ToResult(() => Verb.NoWords)
            .Bind(Verb.TryCreate)
            .Map(v => new[] { v });
}
