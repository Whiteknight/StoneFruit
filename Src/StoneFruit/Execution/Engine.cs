using System;
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
        private readonly IEnvironmentCollection _environments;
        private readonly ICommandSource _commandSource;
        private readonly ITerminalOutput _output;
        private readonly CommandParser _parser;

        // TODO: Be able to specify startup commands that will execute as soon as the engine starts
        // like "env-change" does

        public Engine(ICommandSource commands, IEnvironmentCollection environments, CommandParser parser, ITerminalOutput output)
        {
            _environments = environments ?? new InstanceEnvironmentCollection(null);
            // TODO: If we have 0 commands, we might want to just abort?
            // Otherwise, how do we enforce that we have something here?
            _commandSource = commands;
            _parser = parser ?? CommandParser.GetDefault();
            _output = output ?? new ConsoleTerminalOutput();
        }

        /// <summary>
        /// Selects the appropriate run mode and executes it. If a command is provided, the application
        /// is inferred to be in headless mode. If no arguments are provided the application is inferred to
        /// be in interactive mode.
        /// </summary>
        /// <param name="args"></param>
        public void Run(string[] args = null)
        {
            if (args == null || args.Length == 0)
            {
                RunInteractively();
                return;
            }

            if (args.Length == 1 && _environments.IsValid(args[0]))
            {
                RunInteractively();
                return;
            }

            if (args.Length == 1 && args[0] == "help")
            {
                // TODO: A HeadlessHelpCommand that includes more info about headless usage?
                new HelpCommand(_output, _commandSource, new CommandArguments()).Execute();
                return;
            }

            RunHeadless(args);
        }

        /// <summary>
        /// Runs headless without an interactive prompt. All command information comes from the provided
        /// arguments (which typically come from command line arguments)
        /// </summary>
        /// <param name="arg"></param>
        public void RunHeadless(string[] arg)
        {
            // TODO: If we get here with no args, show help and exit
            var state = new EngineState(true);
            var dispatcher = new CommandDispatcher(_parser, _commandSource, _environments, state, _output);

            var env = arg[0];
            if (_environments.IsValid(env))
            {
                dispatcher.Execute(EnvironmentChangeCommand.Name + " " + env);
                state.AddCommand(string.Join(" ", arg.Skip(1)));
            }
            else
                state.AddCommand(string.Join(" ", arg));

            ExecuteCommandQueue(state, dispatcher);
        }

        /// <summary>
        /// Runs interactively, prompting the user for input and executing each command in turn. If an
        /// environment is not set, the user is prompted to select one before any commands are executed.
        /// Returns when the user has entered the 'exit' or 'quit' commands, or when some other verb has
        /// set the exit condition.
        /// </summary>
        public void RunInteractively()
        {
            var state = new EngineState(false);
            var dispatcher = new CommandDispatcher(_parser, _commandSource, _environments, state, _output);

            if (_environments.Current == null)
                dispatcher.Execute(EnvironmentChangeCommand.Name);

            RunInteractivelyWithEnvironment(state, dispatcher);
        }

        /// <summary>
        /// Runs interactively, setting the environment to the value given and then prompting the user for
        /// commands to execute. Returns when the user has entered the 'exit' or 'quit' commands, or when
        /// some other verb has set the exit condition.
        /// </summary>
        /// <param name="environment"></param>
        public void RunInteractively(string environment)
        {
            var state = new EngineState(false);
            var dispatcher = new CommandDispatcher(_parser, _commandSource, _environments, state, _output);
            dispatcher.Execute(EnvironmentChangeCommand.Name);
            RunInteractivelyWithEnvironment(state, dispatcher);
        }

        private void RunInteractivelyWithEnvironment(EngineState state, CommandDispatcher dispatcher)
        {
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
                    return;
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
