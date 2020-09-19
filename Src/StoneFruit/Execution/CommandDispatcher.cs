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
        public void Execute(CommandOrString command, CancellationToken token = default)
        {
            Assert.ArgumentNotNull(command, nameof(command));
            if (command.Object != null)
                Execute(command.Object, token);
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
        public void Execute(string verb, IArguments args, CancellationToken token = default)
        {
            Assert.ArgumentNotNullOrEmpty(verb, nameof(verb));
            var command = Command.Create(verb, args);
            Execute(command, token);
        }

        /// <summary>
        /// Find and execute the appropriate handler for the given Command object
        /// </summary>
        /// <param name="command"></param>
        /// <param name="token"></param>
        public void Execute(Command command, CancellationToken token = default)
        {
            Assert.ArgumentNotNull(command, nameof(command));
            State.CurrentCommand = command;
            var handler = Handlers.GetInstance(command, this) ?? throw new VerbNotFoundException(command.Verb);
            if (handler is IHandler syncHandler)
            {
                syncHandler.Execute();
                State.CurrentCommand = null;
                return;
            }

            if (handler is IAsyncHandler asyncHandler)
            {
                Task.Run(async () => await asyncHandler.ExecuteAsync(token), token)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
                State.CurrentCommand = null;
            }
        }

        /// <summary>
        /// Find and execute the appropriate handler for the given command object or
        /// unparsed command string
        /// </summary>
        /// <param name="command"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task ExecuteAsync(CommandOrString command, CancellationToken token = default)
        {
            Assert.ArgumentNotNull(command, nameof(command));
            if (command.Object != null)
                return ExecuteAsync(command.Object, token);
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
        public Task ExecuteAsync(string verb, IArguments args, CancellationToken token = default)
        {
            Assert.ArgumentNotNullOrEmpty(verb, nameof(verb));
            var command = Command.Create(verb, args);
            return ExecuteAsync(command, token);
        }

        /// <summary>
        /// Find and execute the appropriate handler for the given Command object
        /// </summary>
        /// <param name="command"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(Command command, CancellationToken token = default)
        {
            Assert.ArgumentNotNull(command, nameof(command));
            State.CurrentCommand = command;
            var handler = Handlers.GetInstance(command, this) ?? throw new VerbNotFoundException(command.Verb);
            if (handler is IHandler syncHandler)
            {
                syncHandler.Execute();
                State.CurrentCommand = null;
                return;
            }

            if (handler is IAsyncHandler asyncHandler)
            {
                await asyncHandler.ExecuteAsync(token);
                State.CurrentCommand = null;
            }
        }
    }
}