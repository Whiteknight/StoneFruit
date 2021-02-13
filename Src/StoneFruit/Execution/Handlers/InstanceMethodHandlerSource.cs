using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Handler source for public methods on a pre-constructed object instance
    /// </summary>
    public class InstanceMethodHandlerSource : IHandlerSource
    {
        private readonly object _instance;
        private readonly Func<string, string> _getDescription;
        private readonly Func<string, string> _getUsage;
        private readonly Func<string, string> _getGroup;
        private readonly VerbTrie<MethodInfo> _methods;

        public InstanceMethodHandlerSource(object instance, Func<string, string>? getDescription, Func<string, string>? getUsage, Func<string, string>? getGroup, IVerbExtractor verbExtractor)
        {
            Assert.ArgumentNotNull(instance, nameof(instance));
            _instance = instance;
            _getDescription = getDescription ?? (_ => string.Empty);
            _getUsage = getUsage ?? (_ => string.Empty);
            _getGroup = getGroup ?? (_ => string.Empty);
            _methods = _instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.ReturnType == typeof(void) || m.ReturnType == typeof(Task))
                .SelectMany(m => verbExtractor
                    .GetVerbs(m)
                    .Select(v => (Method: m, Verb: v))
                )
                .ToVerbTrie(x => x.Verb, x => x.Method);
        }

        public IResult<IHandlerBase> GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        {
            var method = _methods.Get(arguments);
            if (!method.HasValue)
                return FailureResult<IHandlerBase>.Instance;

            if (method.Value.ReturnType == typeof(void))
                return new SuccessResult<IHandlerBase>(new SyncHandlerWrapper(_instance, method.Value, arguments, dispatcher));
            if (method.Value.ReturnType == typeof(Task))
                return new SuccessResult<IHandlerBase>(new AsyncHandlerWrapper(_instance, method.Value, arguments, dispatcher));

            return FailureResult<IHandlerBase>.Instance;
        }

        public IEnumerable<IVerbInfo> GetAll()
            => _methods.GetAll()
                .Select(kvp => new MethodInfoVerbInfo(kvp.Key, _getDescription, _getUsage, _getGroup));

        // Since we're using the name of a method as the verb, and you can't nest methods, the
        // verb must only be a single string. Anything else is a non-match
        public IResult<IVerbInfo> GetByName(Verb verb)
        {
            var method = _methods.Get(verb);
            if (!method.HasValue)
                return FailureResult<IVerbInfo>.Instance;
            return new SuccessResult<IVerbInfo>(new MethodInfoVerbInfo(verb, _getDescription, _getUsage, _getGroup));
        }

        private static object? InvokeMethod(object instance, MethodInfo method, ParameterInfo[] parameters, IArguments command, CommandDispatcher dispatcher, CancellationToken token)
        {
            var args = new object?[parameters.Length];
            var fetcher = new ArgumentValueFetcher(command, dispatcher, token);
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                args[i] = fetcher.GetValue(parameter.ParameterType, parameter.Name ?? "", i);
            }

            return method.Invoke(instance, args);
        }

        private class MethodInfoVerbInfo : IVerbInfo
        {
            private readonly Func<string, string> _getDescription;
            private readonly Func<string, string> _getUsage;
            private readonly Func<string, string> _getGroup;

            public MethodInfoVerbInfo(Verb verb, Func<string, string> getDescription, Func<string, string> getUsage, Func<string, string> getGroup)
            {
                Verb = verb;
                _getDescription = getDescription;
                _getUsage = getUsage;
                _getGroup = getGroup;
            }

            public Verb Verb { get; }
            public string Description => _getDescription(Verb.ToString());
            public string Usage => _getUsage(Verb.ToString());
            public string Group => _getGroup(Verb.ToString());
            public bool ShouldShowInHelp => true;
        }

        private class SyncHandlerWrapper : IHandler
        {
            private readonly object _instance;
            private readonly MethodInfo _method;
            private readonly IArguments _command;
            private readonly CommandDispatcher _dispatcher;

            public SyncHandlerWrapper(object instance, MethodInfo method, IArguments command, CommandDispatcher dispatcher)
            {
                _instance = instance;
                _method = method;
                _command = command;
                _dispatcher = dispatcher;
            }

            public void Execute()
            {
                var parameters = _method.GetParameters();
                if (parameters.Length == 0)
                {
                    _method.Invoke(_instance, new object[0]);
                    return;
                }

                InvokeMethod(_instance, _method, parameters, _command, _dispatcher, CancellationToken.None);
            }
        }

        private class AsyncHandlerWrapper : IAsyncHandler
        {
            private readonly object _instance;
            private readonly MethodInfo _method;
            private readonly IArguments _command;
            private readonly CommandDispatcher _dispatcher;

            public AsyncHandlerWrapper(object instance, MethodInfo method, IArguments command, CommandDispatcher dispatcher)
            {
                Debug.Assert(method.ReturnType == typeof(Task));
                _instance = instance;
                _method = method;
                _command = command;
                _dispatcher = dispatcher;
            }

            public Task ExecuteAsync(CancellationToken cancellation)
            {
                var parameters = _method.GetParameters();
                if (parameters.Length == 0)
                {
                    var result = _method.Invoke(_instance, new object[0]);
                    return (result as Task) ?? Task.CompletedTask;
                }

                return (InvokeMethod(_instance, _method, parameters, _command, _dispatcher, cancellation) as Task) ?? Task.CompletedTask;
            }
        }
    }
}
