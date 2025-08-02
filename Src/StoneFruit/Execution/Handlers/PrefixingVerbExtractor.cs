using System;
using System.Linq;
using System.Reflection;

namespace StoneFruit.Execution.Handlers;

public class PrefixingVerbExtractor : IVerbExtractor
{
    private readonly string _prefix;
    private readonly IVerbExtractor _inner;

    public PrefixingVerbExtractor(string prefix, IVerbExtractor inner)
    {
        _prefix = prefix;
        _inner = inner;
    }

    public Result<Verb[], Verb.Error> GetVerbs(Type type)
        => _inner
            .GetVerbs(type)
            .Map(verbs => verbs
                .Select(v => v.AddPrefix(_prefix))
                .ToArray());

    public Result<Verb[], Verb.Error> GetVerbs(MethodInfo method)
        => _inner
            .GetVerbs(method)
            .Map(verbs => verbs
                .Select(v => v.AddPrefix(_prefix))
                .ToArray());
}
