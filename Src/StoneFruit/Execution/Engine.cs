using System;
using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using StoneFruit.BuiltInVerbs;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Environments;

namespace StoneFruit.Execution
{
    /// <summary>
    /// The execution core
    /// </summary>
    public class Engine
    {
        private readonly IEnvironmentCollection _environments;
        private readonly ICommandSource _commandSource;
        private readonly ITerminalOutput _output;
        private readonly IParser<char, CompleteCommand> _parser;

        // TODO: Be able to specify startup commands that will execute as soon as the engine starts
        // like "change-env" does

        public Engine(ICommandSource commands, IEnvironmentCollection environments, IParser<char, IEnumerable<IArgument>> argParser, ITerminalOutput output)
        {
            _environments = environments ?? new InstanceEnvironmentCollection(null);
            // TODO: If we have 0 commands, we might want to just abort?
            // Otherwise, how do we enforce that we have something here?
            _commandSource = commands;
            _parser = CompleteCommandGrammar.GetParser(argParser);
            _output = output ?? new ConsoleTerminalOutput();
        }

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

        public void RunHeadless(string[] arg)
        {
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

        public void RunInteractively()
        {
            var state = new EngineState(false);
            var dispatcher = new CommandDispatcher(_parser, _commandSource, _environments, state, _output);

            if (_environments.Current == null)
                dispatcher.Execute(EnvironmentChangeCommand.Name);

            RunInteractivelyWithEnvironment(state, dispatcher);
        }

        public void RunInteractively(string environment)
        {
            var state = new EngineState(false);
            var dispatcher = new CommandDispatcher(_parser, _commandSource, _environments, state, _output);
            dispatcher.Execute(EnvironmentChangeCommand.Name);
            RunInteractivelyWithEnvironment(state, dispatcher);
        }

        private void RunInteractivelyWithEnvironment(EngineState state, CommandDispatcher dispatcher)
        {
            _output.Write("Enter command ");
            _output.Write(ConsoleColor.DarkGray, "('help' for help, 'exit' to quit)");
            _output.WriteLine(":");

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
                    _output.RedLine(e.Message);
                    _output.RedLine(e.StackTrace);
                }
            }
        }
    }
}
