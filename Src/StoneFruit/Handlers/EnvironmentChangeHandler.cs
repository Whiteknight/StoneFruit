using System;
using System.Linq;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Handlers
{
    [Verb(Name)]
    [Verb(NotSetName, showInHelp: false)]
    public class EnvironmentChangeHandler : IHandler
    {
        public const string Name = "env-change";
        public const string NotSetName = "env-change-notset";

        private readonly IOutput _output;
        private readonly Command _command;
        private readonly CommandArguments _args;
        private readonly EngineState _state;
        private readonly IEnvironmentCollection _environments;
        private readonly CommandDispatcher _dispatcher;

        public EnvironmentChangeHandler(IOutput output, Command command, EngineState state, IEnvironmentCollection environments, CommandDispatcher dispatcher)
        {
            _output = output;
            _command = command;
            _args = command.Arguments;
            _state = state;
            _environments = environments;
            _dispatcher = dispatcher;
        }

        public static string Description => "Change environment if multiple environments are configured";

        public static string Usage => $@"{Name}
Show a list of possible environments and a prompt to select the one you want.

{Name} <envName>
Change directly to the specified environment

{Name} <number>
Change directly to the environment at the specified position

To prompt the user for an environment only if one is not currently set, use the {NotSetName} verb instead
";

        public void Execute()
        {
            // If we're executing as notset, only prompt the user if we don't have an environment set
            if (_command.Verb == NotSetName)
            {
                if (_environments.Current != null)
                    return;
            }

            // Otherwise do the normal environment switching logic
            ChangeEnvironment();
        }

        private void ChangeEnvironment()
        {
            // If invoked with an argument, it is the name or index of an environment. Attempt to set that
            // environment and exit
            var target = _args.Shift();
            if (target.Exists())
            {
                var envName = target.AsString();
                if (envName == _environments.CurrentName)
                    return;
                if (!TrySetEnvironment(envName))
                    // TODO: Use a better exception type
                    throw new Exception($"Could not set environment {envName}");
                OnEnvironmentChanged();
                return;
            }

            // If we only have a single environment, switch directly to it with no input from the user
            var environments = _environments.GetNames();
            if (environments.Count == 1)
            {
                _environments.SetCurrent(1);
                OnEnvironmentChanged();
                return;
            }

            PromptUserForEnvironment();
        }

        private void PromptUserForEnvironment()
        {
            // In headless mode we can't prompt, so at this point we just throw an exception
            if (_state.Headless)
                throw new Exception("Environment not specified in headless mode");

            // Use the env-list verb to show the list, then prompt the user to make a selection. Loop until
            // a valid selection is made.
            while (true)
            {
                _output.Color(ConsoleColor.DarkCyan).WriteLine("Please select an environment:");
                _dispatcher.Execute(EnvironmentListHandler.Name);

                var envIndex = _output.Prompt("", true, false);
                if (TrySetEnvironment(envIndex))
                {
                    OnEnvironmentChanged();
                    break;
                }
            }
        }

        private void OnEnvironmentChanged()
        {
            var script = _state.EventCatalog.EnvironmentChanged;
            var args = new CommandArguments(new[] { new NamedArgument("environment", _environments.CurrentName) });
            _state.Commands.Prepend(script.GetCommands(_dispatcher.Parser, args));
        }

        private bool TrySetEnvironment(string arg)
        {
            // No argument, nothing to do. Fail
            if (string.IsNullOrEmpty(arg))
                return false;

            // Argument is a number. Set the environment by index
            if (arg.All(char.IsDigit))
            {
                var asInt = int.Parse(arg);
                if (_environments.IsValid(asInt))
                {
                    _environments.SetCurrent(asInt);
                    return true;
                }
            }

            // Argument is a name of a valid environment. Set it and exit.
            if (_environments.IsValid(arg))
            {
                _environments.SetCurrent(arg);
                return true;
            }

            // Invalid selection, show an error and return failure
            _output.Color(ConsoleColor.Red).WriteLine($"Unknown environment '{arg}'");
            return false;
        }
    }
}
