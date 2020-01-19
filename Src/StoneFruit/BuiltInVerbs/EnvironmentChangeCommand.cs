﻿using System;
using System.Linq;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.BuiltInVerbs
{
    [CommandName(Name)]
    // TODO: Would like a way to not have this one listed in help
    [CommandName(NotSetName)]
    public class EnvironmentChangeCommand : ICommandVerb
    {
        public const string Name = "env-change";
        public const string NotSetName = "env-change-notset";

        private readonly ITerminalOutput _output;
        private readonly CompleteCommand _command;
        private readonly CommandArguments _args;
        private readonly EngineState _state;
        private readonly IEnvironmentCollection _environments;
        private readonly CommandDispatcher _dispatcher;

        public EnvironmentChangeCommand(ITerminalOutput output, CompleteCommand command, EngineState state, IEnvironmentCollection environments, CommandDispatcher dispatcher)
        {
            _output = output;
            _command = command;
            _args = command.Arguments;
            _state = state;
            _environments = environments;
            _dispatcher = dispatcher;
        }

        public static string Description => "Change environment if multiple environments are configured";

        public static string Help => @"change-env
Show a list of possible environments and a prompt to select the one you want.

change-env <envName>
Change directly to the specified environment

change-env <number>
Change directly to the environment at the specified position
";

        public void Execute()
        {
            if (_command.Verb == NotSetName)
            {
                if (_environments.Current != null)
                    return;
            }

            ChangeEnvironment();
        }

        private void ChangeEnvironment()
        {
            var target = _args.Shift();
            if (target.Exists())
            {
                if (!TrySetEnvironment(target.Value))
                    throw new Exception($"Could not set environment {target.Value}");

                return;
            }

            var environments = _environments.GetNames();
            if (environments.Count == 1)
            {
                _environments.SetCurrent(1);
                _state.EnvironmentChanged();
                return;
            }

            if (_state.Headless)
                throw new Exception("Environment not specified in headless mode");

            while (true)
            {
                _output.Color(ConsoleColor.DarkCyan).WriteLine("Please select an environment:");
                _dispatcher.Execute(EnvironmentListCommand.Name);

                var envIndex = _output.Prompt("", true, false);
                if (TrySetEnvironment(envIndex))
                {
                    _state.EnvironmentChanged();
                    break;
                }
            }
        }

        private bool TrySetEnvironment(string arg)
        {
            if (string.IsNullOrEmpty(arg))
                return false;

            if (arg.All(char.IsDigit))
            {
                var asInt = int.Parse(arg);
                if (_environments.IsValid(asInt))
                {
                    _environments.SetCurrent(asInt);
                    return true;
                }
            }

            if (_environments.IsValid(arg))
            {
                _environments.SetCurrent(arg);
                return true;
            }

            _output.Color(ConsoleColor.Red).WriteLine($"Unknown environment '{arg}'");
            return false;
        }
    }
}
