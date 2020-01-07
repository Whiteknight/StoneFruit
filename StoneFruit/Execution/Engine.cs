using System;
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

        public Engine(ICommandSource commands, IEnvironmentCollection environments, IParser<char, IArgument> argParser)
        {
            // TODO: If we have 0 commands, we might want to just abort?
            _environments = environments ?? new InstanceEnvironmentCollection(null);
            _commandSource = commands;
            _parser = CompleteCommandGrammar.GetParser(argParser);
            _output = new ConsoleTerminalOutput();
        }

        public void Start(string[] args = null)
        {
            //if (args == null || args.Length == 0)
            //{
                //ExecuteCommand(ChangeEnvironmentCommand.Name, CommandArguments.Empty);
                RunInteractively();
            //}
            //else if (args.Length == 1 && _environments.IsValid(args[0]))
            //{
            //    ExecuteCommand(ChangeEnvironmentCommand.Name, CommandArguments.Parse(args[0]));
            //    RunInteractively();
            //}
            //else
            //{
            //    RunHeadless(args);
            //}
        }

        //private void RunHeadless(string[] arg)
        //{
        //    _state.Headless = true;

        //    var sequence = new StringArraySequence(arg);

        //    var env = sequence.GetNext();
        //    if (env == "help")
        //    {
        //        env = _environments.GetName(0);
        //        new HelpCommand().Execute(CreateContext(env));
        //        System.Environment.ExitCode = 0;
        //        return;
        //    }

        //    var context = CreateContext(env);
        //    new ChangeEnvironmentCommand().Execute(context);

        //    var args = CommandArguments.Parse(string.Join(" ", batch));
        //    try
        //    {
        //        var command = args.Shift();
        //        ReadLine.AddHistory(command);
        //        ExecuteCommand(command, args, false);
        //    }
        //    catch (Exception)
        //    {
        //        System.Environment.ExitCode = 1;
        //        break;
        //    }

        //    if (System.Diagnostics.Debugger.IsAttached)
        //    {
        //        _output.GreenLine("Reverting to interactive since you're debugging");
        //        RunInteractively();
        //    }
        //}

        public void RunInteractively()
        {
            var state = new EngineState(false);

            if (_environments.Current == null)
                new ChangeEnvironmentCommand(_output, new CommandArguments(), state, _environments).Execute();

            _output.Write("Enter command ");
            _output.Write(ConsoleColor.DarkGray, "('help' for help, 'exit' to quit)");
            _output.WriteLine(":");

            while (true)
            {
                var s = ReadLine.Read($"{_environments.CurrentName}> ");
                _output.WriteLine();
                if (string.IsNullOrWhiteSpace(s))
                    continue;

                try
                {
                    var commands = s.Split('\n');
                    for (var index = 0; index < s.Split('\n').Length; index++)
                    {
                        var ss = commands[index];
                        var sequence = new StringCharacterSequence(ss, $"command {index}");
                        var command = _parser.Parse(sequence).Value;
                        ExecuteCommand(command, state);
                        if (state.ShouldExit)
                            return;
                    }
                }
                catch (Exception e)
                {
                    _output.RedLine(e.Message);
                    _output.RedLine(e.StackTrace);
                }
            }
        }

        private void ExecuteCommand(CompleteCommand commandRequest, EngineState state)
        {
            var command = _commandSource.GetCommandInstance(commandRequest, _environments, state, _output) ?? new NotFoundCommandVerb(commandRequest.Verb, _output);
            command.Execute();
        }
    }
}
