using System;
using System.Linq;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Output;

namespace StoneFruit.Handlers
{
    [Verb(Name)]
    public class EnvironmentHandler : IHandler
    {
        public const string Name = "env";

        private readonly IOutput _output;
        private readonly IArguments _args;
        private readonly EngineState _state;
        private readonly IEnvironmentCollection _environments;
        private readonly CommandDispatcher _dispatcher;

        public EnvironmentHandler(IOutput output, IArguments args, EngineState state, IEnvironmentCollection environments, CommandDispatcher dispatcher)
        {
            _output = output;
            _args = args;
            _state = state;
            _environments = environments;
            _dispatcher = dispatcher;
        }

        public static string Group => HelpHandler.BuiltinsGroup;
        public static string Description => "List or change environments";

        public static string Usage => $@"{Name} [-list] [-notset] [ <name> | <number>]

{Name}
Show a prompt to change the current environment, if any are configured.

{Name} -list
Show a list of all environments

{Name} <envName>
Change directly to the specified environment

{Name} <number>
Change directly to the environment at the specified position

{Name} -notset [ <name> | <number> ]?
Change the current environment, only if any are configured but none are set.
";

        public void Execute()
        {
            if (_args.HasFlag("notset"))
            {
                if (_environments.GetCurrent().HasValue)
                    return;
                ChangeEnvironment();
                return;
            }

            if (_args.HasFlag("list"))
            {
                ListEnvironments();
                return;
            }

            // Otherwise we're switching environments
            ChangeEnvironment();
        }

        private void ListEnvironments()
        {
            var highlight = new Brush(ConsoleColor.Black, ConsoleColor.Cyan);
            var envList = _environments.GetNames();
            var currentEnvNameResult = _environments.GetCurrentName();
            var currentEnv = currentEnvNameResult.HasValue ? currentEnvNameResult.Value : "";
            for (int i = 0; i < envList.Count; i++)
            {
                var index = i + 1;
                var env = envList[i];

                _output
                    .Color(ConsoleColor.White).Write(index.ToString())
                    .Color(ConsoleColor.DarkGray).Write(") ")
                    .Color(env == currentEnv ? highlight : ConsoleColor.Cyan).Write(env)
                    .WriteLine();
            }
        }

        private void ChangeEnvironment()
        {
            // If invoked with an argument, it is the name or index of an environment. Attempt to set that
            // environment and exit
            var target = _args.Shift();
            if (target.Exists())
            {
                var envName = target.AsString();
                if (_environments.GetCurrentName().Equals(envName))
                    return;
                if (!TrySetEnvironment(envName))
                    throw new ExecutionException($"Could not set environment {envName}");
                OnEnvironmentChanged();
                return;
            }

            // If we only have a single environment, switch directly to it with no input from the user
            var environments = _environments.GetNames();
            if (environments.Count == 1)
            {
                _environments.SetCurrent(0);
                OnEnvironmentChanged();
                return;
            }

            PromptUserForEnvironment();
        }

        private void PromptUserForEnvironment()
        {
            // In headless mode we can't prompt, so at this point we just throw an exception
            if (_state.RunMode == EngineRunMode.Headless)
                throw new ExecutionException("Environment not specified in headless mode");

            // Use the env-list verb to show the list, then prompt the user to make a selection. Loop until
            // a valid selection is made.
            while (true)
            {
                _output.Color(ConsoleColor.DarkCyan).WriteLine("Please select an environment:");
                ListEnvironments();

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
            var currentEnvName = _environments.GetCurrentName().GetValueOrDefault("");
            var args = SyntheticArguments.From(("environment", currentEnvName));
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
                var asInt = int.Parse(arg) - 1;
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
