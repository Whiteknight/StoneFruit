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
        public IHandlerSource Commands { get; }
        public IEnvironmentCollection Environments { get; }
        public EngineState State { get; }
        public IOutput Output { get; }

        public CommandDispatcher(CommandParser parser, IHandlerSource commands, IEnvironmentCollection environments, EngineState state, IOutput output)
        {
            Parser = parser;
            Commands = commands;
            Environments = environments;
            State = state;
            Output = output;
        }

        private void Execute(Command command, CancellationTokenSource tokenSource = null)
        {
            Assert.ArgumentNotNull(command, nameof(command));
            var handler = Commands.GetInstance(command, this) ?? new NotFoundHandler(command, State, Output);
            var syncHandler = GetSynchronousHandler(tokenSource, handler);
            syncHandler.Execute();
        }

        private static IHandler GetSynchronousHandler(CancellationTokenSource tokenSource, IHandlerBase verbObject)
        {
            if (verbObject is IHandler syncVerb)
                return syncVerb;

            tokenSource ??= new CancellationTokenSource();
            return new AsyncDispatchHandler(verbObject as IAsyncHandler, tokenSource);
        }

        public void Execute(string commandString, CancellationTokenSource tokenSource = null)
        {
            Assert.ArgumentNotNullOrEmpty(commandString, nameof(commandString));
            var Command = Parser.ParseCommand(commandString);
            Execute(Command, tokenSource);
        }

        public void Execute(string verb, CommandArguments args, CancellationTokenSource tokenSource = null)
        {
            Assert.ArgumentNotNullOrEmpty(verb, nameof(verb));
            var Command = new Command(verb, args);
            Execute(Command, tokenSource);
        }

        private class AsyncDispatchHandler : IHandler
        {
            private readonly IAsyncHandler _asyncHandler;
            private readonly CancellationTokenSource _tokenSource;

            public AsyncDispatchHandler(IAsyncHandler asyncHandler, CancellationTokenSource tokenSource)
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

        private class NotFoundHandler : IHandler
        {
            private readonly Command _command;
            private readonly EngineState _state;
            private readonly IOutput _output;

            public NotFoundHandler(Command command, EngineState state, IOutput output)
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