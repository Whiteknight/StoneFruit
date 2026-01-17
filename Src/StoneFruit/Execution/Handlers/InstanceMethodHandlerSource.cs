using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Execution.Exceptions;
using StoneFruit.Execution.IO;
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
    private readonly IObjectOutputWriter _writer;

    public InstanceMethodHandlerSource(Func<T> getInstance, IHandlerMethodInvoker invoker, IVerbExtractor verbExtractor, IObjectOutputWriter writer, string? group)
    {
        _invoker = NotNull(invoker);
        _methods = typeof(T)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .SelectMany(m => verbExtractor
                .GetVerbs(m)
                .Match(
                    verbs => verbs.Select(v => (Handler: WrapMethodMetadata(v, m, group), Verb: v)),
                    _ => [])
            )
            .ToVerbTrie(x => x.Verb, x => x.Handler);
        _getInstance = NotNull(getInstance);
        _writer = writer;
    }

    public Maybe<IHandlerBase> GetInstance(HandlerContext context)
        => _methods
            .Get(context.Arguments)
            .Map<IHandlerBase>(mm => mm.Method.IsAsync()
                ? new AsyncHandler(mm, _getInstance, _invoker, _writer)
                : new SyncHandler(mm, _getInstance, _invoker, _writer));

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

    private sealed record SyncHandler(MethodMetadata Method, Func<T> GetInstance, IHandlerMethodInvoker Invoker, IObjectOutputWriter Writer)
        : IHandler
    {
        public void Execute(IArguments arguments, HandlerContext context)
        {
            var instance = GetInstance() ?? throw new ExecutionException("Instance could not be resolved");
            var result = Invoker.Invoke(instance, Method.Method, context);
            Writer.MaybeWriteObject(result);
        }
    }

    private sealed record AsyncHandler(MethodMetadata Method, Func<T> GetInstance, IHandlerMethodInvoker Invoker, IObjectOutputWriter Writer)
        : IAsyncHandler
    {
        public async Task ExecuteAsync(IArguments arguments, HandlerContext context, CancellationToken cancellation)
        {
            var instance = GetInstance() ?? throw new ExecutionException("Instance could not be resolved");
            var result = await Invoker.InvokeAsync(instance, Method.Method, context, cancellation);
            Writer.MaybeWriteObject(result);
        }
    }
}
