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

    public DelegateHandlerSource Add(Verb verb, Action<HandlerContext> act, string description, string usage, string group)
    {
        _handlers.Insert(verb, new SyncHandler(act, verb, description, usage, group));
        return this;
    }

    public DelegateHandlerSource Add(Verb verb, Func<HandlerContext, CancellationToken, Task> func, string description, string usage, string group)
    {
        _handlers.Insert(verb, new AsyncHandler(func, verb, description, usage, group));
        return this;
    }

    private sealed record SyncHandler(Action<HandlerContext> Act, Verb Verb, string Description, string Usage, string Group)
        : IHandlerWithContext, IVerbInfo
    {
        public bool ShouldShowInHelp => true;

        public void Execute(HandlerContext context)
            => Act(context);
    }

    private sealed record AsyncHandler(Func<HandlerContext, CancellationToken, Task> Func, Verb Verb, string Description, string Usage, string Group)
        : IAsyncHandlerWithContext, IVerbInfo
    {
        public bool ShouldShowInHelp => true;

        public Task ExecuteAsync(HandlerContext context, CancellationToken cancellation)
            => Func(context, cancellation);
    }
}
