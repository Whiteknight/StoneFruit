using System;
using System.Threading.Tasks;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Utility;

namespace StoneFruit
{
    /// <summary>
    /// Handles dispatch of commands to appropriate ICommandVerb objects
    /// </summary>
    public class CommandDispatcher
    {
        public CommandParser Parser { get; }
        public ICommandHandlerSource Commands { get; }
        public IEnvironmentCollection Environments { get; }
        public EngineState State { get; }
        public ITerminalOutput Output { get; }

        public CommandDispatcher(CommandParser parser, ICommandHandlerSource commands, IEnvironmentCollection environments, EngineState state, ITerminalOutput output)
        {
            Parser = parser;
            Commands = commands;
            Environments = environments;
            State = state;
            Output = output;
        }

        private void Execute(CompleteCommand completeCommand)
        {
            Assert.ArgumentNotNull(completeCommand, nameof(completeCommand));
            var verbObject = Commands.GetInstance(completeCommand, this) ?? new NotFoundHandler(completeCommand, State, Output);
            var syncVerb = (verbObject as ICommandHandler) ?? new AsyncDispatchHandler(verbObject as ICommandHandlerAsync);
            syncVerb.Execute();
        }

        public void Execute(string commandString)
        {
            Assert.ArgumentNotNullOrEmpty(commandString, nameof(commandString));
            var completeCommand = Parser.ParseCommand(commandString);
            Execute(completeCommand);
        }

        public void Execute(string verb, CommandArguments args)
        {
            Assert.ArgumentNotNullOrEmpty(verb, nameof(verb));
            var completeCommand = new CompleteCommand(verb, args);
            Execute(completeCommand);
        }

        private class AsyncDispatchHandler : ICommandHandler
        {
            private readonly ICommandHandlerAsync _asyncHandler;

            public AsyncDispatchHandler(ICommandHandlerAsync asyncHandler)
            {
                _asyncHandler = asyncHandler;
            }

            public void Execute()
            {
                if (_asyncHandler == null)
                    return;
                Task.Run(async () => await _asyncHandler.ExecuteAsync()).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        private class NotFoundHandler : ICommandHandler
        {
            private readonly CompleteCommand _command;
            private readonly EngineState _state;
            private readonly ITerminalOutput _output;

            public NotFoundHandler(CompleteCommand command, EngineState state, ITerminalOutput output)
            {
                _command = command;
                _state = state;
                _output = output;
            }

            public void Execute()
            {
                _output
                    .Color(ConsoleColor.Red)
                    .WriteLine($"Command '{_command.Verb}' not found. Please check your spelling or help output and try again");
                _state.AddCommands(_state.EventCatalog.VerbNotFound.GetCommands());
            }
        }
    }
}