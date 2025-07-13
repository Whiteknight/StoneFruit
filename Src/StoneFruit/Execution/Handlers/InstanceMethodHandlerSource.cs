using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Execution.Trie;
using StoneFruit.Utility;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Handler source for public methods on a pre-constructed object instance.
/// </summary>
public class InstanceMethodHandlerSource : IHandlerSource
{
    private readonly object _instance;
    private readonly VerbTrie<IHandlerBase> _methods;
    private readonly IHandlerMethodInvoker _invoker;

    public InstanceMethodHandlerSource(object instance, IHandlerMethodInvoker invoker, IVerbExtractor verbExtractor)
    {
        _invoker = NotNull(invoker);
        _instance = NotNull(instance);
        _methods = _instance.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(m => m.ReturnType == typeof(void) || m.ReturnType == typeof(Task))
            .SelectMany(m => verbExtractor
                .GetVerbs(m)
                .Select(v => (Handler: CreateHandlerInstance(v, m), Verb: v))
            )
            .ToVerbTrie(x => x.Verb, x => x.Handler);
    }

    public Maybe<IHandlerBase> GetInstance(HandlerContext context)
        => _methods.Get(context.Arguments);

    public IEnumerable<IVerbInfo> GetAll()
        => _methods.GetAll()
            .Select(kvp => kvp.Value)
            .Cast<IVerbInfo>();

    // Since we're using the name of a method as the verb, and you can't nest methods, the
    // verb must only be a single string. Anything else is a non-match
    public Maybe<IVerbInfo> GetByName(Verb verb)
        => _methods.Get(verb)
            .Map(v => (IVerbInfo)v);

    private IHandlerBase CreateHandlerInstance(Verb verb, MethodInfo value)
    {
        if (value.ReturnType == typeof(void))
            return new SyncHandlerWrapper(verb, _instance, value, _invoker);
        if (value.ReturnType == typeof(Task))
            return new AsyncHandlerWrapper(verb, _instance, value, _invoker);
        throw new InvalidOperationException("Invalid delegate type");
    }

    private sealed record SyncHandlerWrapper(Verb Verb, object Instance, MethodInfo Method, IHandlerMethodInvoker Invoker)
        : IHandlerWithContext,
        IVerbInfo
    {
        public string Description => Method.GetDescriptionAttributeValue() ?? string.Empty;
        public string Usage => Method.GetUsageAttributeValue() ?? Description;
        public string Group => Method.GetGroupAttributeValue() ?? string.Empty;
        public bool ShouldShowInHelp => true;

        public void Execute(HandlerContext context)
            => Invoker.Invoke(Instance, Method, context);
    }

    private sealed record AsyncHandlerWrapper(Verb Verb, object Instance, MethodInfo Method, IHandlerMethodInvoker Invoker)
        : IAsyncHandlerWithContext,
        IVerbInfo
    {
        public string Description => Method.GetDescriptionAttributeValue() ?? string.Empty;
        public string Usage => Method.GetUsageAttributeValue() ?? Description;
        public string Group => Method.GetGroupAttributeValue() ?? string.Empty;
        public bool ShouldShowInHelp => true;

        public Task ExecuteAsync(HandlerContext context, CancellationToken cancellation)
            => Invoker.InvokeAsync(Instance, Method, context, cancellation);
    }
}
