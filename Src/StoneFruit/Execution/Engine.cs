using System;
using System.Linq;
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
            /*
             * 1. If we have no args, interactive mode
             * 2. If we have exactly one arg "help" show help and exit
             * 3. If we have multiple environments AND we have exactly 1 arg
             *    AND that arg is a valid environment, switch to that environment and continue interactive
             * 4. Pass all args as headless mode then, if we don't exit, interactive mode
             */
            if (string.IsNullOrEmpty(commandLine))
                return RunInteractively();

            if (_environments.IsValid(commandLine))
                return RunInteractively(commandLine);

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

            // If we have a single argument "help", run the help script and exit
            if (commandLine == "help")
            {
                var source = new ScriptCommandSource(state.EventCatalog.HeadlessHelp);
                return RunLoop(state, dispatcher, source);
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
                    state.AddCommand($"{EnvironmentChangeHandler.Name} '{env}'");
                    commandLine = parts[1];
                }
            }

            return RunHeadlessInternal(commandLine, state, dispatcher);
        }

        // Sets up the command source for headless mode and passes it to the RunLoop
        private int RunHeadlessInternal(string commandLine, EngineState state, CommandDispatcher dispatcher)
        {
            var source = new CombinedCommandSource();
            source.AddSource(new ScriptCommandSource(state.EventCatalog.EngineStartHeadless));
            source.AddSource(new SingleCommandSource(commandLine));
            source.AddSource(new ScriptCommandSource(state.EventCatalog.EngineStopHeadless));
            return RunLoop(state, dispatcher, source);
        }

        // Attempt to get the raw commandline arguments as they were passed to the application
        private static string GetRawCommandLineArguments()
        {
            var exeName = Environment.GetCommandLineArgs()[0];
            var firstCommand = Environment.CommandLine.Substring(exeName.Length).Trim();
            return firstCommand;
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
            return RunInteractiveInternal(state, dispatcher);
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
            state.AddCommand($"{EnvironmentChangeHandler.Name} {environment}");
            return RunInteractiveInternal(state, dispatcher);
        }

        // Sets up command sources for interactive mode and passes them to the RunLoop
        private int RunInteractiveInternal(EngineState state, CommandDispatcher dispatcher)
        {
            var source = new CombinedCommandSource();
            source.AddSource(new ScriptCommandSource(state.EventCatalog.EngineStartInteractive));
            source.AddSource(new PromptCommandSource(_output, _environments));
            source.AddSource(new ScriptCommandSource(state.EventCatalog.EngineStopInteractive));
            return RunLoop(state, dispatcher, source);
        }

        // Pulls commands from the command source until the source is empty or an exit signal is received
        // Each command is added to the command queue and the queue is drained
        private int RunLoop(EngineState state, CommandDispatcher dispatcher, ICommandSource source)
        {
            // Drain the state queue first, in case there's anything in there.
            ExecuteCommandQueue(state, dispatcher);
            if (state.ShouldExit)
                return state.ExitCode;

            // Now start draining the command source, executing each in turn.
            source.Start();
            while (true)
            {
                var command = source.GetNextCommand();
                if (string.IsNullOrEmpty(command))
                    return state.ExitCode;
                state.AddCommand(command);
                ExecuteCommandQueue(state, dispatcher);
                if (state.ShouldExit)
                    return state.ExitCode;
            }
        }

        // Drains the command queue. Executed commands may add more commands to the queue during execution
        // so we loop until the queue is empty or until an exit signal is received
        private void ExecuteCommandQueue(EngineState state, CommandDispatcher dispatcher)
        {
            while (true)
            {
                var commandString = state.GetNextCommand();
                if (string.IsNullOrEmpty(commandString))
                    return;
                try
                {
                    dispatcher.Execute(commandString);
                    if (state.ShouldExit)
                        return;
                }
                catch (Exception e)
                {
                    // TODO: Should make this behavior configurable
                    _output
                        .Color(ConsoleColor.Red)
                        .WriteLine(e.Message)
                        .WriteLine(e.StackTrace);
                }
            }
        }
    }
}
