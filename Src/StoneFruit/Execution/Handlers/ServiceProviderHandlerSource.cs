using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Trie;
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
            .Where(sd => sd.HandlerType.IsHandlerType())
            .SelectMany(sd => GetVerbsForHandler(verbExtractor, sd))
            .ToVerbTrie(vi => vi.Verb);
    }

    public Maybe<IHandlerBase> GetInstance(HandlerContext context)
        => _verbs.Get(context.Arguments).Bind(GetHandlerFromProvider);

    public IEnumerable<IVerbInfo> GetAll() => _verbs.GetAll().Select(kvp => kvp.Value);

    public Maybe<IVerbInfo> GetByName(Verb verb) => _verbs.Get(verb).Map(i => (IVerbInfo)i);

    private Maybe<IHandlerBase> GetHandlerFromProvider(VerbInfo st)
    {
        using var scope = _provider.CreateScope();
        return scope.ServiceProvider.GetService(st.Type) switch
        {
            IHandlerBase handler => new Maybe<IHandlerBase>(handler),
            _ => default
        };
    }

    private static IEnumerable<VerbInfo> GetVerbsForHandler(IVerbExtractor verbExtractor, RegisteredHandler sd)
        => verbExtractor
            .GetVerbs(sd.HandlerType)
            .Map(verbs => verbs.Select(verb => verb.AddPrefix(sd.Prefix)))
            .Map(verbs => verbs.Select(verb => new VerbInfo(verb, sd.HandlerType)))
            .Match(
                vis => vis,
                _ => []);

    private sealed record VerbInfo(Verb Verb, Type Type) : IVerbInfo
    {
        public string Description => Type.GetDescription();

        public string Usage => Type.GetUsage();

        public string Group => Type.GetGroup();

        public bool ShouldShowInHelp => Type.ShouldShowInHelp(Verb);
    }
}
