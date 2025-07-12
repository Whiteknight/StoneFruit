using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Trie;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Handler source for the Microsoft DI container which finds types registered with the ServiceCollection
/// and resolves them using the ServiceProvider. All handler types should be registered with the
/// ServiceCollection before constructing an instance of this type.
/// </summary>
public class ServiceProviderHandlerSource : IHandlerSource
{
    private readonly VerbTrie<VerbInfo> _verbs;
    private readonly IServiceProvider _provider;

    public ServiceProviderHandlerSource(IServiceProvider provider, IVerbExtractor verbExtractor, IEnumerable<RegisteredHandler> handlerTypes)
    {
        _provider = provider;

        _verbs = handlerTypes
            .Where(sd => typeof(IHandlerBase).IsAssignableFrom(sd.HandlerType))
            .SelectMany(sd =>
                verbExtractor
                    .GetVerbs(sd.HandlerType)
                    .Select(verb => new VerbInfo(verb, sd.HandlerType))
            )
            .ToVerbTrie(vi => vi.Verb);
    }

    public Maybe<IHandlerBase> GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        => _verbs.Get(arguments).Bind(GetHandlerFromProvider);

    private Maybe<IHandlerBase> GetHandlerFromProvider(VerbInfo st)
    {
        using var scope = _provider.CreateScope();
        var instance = scope.ServiceProvider.GetService(st.Type) as IHandlerBase;
        return instance switch
        {
            IHandlerBase handler => new Maybe<IHandlerBase>(handler),
            _ => default
        };
    }

    public IEnumerable<IVerbInfo> GetAll() => _verbs.GetAll().Select(kvp => kvp.Value);

    public Maybe<IVerbInfo> GetByName(Verb verb) => _verbs.Get(verb).Map(i => (IVerbInfo)i);

    private sealed record VerbInfo(Verb Verb, Type Type) : IVerbInfo
    {
        public string Description => Type.GetDescription();

        public string Usage => Type.GetUsage();

        public string Group => Type.GetGroup();

        public bool ShouldShowInHelp => Type.ShouldShowInHelp(Verb);
    }
}
