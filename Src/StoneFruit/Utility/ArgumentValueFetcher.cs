using System;
using System.Threading;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Utility
{
    public class ArgumentValueFetcher
    {
        private readonly IArguments _arguments;
        private readonly CommandDispatcher _dispatcher;
        private readonly CancellationToken _token;

        public ArgumentValueFetcher(IArguments arguments, CommandDispatcher dispatcher, CancellationToken token)
        {
            _arguments = arguments;
            _dispatcher = dispatcher;
            _token = token;
        }

        public object? GetValue(Type type, string name, int index)
        {
            if (type == typeof(CancellationToken))
                return _token;
            if (type == typeof(CommandDispatcher))
                return _dispatcher;
            if (type == typeof(IEnvironmentCollection))
                return _dispatcher.Environments;
            if (type == typeof(EngineState))
                return _dispatcher.State;
            if (type == typeof(IOutput))
                return _dispatcher.Output;
            if (type == typeof(CommandParser))
                return _dispatcher.Parser;
            if (type == typeof(IArguments))
                return _arguments;
            if (_dispatcher.Environments?.Current?.GetType() != null && type == _dispatcher.Environments.Current.GetType())
                return _dispatcher.Environments.Current;

            if (type == typeof(bool))
                return _arguments.HasFlag(name);

            IValuedArgument arg = _arguments.Get(name);
            if (!arg.Exists())
                arg = _arguments.Get(index);
            if (!arg.Exists())
                return null;

            if (type == typeof(string))
                return arg.AsString();
            if (type == typeof(int))
                return arg.AsInt();

            return null;
        }
    }
}
