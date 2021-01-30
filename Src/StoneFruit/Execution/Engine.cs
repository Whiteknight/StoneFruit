using System;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.CommandSources;
using StoneFruit.Execution.Handlers;
using StoneFruit.Handlers;
using StoneFruit.Utility;

namespace StoneFruit.Execution
{
    public class EngineAccessor
    {
        private Engine? _engine;

        public Engine Engine => _engine ?? throw new InvalidOperationException("Cannot access engine because one has not been created");

        public void SetEngine(Engine engine)
        {
            _engine = engine;
        }
    }

    /// <summary>
    /// The execution core. Provides a run loop to receive user commands and execute them
    /// </summary>
    public class Engine
    {
        private readonly IHandlers _handlers;
        private readonly EngineEventCatalog _eventCatalog;
        private readonly EngineSettings _settings;
        private readonly ICommandParser _parser;

        private EngineState? _state;
        private CommandDispatcher? _dispatcher;

        public Engine(IHandlers handlers, IEnvironmentCollection environments, ICommandParser parser, IOutput output, EngineEventCatalog eventCatalog, EngineSettings settings)
        {
            Assert.ArgumentNotNull(handlers, nameof(handlers));
            Assert.ArgumentNotNull(environments, nameof(environments));
            Assert.ArgumentNotNull(parser, nameof(parser));
            Assert.ArgumentNotNull(output, nameof(output));
            Assert.ArgumentNotNull(eventCatalog, nameof(eventCatalog));

            Environments = environments;
            _handlers = handlers;
            _eventCatalog = eventCatalog;
            _settings = settings;
            _parser = parser;
            Output = output;
        }

        private void SetupState(bool headless)
        {
            if (_state != null)
                throw new ExecutionException("Cannot Run while the engine is already running.");
            _state = new EngineState(headless, _eventCatalog, _settings);
            _dispatcher = new CommandDispatcher(_parser, _handlers, Environments, _state, Output);
        }

        private void ClearState()
        {
            _state = null;
            _dispatcher = null;
        }

        public EngineState GetCurrentState() => _state ?? throw new InvalidOperationException("Cannot access State, one has not been created");

        public CommandDispatcher GetCurrentDispatcher() => _dispatcher ?? throw new InvalidOperationException("Cannot access Dispatcher, one has not been created");

        /// <summary>
        /// The set of configured environments
        /// </summary>
        public IEnvironmentCollection Environments { get; }

        /// <summary>
        /// The configured output channel
        /// </summary>
        public IOutput Output { get; }

        /// <summary>
        /// Selects the appropriate run mode and executes it based on the raw command line
        /// arguments passed to the application. If command line arguments are provided,
        /// they are executed in headless mode and the application will exit. If no
        /// arguments are provided the application will run in interactive mode.
        /// </summary>
        /// <returns></returns>
        public int RunWithCommandLineArguments()
        {
            var commandLine = GetRawCommandLineArguments();
            return Run(commandLine);
        }

        /// <summary>
        /// Selects the appropriate run mode and executes it. If a command is provided, the
        /// application is inferred to be in headless mode. If no arguments are provided
        /// the application is inferred to be in interactive mode.
        /// </summary>
        /// <param name="commandLine"></param>
        public int Run(string commandLine)
        {
            // If there are no arguments, enter interactive mode
            if (string.IsNullOrEmpty(commandLine))
                return RunInteractively();

            // if there is exactly one argument and it's the name of a valid environment,
            // start interactive mode setting that environment first.
            if (Environments.IsValid(commandLine))
                return RunInteractively(commandLine);

            // Otherwise run in headless mode and figure it out from there.
            return RunHeadless(commandLine);
        }

        /// <summary>
        /// Run the application in headless mode with the raw commandline arguments then
        /// exits the application
        /// </summary>
        /// <returns></returns>
        public int RunHeadlessWithCommandLineArgs()
        {
            var commandLine = GetRawCommandLineArguments();
            return RunHeadless(commandLine);
        }

        /// <summary>
        /// Run the application in headless mode with the provided commandline string.
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        public int RunHeadless(string commandLine)
        {
            SetupState(true);
            var sources = new CommandSourceCollection();

            // If we have a single argument "help", run the help script and exit. We don't
            // require a valid environment to run help
            if (commandLine == "help")
            {
                sources.AddToEnd(_state!.EventCatalog.HeadlessHelp, _parser,
                    ("exitcode", Constants.ExitCodeHeadlessHelp.ToString())
                );
                return RunLoop(sources);
            }

            // Now see if the first argument is the name of an environment. If so, switch
            // to that environment and continue
            var (startingEnvironment, newCl) = GetStartingEnvironment(commandLine);
            commandLine = newCl;

            // If there is no commandline left, run the HeadlessNoArgs script
            if (string.IsNullOrWhiteSpace(commandLine))
            {
                sources.AddToEnd(_state!.EventCatalog.HeadlessNoArgs, _parser,
                    ("exitcode", Constants.ExitCodeHeadlessNoVerb.ToString())
                );
                return RunLoop(sources);
            }

            // Setup the Headless start script, an environment change command if any, the
            // user command, and the headless stop script
            sources.AddToEnd(_state!.EventCatalog.EngineStartHeadless, _parser);
            if (!string.IsNullOrWhiteSpace(startingEnvironment))
                sources.AddToEnd($"{EnvironmentHandler.Name} '{startingEnvironment}'");
            sources.AddToEnd(commandLine);
            sources.AddToEnd(_state.EventCatalog.EngineStopHeadless, _parser);

            return RunLoop(sources);
        }

        /// <summary>
        /// Runs interactively, prompting the user for input and executing each command in
        /// turn. If an environment is not set, the user is prompted to select one before
        /// any commands are executed. Returns when the user has entered the 'exit' or
        /// 'quit' commands, or when some other verb has set the exit condition.
        /// </summary>
        public int RunInteractively() => RunInteractively(null);

        /// <summary>
        /// Runs interactively, setting the environment to the value given and then
        /// prompting the user for commands to execute. Returns when the user has entered
        /// the 'exit' or 'quit' commands, or when some other verb has set the exit
        /// condition.
        /// </summary>
        /// <param name="environment"></param>
        public int RunInteractively(string? environment)
        {
            SetupState(false);
            var source = new CommandSourceCollection();

            // Change the environment if necessary. Otherwise the EngineStartInteractive
            // script will probably prompt the user to do so.
            if (!string.IsNullOrEmpty(environment))
                source.AddToEnd($"{EnvironmentHandler.Name} '{environment}'");

            source.AddToEnd(_state!.EventCatalog.EngineStartInteractive, _parser);
            source.AddToEnd(new PromptCommandSource(Output, Environments, _state));

            return RunLoop(source);
        }

        // Attempt to get the raw commandline arguments as they were passed to the
        // application. Main(string[] args) is transformed by the shell with quotes
        // stripped. Environment.CommandLine is unmodified but we have to pull the exe name
        // off the front.
        private static string GetRawCommandLineArguments()
        {
            // Environment.CommandLine includes the name of the exe invoked, so strip that
            // off the front. Luckily it seems like quotes are stripped for us.
            var exeName = Environment.GetCommandLineArgs()[0];
            return Environment.CommandLine.Substring(exeName.Length).Trim();
        }

        // See if the given commandLine starts with a valid environment name. If so,
        // extract the environment name from the front and return the remainder of the
        // commandline.
        private (string? startingEnvironment, string commandLine) GetStartingEnvironment(string commandLine)
        {
            var validEnvironments = Environments.GetNames();
            if (validEnvironments.Count <= 1)
                return (null, commandLine);

            var parts = commandLine.Split(new[] { ' ' }, 2);
            var env = parts[0];
            return Environments.IsValid(env) ? (env, parts[1]) : (null, commandLine);
        }

        // Pulls commands from the command source until the source is empty or an exit
        // signal is received. Each command is added to the command queue and the queue
        // is drained.
        private int RunLoop(CommandSourceCollection sources)
        {
            try
            {
                return RunLoopInternal(sources);
            }
            finally
            {
                ClearState();
            }
        }

        private int RunLoopInternal(CommandSourceCollection sources)
        {
            while (true)
            {
                // Get a command. If we have one in the state use that. Otherwise try to
                // get one from the sources. If null, we're all done so exit
                var commandResult = _state!.Commands.GetNext();
                var command = commandResult.HasValue ? commandResult.Value : sources.GetNextCommand();
                if (command?.IsValid != true)
                    return Constants.ExitCodeOk;

                // Check the counter to make sure that we are not in a runaway loop
                // If we are in a loop, the counter will setup the command queue to handle
                // it
                var canExecute = _state.CommandCounter.VerifyCanExecuteNextCommand(_parser, Output);
                if (!canExecute)
                    continue;

                try
                {
                    // Get a cancellation token source, configured according to state Command
                    // settings, and use that to dispatch the command
                    using var tokenSource = _state.GetConfiguredCancellationSource();
                    _dispatcher!.Execute(command, tokenSource.Token);
                }
                catch (VerbNotFoundException vnf)
                {
                    // The verb was not found. Execute the VerbNotFound script
                    var args = SyntheticArguments.From(("verb", vnf.Verb));
                    HandleError(vnf, _state.EventCatalog.VerbNotFound, args);
                }
                catch (Exception e)
                {
                    // We've received some other error. Execute the EngineError script
                    // and hope for the best
                    var args = SyntheticArguments.From(
                        ("message", e.Message),
                        ("stacktrace", e.StackTrace)
                    );
                    HandleError(e, _state.EventCatalog.EngineError, args);
                }

                // If exit is signaled, return.
                if (_state.ShouldExit)
                    return _state.ExitCode;
            }
        }

        // Handle an error from the dispatcher.
        private void HandleError(Exception e, EventScript script, IArguments args)
        {
            // If an exception is thrown while handling a previous exception, show an
            // angry error message and exit immediately
            var currentExceptionResult = _state!.Metadata.Get(Constants.MetadataError);

            if (currentExceptionResult.HasValue && currentExceptionResult.Value is Exception currentException)
            {
                // This isn't scripted because it's critical error-handling code and we
                // don't want the user to clear/override it
                Output
                    .Color(ConsoleColor.Red)
                    .WriteLine("Received an exception while attempting to handle a previous exception")
                    .WriteLine("This is a fatal condition and the engine will exit")
                    .WriteLine("Make sure you clear the current exception when you are done handling it to avoid these situations")
                    .WriteLine("Current Exception:")
                    .WriteLine(e.Message)
                    .WriteLine(e.StackTrace)
                    .WriteLine("Previous Exception:")
                    .WriteLine(currentException.Message)
                    .WriteLine(currentException.StackTrace)
                    ;
                _state.Exit(Constants.ExitCodeCascadeError);
                return;
            }

            // Add the current exception to state metadata so we can keep track of loops,
            // then prepend the error-handling script and a command to remove the exception
            // from metadata (prepends happen in reverse order from how they're executed)
            // We can't remove metadata in the script, because users might change the script
            // and inadvertantly break loop detection.
            _state.Metadata.Add(Constants.MetadataError, e, false);
            _state.Commands.Prepend($"{MetadataHandler.Name} remove {Constants.MetadataError}");
            _state.Commands.Prepend(script.GetCommands(_parser, args));
            // Current command queue:
            // 1. Error-handling script
            // 2. "metadata remove __CURRENT_EXCEPTION"
            // 3. previous contents of command queue, if any
        }
    }
}
