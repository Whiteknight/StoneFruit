using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Trie;
using StoneFruit.Utility;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Handler source for manually-specified handler instances.  This type is mainly used for
/// situations where the user doesn't employ a DI container to detect and resolve handler types
/// automatically.
/// </summary>
public class NamedInstanceHandlerSource : IHandlerSource
{
    private readonly VerbTrie<VerbInfo> _verbs;

    public NamedInstanceHandlerSource()
    {
        _verbs = new VerbTrie<VerbInfo>();
    }

    public void Add(Verb verb, IHandlerBase handler, string? description = null, string? usage = null, string? group = null)
    {
        description = (string.IsNullOrEmpty(description) ? handler.GetType().GetDescription() : null) ?? string.Empty;
        var info = new VerbInfo(
            verb,
            NotNull(handler),
            description,
            (string.IsNullOrEmpty(usage) ? handler.GetType().GetUsage() : null) ?? description,
            (string.IsNullOrEmpty(group) ? handler.GetType().GetGroup() : null) ?? string.Empty,
            handler.GetType().ShouldShowInHelp(verb));

        _verbs.Insert(verb, info);
    }

    public Maybe<IHandlerBase> GetInstance(HandlerContext context)
        => _verbs.Get(context.Arguments).Map(info => info.Handler);

    public IEnumerable<IVerbInfo> GetAll() => _verbs.GetAll().Select(kvp => kvp.Value);

    public Maybe<IVerbInfo> GetByName(Verb verb) => _verbs.Get(verb).Map(i => (IVerbInfo)i);

    public int Count => _verbs.Count;

    private sealed record VerbInfo(Verb Verb, IHandlerBase Handler, string Description, string Usage, string Group, bool ShouldShowInHelp) : IVerbInfo;
}
