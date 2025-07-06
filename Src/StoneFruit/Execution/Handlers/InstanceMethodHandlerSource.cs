using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Trie;
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

    public Maybe<IHandlerBase> GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        => _methods.Get(arguments)
            .Bind(value => GetInstanceInternal(arguments, dispatcher, value));

    private Maybe<IHandlerBase> GetInstanceInternal(IArguments arguments, CommandDispatcher dispatcher, MethodInfo value)
    {
        if (value.ReturnType == typeof(void))
            return new SyncHandlerWrapper(_instance, value, arguments, dispatcher, _invoker);
        if (value.ReturnType == typeof(Task))
            return new AsyncHandlerWrapper(_instance, value, arguments, dispatcher, _invoker);
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

    private sealed class MethodInfoVerbInfo : IVerbInfo
    {
        private readonly MethodInfo _method;

        public MethodInfoVerbInfo(Verb verb, MethodInfo method)
        {
            Verb = verb;
            _method = method;
        }

        public Verb Verb { get; }
        public string Description => _method.GetDescriptionAttributeValue() ?? string.Empty;
        public string Usage => _method.GetUsageAttributeValue() ?? Description;
        public string Group => _method.GetGroupAttributeValue() ?? string.Empty;
        public bool ShouldShowInHelp => true;
    }

    private sealed class SyncHandlerWrapper : IHandler
    {
        private readonly object _instance;
        private readonly MethodInfo _method;
        private readonly IArguments _command;
        private readonly CommandDispatcher _dispatcher;
        private readonly IHandlerMethodInvoker _invoker;

        public SyncHandlerWrapper(object instance, MethodInfo method, IArguments command, CommandDispatcher dispatcher, IHandlerMethodInvoker invoker)
        {
            _instance = instance;
            _method = method;
            _command = command;
            _dispatcher = dispatcher;
            _invoker = invoker;
        }

        public void Execute()
        {
            _invoker.Invoke(_instance, _method, _command, _dispatcher, CancellationToken.None);
        }
    }

    private sealed class AsyncHandlerWrapper : IAsyncHandler
    {
        private readonly object _instance;
        private readonly MethodInfo _method;
        private readonly IArguments _command;
        private readonly CommandDispatcher _dispatcher;
        private readonly IHandlerMethodInvoker _invoker;

        public AsyncHandlerWrapper(object instance, MethodInfo method, IArguments command, CommandDispatcher dispatcher, IHandlerMethodInvoker invoker)
        {
            Debug.Assert(method.ReturnType == typeof(Task), "Must be using a Task return type");
            _instance = instance;
            _method = method;
            _command = command;
            _dispatcher = dispatcher;
            _invoker = invoker;
        }

        public Task ExecuteAsync(CancellationToken cancellation)
            => _invoker.InvokeAsync(_instance, _method, _command, _dispatcher, cancellation);
    }
}
