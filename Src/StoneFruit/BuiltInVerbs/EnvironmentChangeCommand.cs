using System;
using System.Linq;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.BuiltInVerbs
{
    [CommandName(EnvironmentChangeCommand.Name)]
    public class EnvironmentChangeCommand : ICommandVerb
    {
        public const string Name = "env-change";

        private readonly ITerminalOutput _output;
        private readonly CommandArguments _args;
        private readonly EngineState _state;
        private readonly IEnvironmentCollection _environments;
        private readonly CommandDispatcher _dispatcher;

        public EnvironmentChangeCommand(ITerminalOutput output, CommandArguments args, EngineState state, IEnvironmentCollection environments, CommandDispatcher dispatcher)
        {
            _output = output;
            _args = args;
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
                return;
            }

            if (_state.Headless)
                throw new Exception("Environment not specified in headless mode");

            while (true)
            {
                _output.WriteLine(ConsoleColor.DarkCyan, "Please select an environment:");
                _dispatcher.Execute(EnvironmentListCommand.Name);

                var envIndex = _output.Prompt("", true, false);
                if (TrySetEnvironment(envIndex))
                    break;
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

            _output.RedLine($"Unknown environment '{arg}'");
            return false;
        }
    }
}
