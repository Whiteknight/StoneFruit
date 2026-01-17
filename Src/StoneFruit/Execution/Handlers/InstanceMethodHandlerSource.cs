using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Execution.Exceptions;
using StoneFruit.Execution.Trie;
using StoneFruit.Utility;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Handler source for public methods on a pre-constructed object instance.
/// </summary>
/// <typeparam name="T"></typeparam>
public class InstanceMethodHandlerSource<T> : IHandlerSource
{
    private readonly VerbTrie<MethodMetadata> _methods;
    private readonly IHandlerMethodInvoker _invoker;
    private readonly Func<T> _getInstance;

    public InstanceMethodHandlerSource(Func<T> getInstance, IHandlerMethodInvoker invoker, IVerbExtractor verbExtractor, string? group)
    {
        _invoker = NotNull(invoker);
        _methods = typeof(T)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(m => m.ReturnType == typeof(void) || m.ReturnType == typeof(Task))
            .SelectMany(m => verbExtractor
                .GetVerbs(m)
                .Match(
                    verbs => verbs.Select(v => (Handler: WrapMethodMetadata(v, m, group), Verb: v)),
                    _ => [])
            )
            .ToVerbTrie(x => x.Verb, x => x.Handler);
        _getInstance = NotNull(getInstance);
    }

    public Maybe<IHandlerBase> GetInstance(HandlerContext context)
        => _methods.Get(context.Arguments).Map<IHandlerBase>(mm =>
        {
            if (mm.Method.ReturnType == typeof(void))
                return new SyncHandler(mm, _getInstance, _invoker);
            if (mm.Method.ReturnType == typeof(Task))
                return new AsyncHandler(mm, _getInstance, _invoker);
            throw new ExecutionException("Method does not have a valid return type");
        });

    public IEnumerable<IVerbInfo> GetAll()
        => _methods.GetAll()
            .Select(kvp => kvp.Value);

    // Since we're using the name of a method as the verb, and you can't nest methods, the
    // verb must only be a single string. Anything else is a non-match
    public Maybe<IVerbInfo> GetByName(Verb verb)
        => _methods.Get(verb).Map(mm => (IVerbInfo)mm);

    private static MethodMetadata WrapMethodMetadata(Verb verb, MethodInfo method, string? group)
    {
        var description = method.GetDescriptionAttributeValue() ?? string.Empty;
        var usage = method.GetUsageAttributeValue() ?? description;
        group = (string.IsNullOrEmpty(group) ? method.GetGroupAttributeValue() : group) ?? string.Empty;
        return new MethodMetadata(verb, method, description, usage, group);
    }

    private sealed record MethodMetadata(Verb Verb, MethodInfo Method, string Description, string Usage, string Group)
        : IVerbInfo
    {
        public bool ShouldShowInHelp => true;
    }

    private sealed record SyncHandler(MethodMetadata Method, Func<T> GetInstance, IHandlerMethodInvoker Invoker)
        : IHandler
    {
        public void Execute(IArguments arguments, HandlerContext context)
        {
            var instance = GetInstance() ?? throw new ExecutionException("Instance could not be resolved");
            Invoker.Invoke(instance, Method.Method, context);
        }
    }

    private sealed record AsyncHandler(MethodMetadata Method, Func<T> GetInstance, IHandlerMethodInvoker Invoker)
        : IAsyncHandler
    {
        public Task ExecuteAsync(IArguments arguments, HandlerContext context, CancellationToken cancellation)
        {
            var instance = GetInstance() ?? throw new ExecutionException("Instance could not be resolved");
            return Invoker.InvokeAsync(instance, Method.Method, context, cancellation);
        }
    }
}
