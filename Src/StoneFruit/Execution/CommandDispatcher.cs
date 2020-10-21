using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Handlers;
using StoneFruit.Utility;

namespace StoneFruit.Execution
{
    /// <summary>
    /// Dispatches control to the appropriate handler for a given verb. Used by the engine
    /// to dispatch commands and can be used from within a command to delegate to another
    /// command without returning to the engine runloop first.
    /// </summary>
    public class CommandDispatcher
    {
        public CommandDispatcher(ICommandParser parser, IHandlers handlers, IEnvironmentCollection environments, EngineState state, IOutput output)
        {
            Parser = parser;
            Handlers = handlers;
            Environments = environments;
            State = state;
            Output = output;
        }

        /// <summary>
        /// The parser to turn strings into Commands
        /// </summary>
        public ICommandParser Parser { get; }

        /// <summary>
        /// The source of handlers
        /// </summary>
        public IHandlers Handlers { get; }

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

        /// <summary>
        /// Find and execute the appropriate handler for the given command object or
        /// unparsed command string
        /// </summary>
        /// <param name="command"></param>
        /// <param name="token"></param>
        public void Execute(ArgumentsOrString command, CancellationToken token = default)
        {
            Assert.ArgumentNotNull(command, nameof(command));
            if (command.Arguments != null)
                Execute(command.Arguments, token);
            else if (!string.IsNullOrEmpty(command.String))
                Execute(command.String, token);
        }

        /// <summary>
        /// Find and execute the appropriate handler for the given unparsed command string
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="token"></param>
        public void Execute(string commandString, CancellationToken token = default)
        {
            Assert.ArgumentNotNullOrEmpty(commandString, nameof(commandString));
            var command = Parser.ParseCommand(commandString);
            Execute(command, token);
        }

        /// <summary>
        /// Find and execute the appropriate handler for the given verb and arguments
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="args"></param>
        /// <param name="token"></param>
        public void Execute(Verb verb, IArguments args, CancellationToken token = default)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            var newArgs = new PrependedVerbArguments(verb, args);
            Execute(newArgs, token);
        }

        /// <summary>
        /// Find and execute the appropriate handler for the given Command object
        /// </summary>
        /// <param name="command"></param>
        /// <param name="token"></param>
        public void Execute(IArguments arguments, CancellationToken token = default)
        {
            Assert.ArgumentNotNull(arguments, nameof(arguments));
            State.CurrentArguments = arguments;
            var handler = Handlers.GetInstance(arguments, this) ?? throw VerbNotFoundException.FromArguments(arguments);
            if (handler is IHandler syncHandler)
            {
                syncHandler.Execute();
                State.CurrentArguments = null;
                return;
            }

            if (handler is IAsyncHandler asyncHandler)
            {
                Task.Run(async () => await asyncHandler.ExecuteAsync(token), token)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
                State.CurrentArguments = null;
            }
        }

        /// <summary>
        /// Find and execute the appropriate handler for the given command object or
        /// unparsed command string
        /// </summary>
        /// <param name="command"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task ExecuteAsync(ArgumentsOrString command, CancellationToken token = default)
        {
            Assert.ArgumentNotNull(command, nameof(command));
            if (command.Arguments != null)
                return ExecuteAsync(command.Arguments, token);
            if (!string.IsNullOrEmpty(command.String))
                return ExecuteAsync(command.String, token);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Parse the command string, find and dispatch the appropriate handler.
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task ExecuteAsync(string commandString, CancellationToken token = default)
        {
            Assert.ArgumentNotNullOrEmpty(commandString, nameof(commandString));
            var command = Parser.ParseCommand(commandString);
            return ExecuteAsync(command, token);
        }

        /// <summary>
        /// Find and execute the appropriate handler for the given verb and arguments
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="args"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task ExecuteAsync(Verb verb, IArguments args, CancellationToken token = default)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            var newArgs = new PrependedVerbArguments(verb, args);
            return ExecuteAsync(newArgs, token);
        }

        /// <summary>
        /// Find and execute the appropriate handler for the given Command object
        /// </summary>
        /// <param name="command"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(IArguments arguments, CancellationToken token = default)
        {
            Assert.ArgumentNotNull(arguments, nameof(arguments));
            State.CurrentArguments = arguments;
            var handler = Handlers.GetInstance(arguments, this) ?? throw VerbNotFoundException.FromArguments(arguments);
            if (handler is IHandler syncHandler)
            {
                syncHandler.Execute();
                State.CurrentArguments = null;
                return;
            }

            if (handler is IAsyncHandler asyncHandler)
            {
                await asyncHandler.ExecuteAsync(token);
                State.CurrentArguments = null;
            }
        }
    }
}