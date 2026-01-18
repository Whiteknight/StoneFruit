using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Execution.IO;
using StoneFruit.Execution.Trie;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers;

public sealed record DelegateHandlerRegistration(Delegate Func, Verb Verb, string Description, string Usage, string Group);

/// <summary>
/// Handler source for function delegates.
/// </summary>
public class DelegateHandlerSource : IHandlerSource
{
    private readonly VerbTrie<IHandlerBase> _handlers;

    public DelegateHandlerSource(IHandlerMethodInvoker invoker, IObjectOutputWriter writer, IEnumerable<DelegateHandlerRegistration> handlers)
    {
        _handlers = handlers
            .Select(h => (Handler: CreateHandler(invoker, writer, h.Verb, h.Func, h.Description, h.Usage, h.Group), Verb: h.Verb))
            .ToVerbTrie(h => h.Verb, h => h.Handler);
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

    private static IHandlerBase CreateHandler(IHandlerMethodInvoker invoker, IObjectOutputWriter writer, Verb verb, Delegate func, string description, string usage, string group)
    {
        return func.Method.IsAsync()
            ? new AsyncHandler(invoker, writer, func, verb, description, usage, group)
            : new Handler(invoker, writer, func, verb, description, usage, group);
    }

    private sealed record Handler(IHandlerMethodInvoker Invoker, IObjectOutputWriter Writer, Delegate Func, Verb Verb, string Description, string Usage, string Group)
        : IHandler, IVerbInfo
    {
        public bool ShouldShowInHelp => true;

        public void Execute(IArguments arguments, HandlerContext context)
        {
            var result = Invoker.Invoke(Func, context);
            Writer.MaybeWriteObject(result);
        }
    }

    private sealed record AsyncHandler(IHandlerMethodInvoker Invoker, IObjectOutputWriter Writer, Delegate Func, Verb Verb, string Description, string Usage, string Group)
        : IAsyncHandler, IVerbInfo
    {
        public bool ShouldShowInHelp => true;

        public async Task ExecuteAsync(IArguments arguments, HandlerContext context, CancellationToken cancellation)
        {
            var result = await Invoker.InvokeAsync(Func, context, cancellation);
            Writer.MaybeWriteObject(result);
        }
    }
}
