using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Execution.Trie;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Handler source for function delegates.
/// </summary>
public class DelegateHandlerSource : IHandlerSource
{
    private readonly VerbTrie<IHandlerBase> _handlers;

    public DelegateHandlerSource()
    {
        _handlers = new VerbTrie<IHandlerBase>();
    }

    public int Count => _handlers.Count;

    public Maybe<IHandlerBase> GetInstance(HandlerContext context)
        => _handlers.Get(context.Arguments);

    public IEnumerable<IVerbInfo> GetAll()
        => _handlers.GetAll()
            .Select(kvp => kvp.Value)
            .Cast<IVerbInfo>();

    public Maybe<IVerbInfo> GetByName(Verb verb)
        => _handlers.Get(verb).Map(i => (IVerbInfo)i);

    public void Add(Verb verb, Action<IArguments, HandlerContext> act, string description, string usage, string group)
    {
        _handlers.Insert(verb, new SyncHandler(act, verb, description, usage, group));
    }

    public void Add(Verb verb, Func<IArguments, HandlerContext, CancellationToken, Task> func, string description, string usage, string group)
    {
        _handlers.Insert(verb, new AsyncHandler(func, verb, description, usage, group));
    }

    private sealed record SyncHandler(Action<IArguments, HandlerContext> Act, Verb Verb, string Description, string Usage, string Group)
        : IHandler, IVerbInfo
    {
        public bool ShouldShowInHelp => true;

        public void Execute(IArguments arguments, HandlerContext context)
            => Act(arguments, context);
    }

    private sealed record AsyncHandler(Func<IArguments, HandlerContext, CancellationToken, Task> Func, Verb Verb, string Description, string Usage, string Group)
        : IAsyncHandler, IVerbInfo
    {
        public bool ShouldShowInHelp => true;

        public Task ExecuteAsync(IArguments arguments, HandlerContext context, CancellationToken cancellation)
            => Func(arguments, context, cancellation);
    }
}
