using System;
using System.Linq;
using System.Threading;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.CommandSources;
using StoneFruit.Handlers;
using StoneFruit.Utility;

namespace StoneFruit.Execution
{
    /// <summary>
    /// The execution core. Provides a run loop to receive user commands and execute them
    /// </summary>
    public class Engine
    {
        private readonly IEnvironmentCollection _environments;
        private readonly IHandlerSource _commandSource;
        private readonly EngineEventCatalog _eventCatalog;
        private readonly EngineSettings _settings;
        private readonly IOutput _output;
        private readonly ICommandParser _parser;

        public Engine(IHandlerSource commands, IEnvironmentCollection environments, ICommandParser parser, IOutput output, EngineEventCatalog eventCatalog, EngineSettings settings)
        {
            Assert.ArgumentNotNull(commands, nameof(commands));
            Assert.ArgumentNotNull(environments, nameof(environments));
            Assert.ArgumentNotNull(parser, nameof(parser));
            Assert.ArgumentNotNull(output, nameof(output));
            Assert.ArgumentNotNull(eventCatalog, nameof(eventCatalog));
            
            _environments = environments;
            _eventCatalog = eventCatalog;
            _settings = settings;
            _parser = parser;
            _output = output;
            _commandSource = commands;

            if (!_commandSource.GetAll().Any())
                throw EngineException.NoHandlers();
        }

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
            if (_environments.IsValid(commandLine))
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
        /// Run the application in headless mode with the provide commandline string.
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        public int RunHeadless(string commandLine)
        {
            var state = new EngineState(true, _eventCatalog, _settings);
            var dispatcher = new CommandDispatcher(_parser, _commandSource, _environments, state, _output);
            var sources = new CommandSourceCollection();

            // If we have a single argument "help", run the help script and exit. We don't
            // require a valid environment to run help
            if (commandLine == "help")
            {
                sources.AddToEnd(state.EventCatalog.HeadlessHelp, _parser,
                    ("exitcode", Constants.ExitCodeHeadlessHelp.ToString())
                );
                return RunLoop(state, dispatcher, sources);
            }

            // Now see if the first argument is the name of an environment. If so, switch
            // to that environment and continue
            var validEnvironments = _environments.GetNames();
            if (validEnvironments.Count > 1)
            {
                var parts = commandLine.Split(new[] { ' ' }, 2);
                var env = parts[0];
                if (_environments.IsValid(env))
                {
                    sources.AddToEnd($"{EnvironmentChangeHandler.Name} '{env}'");
                    commandLine = parts[1];
                }
            }

            if (string.IsNullOrWhiteSpace(commandLine))
            {
                sources.AddToEnd(state.EventCatalog.HeadlessNoArgs, _parser,
                    ("exitcode", Constants.ExitCodeHeadlessNoVerb.ToString())
                );
                return RunLoop(state, dispatcher, sources);
            }

            // Setup the Headless start script, the user command, and the headless stop
            // script before running the RunLoop
            sources.AddToEnd(state.EventCatalog.EngineStartHeadless, _parser);
            sources.AddToEnd(commandLine);
            sources.AddToEnd(state.EventCatalog.EngineStopHeadless, _parser);
            return RunLoop(state, dispatcher, sources);
        }

        /// <summary>
        /// Runs interactively, prompting the user for input and executing each command in
        /// turn. If an environment is not set, the user is prompted to select one before
        /// any commands are executed. Returns when the user has entered the 'exit' or
        /// 'quit' commands, or when some other verb has set the exit condition.
        /// </summary>
        public int RunInteractively()
        {
            var state = new EngineState(false, _eventCatalog, _settings);
            var dispatcher = new CommandDispatcher(_parser, _commandSource, _environments, state, _output);
            var source = new CommandSourceCollection();
            source.AddToEnd(state.EventCatalog.EngineStartInteractive, _parser);
            source.AddToEnd(new PromptCommandSource(_output, _environments, state));
            //source.AddToEnd(state.EventCatalog.EngineStopInteractive, _parser);
            return RunLoop(state, dispatcher, source);
        }

        /// <summary>
        /// Runs interactively, setting the environment to the value given and then
        /// prompting the user for commands to execute. Returns when the user has entered
        /// the 'exit' or 'quit' commands, or when some other verb has set the exit
        /// condition.
        /// </summary>
        /// <param name="environment"></param>
        public int RunInteractively(string environment)
        {
            var state = new EngineState(false, _eventCatalog, _settings);
            var dispatcher = new CommandDispatcher(_parser, _commandSource, _environments, state, _output);
            var source = new CommandSourceCollection();
            source.AddToEnd($"{EnvironmentChangeHandler.Name} {environment}");
            source.AddToEnd(state.EventCatalog.EngineStartInteractive, _parser);
            source.AddToEnd(new PromptCommandSource(_output, _environments, state));
            //source.AddToEnd(state.EventCatalog.EngineStopInteractive, _parser);
            return RunLoop(state, dispatcher, source);
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
            var firstCommand = Environment.CommandLine.Substring(exeName.Length).Trim();
            return firstCommand;
        }

        // Pulls commands from the command source until the source is empty or an exit
        // signal is received. Each command is added to the command queue and the queue
        // is drained. 
        private int RunLoop(EngineState state, CommandDispatcher dispatcher, CommandSourceCollection sources)
        {
            while (true)
            {
                // Get a command. If we have one in the state use that. Otherwise try to
                // get one from the sources.
                var command = state.Commands.GetNext() ?? sources.GetNextCommand();
                if (command == null || !command.IsValid)
                    return Constants.ExitCodeOk;

                var canExecute = state.CommandCounter.VerifyCanExecuteNextCommand(_parser, _output);
                if (!canExecute)
                    continue;
                
                // Dispatch the command to the handler, dealing with any errors
                try
                {
                    using var tokenSource = GetCancellation(state);
                    dispatcher.Execute(command, tokenSource.Token);
                }
                catch (VerbNotFoundException vnf)
                {
                    var args = SyntheticArguments.From(("verb", vnf.Verb));
                    HandleError(state, vnf, state.EventCatalog.VerbNotFound, args);
                }
                catch (Exception e)
                {
                    var args = SyntheticArguments.From(
                        ("message", e.Message),
                        ("stacktrace", e.StackTrace)
                    );
                    HandleError(state, e, state.EventCatalog.EngineError, args);
                }

                // If exit is signaled, return. 
                if (state.ShouldExit)
                    return state.ExitCode;
            }
        }

        private CancellationTokenSource GetCancellation(EngineState state)
        {
            var tokenSource = new CancellationTokenSource();
            var timeout = state.Settings.MaxExecuteTimeout;
            if (timeout < TimeSpan.MaxValue)
                tokenSource.CancelAfter(timeout);
            return tokenSource;
        }

        // Handle an error from the dispatcher.
        private void HandleError(EngineState state, Exception e, EventScript script, IArguments args)
        {
            // If we're in an error loop (throw an exception while handling a previous
            // exception) show an angry error message and signal for exit.
            var currentException = state.Metadata.Get(Constants.MetadataError);
            if (currentException != null)
            {
                // This isn't scripted because it's critical error-handling code and we
                // don't want the user to clear/override it
                _output
                    .Color(ConsoleColor.Red)
                    .WriteLine("Received an exception while attempting to handle a previous exception")
                    .WriteLine("This is a fatal condition and the engine will exit")
                    .WriteLine("Make sure you clear the current exception when you are done handling it to avoid these situations")
                    .WriteLine(e.Message)
                    .WriteLine(e.StackTrace);
                state.Exit(Constants.ExitCodeCascadeError);
            }

            // Otherwise add the error-handling script to the command queue
            state.Metadata.Add(Constants.MetadataError, e, false);
            state.Commands.Prepend($"{MetadataRemoveHandler.Name} {Constants.MetadataError}");
            state.Commands.Prepend(script.GetCommands(_parser, args));
        }
    }
}
