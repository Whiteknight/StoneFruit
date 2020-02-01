using System;
using System.Linq;
using System.Threading;
using StoneFruit.BuiltInVerbs;
using StoneFruit.Execution.CommandSources;
using StoneFruit.Execution.Environments;
using StoneFruit.Execution.Output;

namespace StoneFruit.Execution
{
    /// <summary>
    /// The execution core. Provides a run loop to receive user commands and execute them
    /// </summary>
    public class Engine
    {
        // TODO: Move these to a better place
        public const int ExitCodeOk = 0;
        public const int ExitCodeHeadlessHelp = 0;
        public const int ExitCodeHeadlessNoVerb = 1;
        public const int ExitCodeCascadeError = 2;

        public const string MetadataError = "__CURRENT_EXCEPTION";

        private readonly IEnvironmentCollection _environments;
        private readonly ICommandHandlerSource _commandSource;
        private readonly EngineEventCatalog _eventCatalog;
        private readonly ITerminalOutput _output;
        private readonly CommandParser _parser;

        public Engine(ICommandHandlerSource commands, IEnvironmentCollection environments, CommandParser parser, ITerminalOutput output, EngineEventCatalog eventCatalog)
        {
            _environments = environments ?? new InstanceEnvironmentCollection(null);
            // TODO: If we have 0 commands, we might want to just abort?
            // Otherwise, how do we enforce that we have something here?
            _commandSource = commands;
            _eventCatalog = eventCatalog;
            _parser = parser ?? CommandParser.GetDefault();
            _output = output ?? new ConsoleTerminalOutput();
        }

        /// <summary>
        /// Selects the appropriate run mode and executes it based on the raw command line arguments passed
        /// to the application. If command line arguments are provided, they are executed in headless mode
        /// and the application will exit. If no arguments are provided the application will run in
        /// interactive mode.
        /// </summary>
        /// <returns></returns>
        public int RunWithCommandLineArguments()
        {
            var commandLine = GetRawCommandLineArguments();
            return Run(commandLine);
        }

        /// <summary>
        /// Selects the appropriate run mode and executes it. If a command is provided, the application
        /// is inferred to be in headless mode. If no arguments are provided the application is inferred to
        /// be in interactive mode.
        /// </summary>
        /// <param name="commandLine"></param>
        public int Run(string commandLine)
        {
            // If there are no arguments, enter interactive mode
            if (string.IsNullOrEmpty(commandLine))
                return RunInteractively();

            // if there is exactly one argument and it's the name of a valid environment, start interactive
            // mode setting that environment first.
            if (_environments.IsValid(commandLine))
                return RunInteractively(commandLine);

            // Otherwise run in headless mode and figure it out from there.
            return RunHeadless(commandLine);
        }

        /// <summary>
        /// Run the application in headless mode with the raw commandline arguments then exits the
        /// application
        /// </summary>
        /// <returns></returns>
        public int RunHeadlessWithCommandLineArgs()
        {
            // Environment.CommandLine includes the name of the exe invoked, so strip that off the front
            var commandLine = GetRawCommandLineArguments();
            return RunHeadless(commandLine);
        }

        /// <summary>
        /// Run the application in headless mode with the provide commandline string.
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        public int RunHeadless(string commandLine)
        {
            var state = new EngineState(true, _eventCatalog);
            var dispatcher = new CommandDispatcher(_parser, _commandSource, _environments, state, _output);
            var sources = new CommandSourceCollection();

            // If we have a single argument "help", run the help script and exit. We don't require a valid
            // environment to run help
            if (commandLine == "help")
            {
                sources.AddToEnd(new ScriptCommandSource(state.EventCatalog.HeadlessHelp));
                return RunLoop(state, dispatcher, sources);
            }

            // Now see if the first argument is the name of an environment. If so, switch to that environment
            // and continue
            var validEnvironments = _environments.GetNames().Values.ToList();
            if (validEnvironments.Count > 1)
            {
                var parts = commandLine.Split(new[] { ' ' }, 2);
                var env = parts[0];
                if (_environments.IsValid(env))
                {
                    sources.AddToEnd(new SingleCommandSource($"{EnvironmentChangeHandler.Name} '{env}'"));
                    commandLine = parts[1];
                }
            }

            // Setup the Headless start script, the user command, and the headless stop script before
            // running the RunLoop
            sources.AddToEnd(new ScriptCommandSource(state.EventCatalog.EngineStartHeadless));
            sources.AddToEnd(new SingleCommandSource(commandLine));
            sources.AddToEnd(new ScriptCommandSource(state.EventCatalog.EngineStopHeadless));
            return RunLoop(state, dispatcher, sources);
        }

        /// <summary>
        /// Runs interactively, prompting the user for input and executing each command in turn. If an
        /// environment is not set, the user is prompted to select one before any commands are executed.
        /// Returns when the user has entered the 'exit' or 'quit' commands, or when some other verb has
        /// set the exit condition.
        /// </summary>
        public int RunInteractively()
        {
            var state = new EngineState(false, _eventCatalog);
            var dispatcher = new CommandDispatcher(_parser, _commandSource, _environments, state, _output);
            var source = new CommandSourceCollection();
            source.AddToEnd(new ScriptCommandSource(state.EventCatalog.EngineStartInteractive));
            source.AddToEnd(new PromptCommandSource(_output, _environments));
            source.AddToEnd(new ScriptCommandSource(state.EventCatalog.EngineStopInteractive));
            return RunLoop(state, dispatcher, source);
        }

        /// <summary>
        /// Runs interactively, setting the environment to the value given and then prompting the user for
        /// commands to execute. Returns when the user has entered the 'exit' or 'quit' commands, or when
        /// some other verb has set the exit condition.
        /// </summary>
        /// <param name="environment"></param>
        public int RunInteractively(string environment)
        {
            var state = new EngineState(false, _eventCatalog);
            var dispatcher = new CommandDispatcher(_parser, _commandSource, _environments, state, _output);
            var source = new CommandSourceCollection();
            source.AddToEnd(new SingleCommandSource($"{EnvironmentChangeHandler.Name} {environment}"));
            source.AddToEnd(new ScriptCommandSource(state.EventCatalog.EngineStartInteractive));
            source.AddToEnd(new PromptCommandSource(_output, _environments));
            source.AddToEnd(new ScriptCommandSource(state.EventCatalog.EngineStopInteractive));
            return RunLoop(state, dispatcher, source);
        }

        // Attempt to get the raw commandline arguments as they were passed to the application. We have to
        // do it this way because (string[] args) will be parsed by the shell and quotes may be stripped
        // from arguments. Environment.CommandLine is unmodified but we have to pull the exe name off the
        // front.
        private static string GetRawCommandLineArguments()
        {
            var exeName = Environment.GetCommandLineArgs()[0];
            var firstCommand = Environment.CommandLine.Substring(exeName.Length).Trim();
            return firstCommand;
        }

        // Pulls commands from the command source until the source is empty or an exit signal is received
        // Each command is added to the command queue and the queue is drained. 
        private int RunLoop(EngineState state, CommandDispatcher dispatcher, CommandSourceCollection sources)
        {
            while (true)
            {
                // Get a command. If we have one in the state use that. Otherwise try to get one from the
                // sources.
                var command = state.GetNextCommand();
                if (string.IsNullOrEmpty(command))
                    command = sources.GetNextCommand();
                if (string.IsNullOrEmpty(command))
                    return ExitCodeOk;

                // Dispatch the command to the handler, dealing with any errors that may arise
                try
                {
                    // TODO: Figure out how to set this? I assume ctrl+c would break it, but we would need
                    // to setup handlers for that throughout
                    // TODO: Should we maybe set some kind of timeout?
                    var tokenSource = new CancellationTokenSource();
                    dispatcher.Execute(command, tokenSource);
                }
                catch (Exception e)
                {
                    HandleError(state, e);
                }

                // If exit is signaled, return. 
                if (state.ShouldExit)
                    return state.ExitCode;
            }
        }

        // Handle an error from the dispatcher.
        private void HandleError(EngineState state, Exception e)
        {
            // If we're in an error loop (throw an exception while handling a previous exception) show an
            // angry error message and signal for exit.
            var currentException = state.GetMetadata(MetadataError);
            if (currentException != null)
            {
                _output
                    .Color(ConsoleColor.Red)
                    .WriteLine("Received an exception while attempting to handle a previous exception")
                    .WriteLine("This is a fatal condition and the engine will exit")
                    .WriteLine("Make sure you clear the current exception when you are done handling it to avoid these situations")
                    .WriteLine(e.Message)
                    .WriteLine(e.StackTrace);
                state.Exit(ExitCodeCascadeError);
            }

            // Otherwise add the error-handling script to the command queue so the queue loop can handle it.
            state.AddMetadata(MetadataError, e, false);
            state.PrependCommand($"{MetadataRemoveHandler.Name} {MetadataError}");
            state.PrependCommands(state.EventCatalog.EngineError.GetCommands());
        }
    }
}
