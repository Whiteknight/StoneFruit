using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Utility;

namespace StoneFruit
{
    /// <summary>
    /// Dispatches control to the appropriate handler for a given verb.
    /// </summary>
    public class CommandDispatcher
    {
        /// <summary>
        /// The parser to turn strings into Commands
        /// </summary>
        public CommandParser Parser { get; }

        /// <summary>
        /// The source of handlers
        /// </summary>
        public IHandlerSource Commands { get; }

        /// <summary>
        /// The current environment and collection of all possible environments
        /// </summary>
        public IEnvironmentCollection Environments { get; }

        /// <summary>
        /// The execution state of the engine
        /// </summary>
        public EngineState State { get; }

        /// <summary>
        /// The output
        /// </summary>
        public IOutput Output { get; }

        public CommandDispatcher(CommandParser parser, IHandlerSource commands, IEnvironmentCollection environments, EngineState state, IOutput output)
        {
            Parser = parser;
            Commands = commands;
            Environments = environments;
            State = state;
            Output = output;
        }

        /// <summary>
        /// Find and execute the appropriate handler for the given command object or
        /// unparsed command string
        /// </summary>
        /// <param name="command"></param>
        /// <param name="tokenSource"></param>
        public void Execute(CommandObjectOrString command, CancellationTokenSource tokenSource = null)
        {
            Assert.ArgumentNotNull(command, nameof(command));
            if (command.Object != null)
                Execute(command.Object, tokenSource);
            else if (!string.IsNullOrEmpty(command.String))
                Execute(command.String, tokenSource);
        }

        /// <summary>
        /// Find and execute the appropriate handler for the given unparsed command string
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="tokenSource"></param>
        public void Execute(string commandString, CancellationTokenSource tokenSource = null)
        {
            Assert.ArgumentNotNullOrEmpty(commandString, nameof(commandString));
            var Command = Parser.ParseCommand(commandString);
            Execute(Command, tokenSource);
        }

        /// <summary>
        /// Find and execute the appropriate handler for the given verb and arguments
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="args"></param>
        /// <param name="tokenSource"></param>
        public void Execute(string verb, IArguments args, CancellationTokenSource tokenSource = null)
        {
            Assert.ArgumentNotNullOrEmpty(verb, nameof(verb));
            var command = Command.Create(verb, args);
            Execute(command, tokenSource);
        }

        /// <summary>
        /// Find and execute the appropriate handler for the given Command object
        /// </summary>
        /// <param name="command"></param>
        /// <param name="tokenSource"></param>
        public void Execute(Command command, CancellationTokenSource tokenSource = null)
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