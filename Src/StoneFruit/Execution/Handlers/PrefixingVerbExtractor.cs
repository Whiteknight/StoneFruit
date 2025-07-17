using System;
using System.Collections.Generic;
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

    public IReadOnlyList<Verb> GetVerbs(Type type)
        => _inner
            .GetVerbs(type)
            .Select(v => v.AddPrefix(_prefix))
            .ToArray();

    public IReadOnlyList<Verb> GetVerbs(MethodInfo method)
        => _inner
            .GetVerbs(method)
            .Select(v => v.AddPrefix(_prefix))
            .ToArray();
}
