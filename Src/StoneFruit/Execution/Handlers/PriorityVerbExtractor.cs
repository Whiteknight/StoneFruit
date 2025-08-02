using System;
using System.Linq;
using System.Reflection;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Takes an array of type verb extractors and attempts to invoke each one in sequence
/// until a verb is successfully found.
/// </summary>
public class PriorityVerbExtractor : IVerbExtractor
{
    // The default set of extractors. First we look for VerbAttribute, then we attempt to parse
    // the name as CamelCase, finally we just take the name and lowercase it.
    private static readonly Lazy<IVerbExtractor> _default = new Lazy<IVerbExtractor>(
        () => new PriorityVerbExtractor(
            new VerbAttributeVerbExtractor(),
            new CamelCaseVerbExtractor(),
            new ToLowerNameVerbExtractor()
        )
    );

    private readonly IVerbExtractor[] _extractors;

    public PriorityVerbExtractor(params IVerbExtractor[] extractors)
    {
        _extractors = extractors;
    }

    /// <summary>
    /// Gets the default ITypeVerbExtractor instance which will be used if a custom
    /// one isn't provided.
    /// </summary>
    public static IVerbExtractor DefaultInstance => _default.Value;

    public Result<Verb[], Verb.Error> GetVerbs(Type type)
        => type == null || !typeof(IHandlerBase).IsAssignableFrom(type)
            ? new Verb.InvalidHandler()
            : _extractors
                .Select(e => e.GetVerbs(type))
                .FirstOrDefault(verbs => verbs.Satisfies(v => v.Length > 0), new Verb.NoWords());

    public Result<Verb[], Verb.Error> GetVerbs(MethodInfo method)
        => method == null
            ? new Verb.InvalidHandler()
            : _extractors
                .Select(e => e.GetVerbs(method))
                .FirstOrDefault(verbs => verbs.Satisfies(v => v.Length > 0), new Verb.NoWords());
}
