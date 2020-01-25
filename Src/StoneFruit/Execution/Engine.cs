using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using StoneFruit.BuiltInVerbs;
using StoneFruit.Execution.Arguments;
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
             * 3. If we have switchtable environments AND we have at least 1 arg
             *    AND that arg is a valid environment, switch to that environment and continue
             * 4. Pass all args as headless mode then, if we don't exit, interactive mode
             */
            if (string.IsNullOrEmpty(commandLine))
                return RunInteractively();

            if (commandLine == "help")
            {
                // TODO: A HeadlessHelpCommand that includes more info about headless usage?
                new HelpVerb(_output, _commandSource, new CommandArguments()).Execute();
                return ExitCodeOk;
            }

            if (_environments.IsValid(commandLine))
                return RunInteractively(commandLine);

            return RunHeadlessWithCommandLineArgs();
        }

        // <summary>
        // Runs headless without an interactive prompt. All command information comes from the provided
        // arguments array. Notice that the args array from your Main() function probably don't work here
        // because the shell will automatically strip quotes and other punctuation. If you want to use your
        // command line arguments, use RunHeadlessWithCommandLineArgs() instead
        // </summary>
        // <param name="arg"></param>
        //public int RunHeadless(string[] arg)
        //{
        //    var state = new EngineState(true, _eventCatalog);
        //    var dispatcher = new CommandDispatcher(_parser, _commandSource, _environments, state, _output);
        //    if (arg == null || arg.Length == 0)
        //    {
        //        state.HeadlessNoArgs();
        //        ExecuteCommandQueue(state, dispatcher);
        //        return ExitCodeHeadlessNoArgs;
        //    }

        //    var env = arg[0];
        //    var realArgs = arg;
        //    if (_environments.IsValid(env))
        //    {
        //        state.AddCommand($"{EnvironmentChangeVerb.Name} '{env}'");
        //        realArgs = arg.Skip(1).ToArray();
        //    }

        //    var firstCommand = FixIncomingArguments(realArgs);

        //    state.StartHeadless();
        //    state.AddCommand(firstCommand);
        //    ExecuteCommandQueue(state, dispatcher);
        //    state.StopHeadless();
        //    ExecuteCommandQueue(state, dispatcher);
        //    return state.ExitCode;
        //}

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
            state.StartHeadless();
            state.AddCommand(commandLine);
            ExecuteCommandQueue(state, dispatcher);

            // The default StopHeadless script exits, but if we've removed that we can go into an interactive 
            // mode
            state.StopHeadless();
            ExecuteCommandQueue(state, dispatcher);
            if (state.ShouldExit)
                return state.ExitCode;
            return RunInteractively();
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
