using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Utility;

namespace StoneFruit.Execution.HandlerSources
{
    public class InstanceMethodHandlerSource : IHandlerSource
    {
        // TODO: Unit tests
        private readonly object _instance;
        private readonly Func<string, string> _getDescription;
        private readonly Func<string, string> _getUsage;
        private readonly Dictionary<string, MethodInfo> _methods;

        public InstanceMethodHandlerSource(object instance, Func<string, string> getDescription, Func<string, string> getUsage)
        {
            _instance = instance;
            _getDescription = getDescription ?? (s => string.Empty);
            _getUsage = getUsage ?? (s => string.Empty);
            _methods = _instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.ReturnType == typeof(void) || m.ReturnType == typeof(Task))
                .ToDictionaryUnique(m => m.Name.ToLowerInvariant(), m => m);
        }

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher)
        {
            if (!_methods.ContainsKey(command.Verb))
                return null;
            var method = _methods[command.Verb];
            if (method.ReturnType == typeof(void))
                return new SyncHandlerWrapper(_instance, method, command, dispatcher);
            if (method.ReturnType == typeof(Task))
                return new AsyncHandlerWrapper(_instance, method, command, dispatcher);

            return null;
        }

        public IEnumerable<IVerbInfo> GetAll()
        {
            return _methods.Select(kvp => new MethodInfoVerbInfo(kvp.Key, kvp.Value, _getDescription, _getUsage));
        }

        public IVerbInfo GetByName(string name)
        {
            name = name.ToLowerInvariant();
            if (!_methods.ContainsKey(name))
                return null;
            return new MethodInfoVerbInfo(name, _methods[name], _getDescription, _getUsage);
        }

        private class MethodInfoVerbInfo : IVerbInfo
        {
            private readonly MethodInfo _method;
            private readonly Func<string, string> _getDescription;
            private readonly Func<string, string> _getUsage;

            public MethodInfoVerbInfo(string verb, MethodInfo method, Func<string, string> getDescription, Func<string, string> getUsage)
            {
                Verb = verb;
                _method = method;
                _getDescription = getDescription;
                _getUsage = getUsage;
            }

            public string Verb { get; }
            public string Description => _getDescription(Verb);
            public string Usage => _getUsage(Verb);
            public bool ShouldShowInHelp => true;
        }

        private class SyncHandlerWrapper : IHandler
        {
            private readonly object _instance;
            private readonly MethodInfo _method;
            private readonly Command _command;
            private readonly CommandDispatcher _dispatcher;

            public SyncHandlerWrapper(object instance, MethodInfo method, Command command, CommandDispatcher dispatcher)
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

                var args = new object[parameters.Length];
                var fetcher = new ArgumentValueFetcher(_command, _dispatcher);
                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    args[i] = fetcher.GetValue(parameter.ParameterType, parameter.Name, i);
                }

                _method.Invoke(_instance, args);
            }
        }

        private class AsyncHandlerWrapper : IAsyncHandler
        {
            private readonly object _instance;
            private readonly MethodInfo _method;
            private readonly Command _command;
            private readonly CommandDispatcher _dispatcher;

            public AsyncHandlerWrapper(object instance, MethodInfo method, Command command, CommandDispatcher dispatcher)
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

                var args = new object[parameters.Length];
                var fetcher = new ArgumentValueFetcher(_command, _dispatcher);
                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    args[i] = fetcher.GetValue(parameter.ParameterType, parameter.Name, i);
                }

                return _method.Invoke(_instance, args) as Task;
            }
        }
    }
}
