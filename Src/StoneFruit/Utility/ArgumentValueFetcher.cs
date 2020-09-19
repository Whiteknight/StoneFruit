using System;
using System.Threading;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Utility
{
    public class ArgumentValueFetcher
    {
        private readonly Command _command;
        private readonly CommandDispatcher _dispatcher;
        private readonly CancellationToken _token;

        public ArgumentValueFetcher(Command command, CommandDispatcher dispatcher, CancellationToken token)
        {
            _command = command;
            _dispatcher = dispatcher;
            _token = token;
        }

        public object GetValue(Type type, string name, int index)
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
            if (type == typeof(Command))
                return _command;
            if (type == typeof(IArguments))
                return _command.Arguments;
            if (_dispatcher.Environments?.Current?.GetType() != null && type == _dispatcher.Environments.Current.GetType())
                return _dispatcher.Environments.Current;

            if (type == typeof(string))
                return _command.Arguments.Get(name).AsString();
            if (type == typeof(int))
                return _command.Arguments.Get(name).AsInt();
            if (type == typeof(bool))
                return _command.Arguments.HasFlag(name);

            return null;
        }
    }
}