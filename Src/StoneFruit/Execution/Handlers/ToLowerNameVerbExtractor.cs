using System;
using System.Reflection;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Verb extractor which takes the name of the handler class or method, removes common suffixes
/// ('verb', 'handler', 'command') and converts the remainder to lowercase.
/// </summary>
public class ToLowerNameVerbExtractor : IVerbExtractor
{
    public Result<Verb[], Verb.Error> GetVerbs(Type type)
        => type != null && typeof(IHandlerBase).IsAssignableFrom(type)
            ? GetVerbs(type.Name)
            : new Verb.InvalidHandler();

    public Result<Verb[], Verb.Error> GetVerbs(MethodInfo method)
        => method != null
            ? GetVerbs(method.Name)
            : new Verb.InvalidHandler();

    private static Result<Verb[], Verb.Error> GetVerbs(string name)
    {
        name = name.CleanVerbName()
            .ToLowerInvariant();
        if (string.IsNullOrEmpty(name))
            return new Verb.NoWords();
        return Verb.TryCreate(name)
            .Map(v => new[] { v });
    }
}
