using System;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.CommandSources;
using StoneFruit.Handlers;
using StoneFruit.Utility;

namespace StoneFruit
{
    /// <summary>
    /// The execution core. Provides a run loop to receive user commands and execute them
    /// </summary>
    public class Engine
    {
        private readonly ICommandParser _parser;

        public Engine(IHandlers handlers, IEnvironmentCollection environments, ICommandParser parser, IOutput output, EngineEventCatalog eventCatalog, EngineSettings settings)
        {
            Assert.ArgumentNotNull(environments, nameof(environments));
            Assert.ArgumentNotNull(parser, nameof(parser));
            Assert.ArgumentNotNull(output, nameof(output));

            Environments = environments;
            _parser = parser;
            Output = output;
            State = new EngineState(eventCatalog, settings);
            Dispatcher = new CommandDispatcher(_parser, handlers, Environments, State, Output);
        }

        public EngineState State { get; }

        /// <summary>
        /// The set of configured environments
        /// </summary>
        public IEnvironmentCollection Environments { get; }

        /// <summary>
        /// The configured output channel
        /// </summary>
        public IOutput Output { get; }

        public CommandDispatcher Dispatcher { get; }

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
            State.SetRunMode(EngineRunMode.Headless);
            var sources = new CommandSourceCollection();

            // If we have a single argument "help", run the help script and exit. We don't
            // require a valid environment to run help
            if (commandLine == "help")
            {
                sources.AddToEnd(State.EventCatalog.HeadlessHelp, _parser,
                    ("exitcode", Constants.ExitCode.HeadlessHelp.ToString())
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
                sources.AddToEnd(State.EventCatalog.HeadlessNoArgs, _parser,
                    ("exitcode", Constants.ExitCode.HeadlessNoVerb.ToString())
                );
                return RunLoop(sources);
            }

            // Setup the Headless start script, an environment change command if any, the
            // user command, and the headless stop script
            sources.AddToEnd(State.EventCatalog.EngineStartHeadless, _parser);
            if (!string.IsNullOrWhiteSpace(startingEnvironment))
                sources.AddToEnd($"{EnvironmentHandler.Name} '{startingEnvironment}'");
            sources.AddToEnd(commandLine);
            sources.AddToEnd(State.EventCatalog.EngineStopHeadless, _parser);

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
            State.SetRunMode(EngineRunMode.Interactive);
            var source = new CommandSourceCollection();

            // Change the environment if necessary. Otherwise the EngineStartInteractive
            // script will probably prompt the user to do so.
            if (!string.IsNullOrEmpty(environment))
                source.AddToEnd($"{EnvironmentHandler.Name} '{environment}'");

            source.AddToEnd(State.EventCatalog.EngineStartInteractive, _parser);
            source.AddToEnd(new PromptCommandSource(Output, Environments, State));

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
            if (State.RunMode == EngineRunMode.Idle)
                throw new ExecutionException("Cannot run the engine in Idle mode. Must enter Headless or Interactive mode first.");
            try
            {
                return RunLoopInternal(sources);
            }
            finally
            {
                State.SetRunMode(EngineRunMode.Idle);
            }
        }

        private int RunLoopInternal(CommandSourceCollection sources)
        {
            while (true)
            {
                // Get a command. If we have one in the state use that. Otherwise try to
                // get one from the sources. If null, we're all done so exit
                IResult<ArgumentsOrString> commandResult = State.Commands.GetNext();
                if (!commandResult.HasValue)
                    commandResult = sources.GetNextCommand();
                if (!commandResult.HasValue)
                    return Constants.ExitCode.Ok;

                var command = commandResult.Value;
                if (command?.IsValid != true)
                    return Constants.ExitCode.Ok;

                // Check the counter to make sure that we are not in a runaway loop
                // If we are in a loop, the counter will setup the command queue to handle
                // it
                var canExecute = State.CommandCounter.VerifyCanExecuteNextCommand(_parser, Output);
                if (!canExecute)
                    continue;

                RunOneCommand(command);

                // If exit is signaled, return.
                if (State.ShouldExit)
                    return State.ExitCode;
            }
        }

        private void RunOneCommand(ArgumentsOrString command)
        {
            try
            {
                // Get a cancellation token source, configured according to state Command
                // settings, and use that to dispatch the command
                using var tokenSource = State.GetConfiguredCancellationSource();
                Dispatcher.Execute(command, tokenSource.Token);
            }
            catch (VerbNotFoundException vnf)
            {
                // The verb was not found. Execute the VerbNotFound script
                var args = SyntheticArguments.From(("verb", vnf.Verb));
                HandleError(vnf, State.EventCatalog.VerbNotFound, args);
            }
            catch (Exception e)
            {
                // We've received some other error. Execute the EngineError script
                // and hope for the best
                var args = SyntheticArguments.From(
                    ("message", e.Message),
                    ("stacktrace", e.StackTrace ?? "")
                );
                HandleError(e, State.EventCatalog.EngineError, args);
            }
        }

        // Handle an error from the dispatcher.
        private void HandleError(Exception currentException, EventScript script, IArguments args)
        {
            // If an exception is thrown while handling a previous exception, show an
            // angry error message and exit immediately
            var currentExceptionResult = State.Metadata.Get(Constants.Metadata.Error);

            if (currentExceptionResult.HasValue && currentExceptionResult.Value is Exception previousException)
            {
                // This isn't scripted because it's critical error-handling code and we
                // don't want the user to clear/override it
                Output
                    .Color(ConsoleColor.Red)
                    .WriteLine("Received an exception while attempting to handle a previous exception")
                    .WriteLine("This is a fatal condition and the engine will exit")
                    .WriteLine("Make sure you clear the current exception when you are done handling it to avoid these situations")
                    .WriteLine("Current Exception:")
                    .WriteLine(currentException.Message)
                    .WriteLine(currentException.StackTrace ?? "")
                    .WriteLine("Previous Exception:")
                    .WriteLine(previousException.Message)
                    .WriteLine(previousException.StackTrace ?? "")
                    ;
                State.SignalExit(Constants.ExitCode.CascadeError);
                return;
            }

            // Add the current exception to state metadata so we can keep track of loops,
            // then prepend the error-handling script and a command to remove the exception
            // from metadata (prepends happen in reverse order from how they're executed)
            // We can't remove metadata in the script, because users might change the script
            // and inadvertantly break loop detection.
            State.Metadata.Add(Constants.Metadata.Error, currentException, false);
            State.Commands.Prepend($"{MetadataHandler.Name} remove {Constants.Metadata.Error}");
            State.Commands.Prepend(script.GetCommands(_parser, args));
            // Current command queue:
            // 1. Error-handling script
            // 2. "metadata remove __CURRENT_EXCEPTION"
            // 3. previous contents of command queue, if any
        }
    }
}
