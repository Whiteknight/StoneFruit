using System;
using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Sequences;
using StoneFruit.BuiltInVerbs;
using StoneFruit.BuiltInVerbs.Hidden;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Environments;

namespace StoneFruit.Execution
{
    public class Engine
    {
        private readonly IEnvironmentCollection _environments;
        private readonly ICommandSource _commandSource;
        private readonly ITerminalOutput _output;
        private readonly IParser<char, CompleteCommand> _parser;

        public Engine(ICommandSource commands, IEnvironmentCollection environments, IParser<char, IEnumerable<IArgument>> argParser)
        {
            // TODO: If we have 0 commands, we might want to just abort?
            _environments = environments ?? new InstanceEnvironmentCollection(null);
            _commandSource = commands;
            _parser = CompleteCommandGrammar.GetParser(argParser);
            _output = new ConsoleTerminalOutput();
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

            var env = arg[0];
            new ChangeEnvironmentCommand(_output, CommandArguments.Single(env), state, _environments).Execute();

            state.AddCommand(string.Join(" ", arg.Skip(1)));
            ExecuteCommandQueue(state);
        }

        public void RunInteractively()
        {
            var state = new EngineState(false);

            if (_environments.Current == null)
                new ChangeEnvironmentCommand(_output, new CommandArguments(), state, _environments).Execute();

            RunInteractivelyWithEnvironment(state);
        }

        public void RunInteractively(string environment)
        {
            var state = new EngineState(false);
            new ChangeEnvironmentCommand(_output, CommandArguments.Single(environment), state, _environments).Execute();
            RunInteractivelyWithEnvironment(state);
        }

        private void RunInteractivelyWithEnvironment(EngineState state)
        {
            _output.Write("Enter command ");
            _output.Write(ConsoleColor.DarkGray, "('help' for help, 'exit' to quit)");
            _output.WriteLine(":");

            while (true)
            {
                var commandString = ReadLine.Read($"{_environments.CurrentName}> ");
                if (string.IsNullOrWhiteSpace(commandString))
                    continue;
                state.AddCommand(commandString);

                ExecuteCommandQueue(state);
                if (state.ShouldExit)
                    return;
            }
        }

        private void ExecuteCommandQueue(EngineState state)
        {
            while (true)
            {
                var commandString = state.GetNextCommand();
                if (string.IsNullOrEmpty(commandString))
                    return;
                try
                {
                    var sequence = new StringCharacterSequence(commandString);
                    var command = _parser.Parse(sequence).Value;
                    GetCommand(command, state).Execute();
                    if (state.ShouldExit)
                        return;
                    ReadLine.AddHistory(commandString);
                }
                catch (Exception e)
                {
                    _output.RedLine(e.Message);
                    _output.RedLine(e.StackTrace);
                }
            }
        }

        private ICommandVerb GetCommand(CompleteCommand commandRequest, EngineState state) 
            => _commandSource.GetCommandInstance(commandRequest, _environments, state, _output) ?? new NotFoundCommandVerb(commandRequest.Verb, _output);
    }
}
