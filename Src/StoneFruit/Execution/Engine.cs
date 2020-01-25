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
        public const int ExitCodeOk = 0;
        public const int ExitCodeHeadlessHelp = 0;
        public const int ExitCodeHeadlessNoVerb = 1;

        private readonly IEnvironmentCollection _environments;
        private readonly ICommandVerbSource _commandSource;
        private readonly EngineEventCatalog _eventCatalog;
        private readonly ITerminalOutput _output;
        private readonly CommandParser _parser;

        public Engine(ICommandVerbSource commands, IEnvironmentCollection environments, CommandParser parser, ITerminalOutput output, EngineEventCatalog eventCatalog)
        {
            _environments = environments ?? new InstanceEnvironmentCollection(null);
            // TODO: If we have 0 commands, we might want to just abort?
            // Otherwise, how do we enforce that we have something here?
            _commandSource = commands;
            _eventCatalog = eventCatalog;
            _parser = parser ?? CommandParser.GetDefault();
            _output = output ?? new ConsoleTerminalOutput();
        }

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

        public int RunHeadlessWithCommandLineArgs()
        {
            // Environment.CommandLine includes the name of the exe invoked, so strip that off the front
            var commandLine = GetRawCommandLineArguments();
            return RunHeadless(commandLine);
        }

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
                    state.AddCommand($"{EnvironmentChangeVerb.Name} '{env}'");
                    commandLine = parts[1];
                }
            }

            return RunHeadlessInternal(commandLine, state, dispatcher);
        }

        private int RunHeadlessInternal(string commandLine, EngineState state, CommandDispatcher dispatcher)
        {
            var source = new CombinedCommandSource();
            source.AddSource(new ScriptCommandSource(state.EventCatalog.EngineStartHeadless));
            source.AddSource(new SingleCommandSource(commandLine));
            source.AddSource(new ScriptCommandSource(state.EventCatalog.EngineStopHeadless));
            return RunLoop(state, dispatcher, source);
        }

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
            state.AddCommand($"{EnvironmentChangeVerb.Name} {environment}");
            return RunInteractiveInternal(state, dispatcher);
        }

        private int RunInteractiveInternal(EngineState state, CommandDispatcher dispatcher)
        {
            var source = new CombinedCommandSource();
            source.AddSource(new ScriptCommandSource(state.EventCatalog.EngineStartInteractive));
            source.AddSource(new PromptCommandSource(_output, _environments));
            source.AddSource(new ScriptCommandSource(state.EventCatalog.EngineStopInteractive));
            return RunLoop(state, dispatcher, source);
        }

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
                    _output
                        .Color(ConsoleColor.Red)
                        .WriteLine(e.Message)
                        .WriteLine(e.StackTrace);
                }
            }
        }
    }
}
