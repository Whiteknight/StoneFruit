using StoneFruit.BuiltInVerbs.Hidden;
using StoneFruit.Execution;

namespace StoneFruit
{
    public class CommandDispatcher
    {
        public CommandParser Parser { get; }
        public ICommandSource Commands { get; }
        public IEnvironmentCollection Environments { get; }
        public EngineState State { get; }
        public ITerminalOutput Output { get; }

        public CommandDispatcher(CommandParser parser, ICommandSource commands, IEnvironmentCollection environments, EngineState state, ITerminalOutput output)
        {
            Parser = parser;
            Commands = commands;
            Environments = environments;
            State = state;
            Output = output;
        }

        public void Execute(string commandString)
        {
            var completeCommand = Parser.ParseCommand(commandString);
            var verbObject = Commands.GetCommandInstance(completeCommand, this) ?? new NotFoundCommandVerb(completeCommand.Verb, Output);
            verbObject.Execute();
        }
    }
}