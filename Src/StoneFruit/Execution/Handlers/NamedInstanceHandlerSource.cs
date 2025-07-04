using System.Collections.Generic;
using System.Linq;
using StoneFruit.Trie;
using StoneFruit.Utility;

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

    public void Add(Verb verb, IHandlerBase handlerObject, string? description = null, string? usage = null, string? group = null)
    {
        var info = new VerbInfo(verb, handlerObject, description, usage, group);
        _verbs.Insert(verb, info);
    }

    public Maybe<IHandlerBase> GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        => _verbs.Get(arguments).Map(info => info.HandlerObject);

    public IEnumerable<IVerbInfo> GetAll() => _verbs.GetAll().Select(kvp => kvp.Value);

    public Maybe<IVerbInfo> GetByName(Verb verb) => _verbs.Get(verb).Map(i => (IVerbInfo)i);

    public int Count => _verbs.Count;

    private class VerbInfo : IVerbInfo
    {
        private readonly string? _description;
        private readonly string? _usage;
        private readonly string? _group;

        public VerbInfo(Verb verb, IHandlerBase handlerObject, string? description, string? usage, string? group)
        {
            Verb = verb;
            HandlerObject = handlerObject;
            _description = description;
            _usage = usage;
            _group = group;
        }

        public IHandlerBase HandlerObject { get; }
        public Verb Verb { get; }

        public string Description => !string.IsNullOrEmpty(_description)
            ? _description!
            : HandlerObject.GetType().GetDescription() ?? string.Empty;

        public string Usage => !string.IsNullOrEmpty(_usage)
            ? _usage!
            : HandlerObject.GetType().GetUsage() ?? Description;

        public string Group => !string.IsNullOrEmpty(_group)
            ? _group!
            : HandlerObject.GetType().GetGroup() ?? string.Empty;

        public bool ShouldShowInHelp => HandlerObject.GetType().ShouldShowInHelp(Verb);
    }
}
