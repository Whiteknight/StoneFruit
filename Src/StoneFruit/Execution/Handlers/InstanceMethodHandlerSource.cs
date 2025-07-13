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
    private readonly VerbTrie<MethodInfo> _methods;
    private readonly IHandlerMethodInvoker _invoker;

    public InstanceMethodHandlerSource(object instance, IHandlerMethodInvoker invoker, IVerbExtractor verbExtractor)
    {
        _instance = NotNull(instance);
        _methods = _instance.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(m => m.ReturnType == typeof(void) || m.ReturnType == typeof(Task))
            .SelectMany(m => verbExtractor
                .GetVerbs(m)
                .Select(v => (Method: m, Verb: v))
            )
            .ToVerbTrie(x => x.Verb, x => x.Method);
        _invoker = NotNull(invoker);
    }

    public Maybe<IHandlerBase> GetInstance(HandlerContext context)
        => _methods.Get(context.Arguments)
            .Bind(value => GetInstanceInternal(context, value));

    private Maybe<IHandlerBase> GetInstanceInternal(HandlerContext context, MethodInfo value)
    {
        if (value.ReturnType == typeof(void))
            return new SyncHandlerWrapper(_instance, value, context, _invoker);
        if (value.ReturnType == typeof(Task))
            return new AsyncHandlerWrapper(_instance, value, context, _invoker);
        return default;
    }

    public IEnumerable<IVerbInfo> GetAll()
        => _methods.GetAll()
            .Select(kvp => new MethodInfoVerbInfo(kvp.Key, kvp.Value));

    // Since we're using the name of a method as the verb, and you can't nest methods, the
    // verb must only be a single string. Anything else is a non-match
    public Maybe<IVerbInfo> GetByName(Verb verb)
        => _methods.Get(verb)
            .Map(v => (IVerbInfo)new MethodInfoVerbInfo(verb, v));

    private sealed record MethodInfoVerbInfo(Verb Verb, MethodInfo Method) : IVerbInfo
    {
        public string Description => Method.GetDescriptionAttributeValue() ?? string.Empty;
        public string Usage => Method.GetUsageAttributeValue() ?? Description;
        public string Group => Method.GetGroupAttributeValue() ?? string.Empty;
        public bool ShouldShowInHelp => true;
    }

    private sealed record SyncHandlerWrapper(object Instance, MethodInfo Method, HandlerContext Context, IHandlerMethodInvoker Invoker) : IHandler
    {
        public void Execute()
            => Invoker.Invoke(Instance, Method, Context);
    }

    private sealed record AsyncHandlerWrapper(object Instance, MethodInfo Method, HandlerContext Context, IHandlerMethodInvoker Invoker) : IAsyncHandler
    {
        public Task ExecuteAsync(CancellationToken cancellation)
            => Invoker.InvokeAsync(Instance, Method, Context, cancellation);
    }
}
