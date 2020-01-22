using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.BuiltInVerbs;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Environments;

namespace StoneFruit.Execution
{
    /// <summary>
    /// The execution core. Provides a run loop to receive user commands and execute them
    /// </summary>
    public class Engine
    {
        public const int ExitCodeOk = 0;
        public const int ExitCodeHeadlessNoArgs = 1;

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

        /// <summary>
        /// Selects the appropriate run mode and executes it. If a command is provided, the application
        /// is inferred to be in headless mode. If no arguments are provided the application is inferred to
        /// be in interactive mode.
        /// </summary>
        /// <param name="args"></param>
        public int Run(string[] args = null)
        {
            if (args == null || args.Length == 0)
                return RunInteractively();

            if (args.Length == 1 && _environments.IsValid(args[0]))
                return RunInteractively();

            if (args.Length == 1 && args[0] == "help")
            {
                // TODO: A HeadlessHelpCommand that includes more info about headless usage?
                new HelpVerb(_output, _commandSource, new CommandArguments()).Execute();
                return ExitCodeOk;
            }

            return RunHeadless(args);
        }

        /// <summary>
        /// Runs headless without an interactive prompt. All command information comes from the provided
        /// arguments (which typically come from command line arguments)
        /// </summary>
        /// <param name="arg"></param>
        public int RunHeadless(string[] arg)
        {
            var state = new EngineState(true, _eventCatalog);
            var dispatcher = new CommandDispatcher(_parser, _commandSource, _environments, state, _output);
            if (arg == null || arg.Length == 0)
            {
                state.HeadlessNoArgs();
                ExecuteCommandQueue(state, dispatcher);
                return ExitCodeHeadlessNoArgs;
            }

            var env = arg[0];
            IEnumerable<string> realArgs = arg;
            if (_environments.IsValid(env))
            {
                state.AddCommand($"{EnvironmentChangeVerb.Name} '{env}'");
                realArgs = arg.Skip(1);
            }

            state.StartHeadless();
            state.AddCommand(string.Join(" ", realArgs));
            ExecuteCommandQueue(state, dispatcher);
            state.StopHeadless();
            ExecuteCommandQueue(state, dispatcher);
            return state.ExitCode;
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
            return RunInteractivelyWithEnvironment(state, dispatcher);
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
            return RunInteractivelyWithEnvironment(state, dispatcher);
        }

        private int RunInteractivelyWithEnvironment(EngineState state, CommandDispatcher dispatcher)
        {
            state.StartInteractive();
            ExecuteCommandQueue(state, dispatcher);

            _output
                .Write("Enter command ")
                .Color(ConsoleColor.DarkGray).Write("('help' for help, 'exit' to quit)")
                .Color(ConsoleColor.Gray).WriteLine(":");

            while (true)
            {
                var commandString = _output.Prompt($"{_environments.CurrentName}");
                state.AddCommand(commandString);

                ExecuteCommandQueue(state, dispatcher);
                if (state.ShouldExit)
                    break;
            }

            state.StopInteractive();
            ExecuteCommandQueue(state, dispatcher);
            return state.ExitCode;
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
