using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using StoneFruit.Trie;
using StoneFruit.Utility;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// A handler source which takes a list of Type and attempts to construct one using
/// built-in mechanisms. This source is used primarily for built-in handlers and for situations
/// where the user is not employing a DI container to find and resolve handlers automatically.
/// </summary>
public class TypeListConstructSource : IHandlerSource
{
    private readonly TypeInstanceResolver _resolver;
    private readonly VerbTrie<VerbInfo> _types;

    public TypeListConstructSource(
        IEnumerable<Type> types,
        TypeInstanceResolver resolver,
        IVerbExtractor verbExtractor)
    {
        _resolver = NotNull(resolver);
        _types = NotNull(types)
            .SelectMany(t => verbExtractor.GetVerbs(t).Select(v => new VerbInfo(v, t)))
            .ToVerbTrie(vi => vi.Verb);
    }

    public Maybe<IHandlerBase> GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        => _types.Get(arguments)
            .Bind(vi => ResolveInstance(arguments, dispatcher, vi.Type)!);

    private Maybe<IHandlerBase> ResolveInstance(IArguments command, CommandDispatcher dispatcher, Type commandType)
        => _resolver(commandType, command, dispatcher) switch
        {
            IHandlerBase handler => new Maybe<IHandlerBase>(handler),
            _ => default
        };

    public IEnumerable<IVerbInfo> GetAll() => _types.GetAll().Select(kvp => kvp.Value);

    public Maybe<IVerbInfo> GetByName(Verb verb) => _types.Get(verb).Map(i => (IVerbInfo)i);

    private sealed record VerbInfo(Verb Verb, Type Type) : IVerbInfo
    {
        public string Group => Type.GetGroup();
        public string Description => Type.GetDescription();
        public string Usage => Type.GetUsage();
        public bool ShouldShowInHelp => Type.ShouldShowInHelp(Verb);
    }
}
