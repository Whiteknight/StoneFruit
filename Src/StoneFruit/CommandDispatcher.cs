using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Scripts;
using StoneFruit.Utility;

namespace StoneFruit
{
    /// <summary>
    /// Dispatches control to the appropriate handler for a given verb.
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

        public void Execute(CommandObjectOrString command, CancellationTokenSource tokenSource = null)
        {
            Assert.ArgumentNotNull(command, nameof(command));
            if (command.Object != null)
                Execute(command.Object, tokenSource);
            else if (!string.IsNullOrEmpty(command.String))
                Execute(command.String, tokenSource);
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

        private void Execute(Command command, CancellationTokenSource tokenSource = null)
        {
            Assert.ArgumentNotNull(command, nameof(command));
            var handler = Commands.GetInstance(command, this) ?? throw new VerbNotFoundException(command.Verb);
            var syncHandler = GetSynchronousHandler(tokenSource, handler);
            // TODO: If the handler has a second Execute() method with arguments, we should attempt to invoke
            // that version instead (converting named arguments to method arguments).
            syncHandler.Execute();
        }

        // TODO: Do it the other way around, convert IHandler->IAsyncHandler and invoke asynchronously
        private static IHandler GetSynchronousHandler(CancellationTokenSource tokenSource, IHandlerBase verbObject)
        {
            if (verbObject is IHandler syncVerb)
                return syncVerb;

            if (verbObject is IAsyncHandler asyncHandler)
            {
                tokenSource ??= new CancellationTokenSource();
                return new AsyncDispatchHandler(asyncHandler, tokenSource);
            }

            return null;
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
    }
}