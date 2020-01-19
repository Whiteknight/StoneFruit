﻿using System;
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

        private void Execute(CompleteCommand completeCommand)
        {
            Assert.ArgumentNotNull(completeCommand, nameof(completeCommand));
            var verbObject = Commands.GetCommandInstance(completeCommand, this);
            if (verbObject != null)
            {
                verbObject.Execute();
                return;
            }
            Output
                .Color(ConsoleColor.Red)
                .WriteLine($"Command '{completeCommand.Verb}' not found. Please check your spelling or help output and try again");
            State.VerbNotFound();
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
    }
}