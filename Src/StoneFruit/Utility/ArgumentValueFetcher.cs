using System;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Utility
{
    public class ArgumentValueFetcher
    {
        private readonly Command _command;
        private readonly CommandDispatcher _dispatcher;

        public ArgumentValueFetcher(Command command, CommandDispatcher dispatcher)
        {
            _command = command;
            _dispatcher = dispatcher;
        }

        public object GetValue(Type type, string name, int index)
        {
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
            if (type == typeof(ICommandArguments))
                return _command.Arguments;
            if (_dispatcher.Environments?.Current?.GetType() != null && type == _dispatcher.Environments.Current.GetType())
                return _dispatcher.Environments.Current;

            // TODO: If we don't have it by name look it up by index
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