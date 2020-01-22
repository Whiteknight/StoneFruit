using System;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Utility;

namespace StoneFruit
{
    /// <summary>
    /// Handles dispatch of commands to appropriate ICommandVerb objects
    /// </summary>
    public class CommandDispatcher
    {
        public CommandParser Parser { get; }
        public ICommandVerbSource Commands { get; }
        public IEnvironmentCollection Environments { get; }
        public EngineState State { get; }
        public ITerminalOutput Output { get; }

        public CommandDispatcher(CommandParser parser, ICommandVerbSource commands, IEnvironmentCollection environments, EngineState state, ITerminalOutput output)
        {
            Parser = parser;
            Commands = commands;
            Environments = environments;
            State = state;
            Output = output;
        }

        private void Execute(CompleteCommand completeCommand)
        {
            Assert.ArgumentNotNull(completeCommand, nameof(completeCommand));
            var verbObject = Commands.GetInstance(completeCommand, this) ?? new NotFoundVerb(completeCommand, State, Output);
            verbObject.Execute();
        }

        public void Execute(string commandString)
        {
            Assert.ArgumentNotNullOrEmpty(commandString, nameof(commandString));
            var completeCommand = Parser.ParseCommand(commandString);
            Execute(completeCommand);
        }

        public void Execute(string verb, CommandArguments args)
        {
            Assert.ArgumentNotNullOrEmpty(verb, nameof(verb));
            var completeCommand = new CompleteCommand(verb, args);
            Execute(completeCommand);
        }

        private class NotFoundVerb : ICommandVerb
        {
            private readonly CompleteCommand _command;
            private readonly EngineState _state;
            private readonly ITerminalOutput _output;

            public NotFoundVerb(CompleteCommand command, EngineState state, ITerminalOutput output)
            {
                _command = command;
                _state = state;
                _output = output;
            }

            public void Execute()
            {
                _output
                    .Color(ConsoleColor.Red)
                    .WriteLine($"Command '{_command.Verb}' not found. Please check your spelling or help output and try again");
                _state.VerbNotFound();
            }
        }
    }
}