using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Execution.Arguments;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Handler source for public methods on a pre-defined object instance
    /// </summary>
    public class InstanceMethodHandlerSource : IHandlerSource
    {
        private readonly object _instance;
        private readonly Func<string, string> _getDescription;
        private readonly Func<string, string> _getUsage;
        private readonly Func<string, string> _getGroup;
        private readonly Dictionary<string, MethodInfo> _methods;

        public InstanceMethodHandlerSource(object instance, Func<string, string> getDescription, Func<string, string> getUsage, Func<string, string> getGroup)
        {
            Assert.ArgumentNotNull(instance, nameof(instance));
            _instance = instance;
            _getDescription = getDescription ?? (s => string.Empty);
            _getUsage = getUsage ?? (s => string.Empty);
            _getGroup = getGroup ?? (s => string.Empty);
            _methods = _instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.ReturnType == typeof(void) || m.ReturnType == typeof(Task))
                .ToDictionaryUnique(m => m.Name.ToLowerInvariant(), m => m);
        }

        public IHandlerBase GetInstance(IArguments command, CommandDispatcher dispatcher)
        {
            var verbSource = command as IVerbSource;
            if (verbSource == null)
                return null;
            var candidate = verbSource.GetVerbCandidatePositionals().FirstOrDefault();
            if (candidate == null)
                return null;
            var verb = candidate.AsString();
            if (!_methods.ContainsKey(verb))
                return null;
            var method = _methods[verb];
            if (method.ReturnType == typeof(void))
            {
                verbSource.SetVerbCount(1);
                return new SyncHandlerWrapper(_instance, method, command, dispatcher);
            }
            if (method.ReturnType == typeof(Task))
            {
                verbSource.SetVerbCount(1);
                return new AsyncHandlerWrapper(_instance, method, command, dispatcher);
            }

            return null;
        }

        public IEnumerable<IVerbInfo> GetAll()
        {
            return _methods.Select(kvp => new MethodInfoVerbInfo(kvp.Key, _getDescription, _getUsage, _getGroup));
        }

        // Since we're using the name of a method as the verb, and you can't nest methods, the
        // verb must only be a single string. Anything else is a non-match
        public IVerbInfo GetByName(Verb verb)
        {
            if (verb.Count > 1)
                return null;
            var name = verb[0].ToLowerInvariant();
            return _methods.ContainsKey(name) ? new MethodInfoVerbInfo(name, _getDescription, _getUsage, _getGroup) : null;
        }

        private static object InvokeMethod(object instance, MethodInfo method, ParameterInfo[] parameters, IArguments command, CommandDispatcher dispatcher, CancellationToken token)
        {
            var args = new object[parameters.Length];
            var fetcher = new ArgumentValueFetcher(command, dispatcher, token);
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                args[i] = fetcher.GetValue(parameter.ParameterType, parameter.Name, i);
            }

            return method.Invoke(instance, args);
        }

        private class MethodInfoVerbInfo : IVerbInfo
        {
            private readonly string _verb;
            private readonly Func<string, string> _getDescription;
            private readonly Func<string, string> _getUsage;
            private readonly Func<string, string> _getGroup;

            public MethodInfoVerbInfo(string verb, Func<string, string> getDescription, Func<string, string> getUsage, Func<string, string> getGroup)
            {
                _verb = verb;
                _getDescription = getDescription;
                _getUsage = getUsage;
                _getGroup = getGroup;
            }

            public Verb Verb => new Verb(_verb);
            public string Description => _getDescription(_verb);
            public string Usage => _getUsage(_verb);
            public string Group => _getGroup(_verb);
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
                    return _method.Invoke(_instance, new object[0]) as Task;

                return InvokeMethod(_instance, _method, parameters, _command, _dispatcher, cancellation) as Task;
            }
        }
    }
}
