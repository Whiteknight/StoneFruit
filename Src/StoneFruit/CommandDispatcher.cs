using System;
using System.Threading;
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

        private void Execute(CompleteCommand completeCommand, CancellationTokenSource tokenSource = null)
        {
            Assert.ArgumentNotNull(completeCommand, nameof(completeCommand));
            var handler = Commands.GetInstance(completeCommand, this) ?? new NotFoundHandler(completeCommand, State, Output);
            var syncHandler = GetSynchronousHandler(tokenSource, handler);
            syncHandler.Execute();
        }

        private static ICommandHandler GetSynchronousHandler(CancellationTokenSource tokenSource, ICommandHandlerBase verbObject)
        {
            if (verbObject is ICommandHandler syncVerb)
                return syncVerb;

            tokenSource ??= new CancellationTokenSource();
            return new AsyncDispatchHandler(verbObject as ICommandHandlerAsync, tokenSource);
        }

        public void Execute(string commandString, CancellationTokenSource tokenSource = null)
        {
            Assert.ArgumentNotNullOrEmpty(commandString, nameof(commandString));
            var completeCommand = Parser.ParseCommand(commandString);
            Execute(completeCommand, tokenSource);
        }

        public void Execute(string verb, CommandArguments args, CancellationTokenSource tokenSource = null)
        {
            Assert.ArgumentNotNullOrEmpty(verb, nameof(verb));
            var completeCommand = new CompleteCommand(verb, args);
            Execute(completeCommand, tokenSource);
        }

        private class AsyncDispatchHandler : ICommandHandler
        {
            private readonly ICommandHandlerAsync _asyncHandler;
            private readonly CancellationTokenSource _tokenSource;

            public AsyncDispatchHandler(ICommandHandlerAsync asyncHandler, CancellationTokenSource tokenSource)
            {
                _asyncHandler = asyncHandler;
                _tokenSource = tokenSource;
            }

            public void Execute()
            {
                if (_asyncHandler == null)
                    return;
                var token = _tokenSource.Token;
                Task.Run(async () => await _asyncHandler.ExecuteAsync(token), token).ConfigureAwait(false).GetAwaiter().GetResult();
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