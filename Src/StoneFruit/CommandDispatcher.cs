using ParserObjects;
using ParserObjects.Sequences;
using StoneFruit.BuiltInVerbs.Hidden;
using StoneFruit.Execution;

namespace StoneFruit
{
    public class CommandDispatcher
    {
        private readonly IParser<char, CompleteCommand> _parser;
        public ICommandSource Commands { get; }
        public IEnvironmentCollection Environments { get; }
        public EngineState State { get; }
        public ITerminalOutput Output { get; }

        public CommandDispatcher(IParser<char, CompleteCommand> parser, ICommandSource commands, IEnvironmentCollection environments, EngineState state, ITerminalOutput output)
        {
            _parser = parser;
            Commands = commands;
            Environments = environments;
            State = state;
            Output = output;
        }

        public void Execute(string commandString)
        {
            var sequence = new StringCharacterSequence(commandString);
            var completeCommand = _parser.Parse(sequence).Value;
            var verbObject = Commands.GetCommandInstance(completeCommand, this) ?? new NotFoundCommandVerb(completeCommand.Verb, Output);
            verbObject.Execute();
        }
    }
}