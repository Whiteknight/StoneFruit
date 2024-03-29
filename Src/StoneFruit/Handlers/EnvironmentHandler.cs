﻿using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution;
using StoneFruit.Execution.Environments;
using StoneFruit.Execution.Output;

namespace StoneFruit.Handlers
{
    [Verb(Name)]
    public class EnvironmentHandler : IHandler
    {
        public const string Name = "env";
        public const string FlagList = "list";
        public const string FlagNotSet = "notset";
        public const string FlagClearData = "cleardata";

        private readonly IOutput _output;
        private readonly IArguments _args;
        private readonly EngineState _state;
        private readonly IEnvironmentCollection _environments;
        private readonly EnvironmentObjectCache _objectCache;

        public EnvironmentHandler(IOutput output, IArguments args, EngineState state, IEnvironmentCollection environments, ICommandParser parser, EnvironmentObjectCache objectCache)
        {
            _output = output;
            _args = args;
            _state = state;
            _environments = environments;
            _objectCache = objectCache;
        }

        public static string Group => HelpHandler.BuiltinsGroup;
        public static string Description => "List or change environments";

        public static string Usage => $@"{Name} ...

{Name}
Show a prompt to change the current environment, if any are configured.

{Name} -{FlagList}
Show a list of all environments. All other arguments are ignored.

{Name} <envName> [ -{FlagNotSet} ]? [ -{FlagClearData} ]?
Change directly to the specified environment.

{Name} <number> [ -{FlagNotSet} ]? [ -{FlagClearData} ]?
Change directly to the environment at the specified position.

-{FlagNotSet}: Only change if not in an environment.
-{FlagClearData}: Clear contextual data when the environment is changed.

{Name} -{FlagClearData}
Clear the data for the current environment.
";

        public void Execute()
        {
            // If --list, list all environments and return
            if (_args.HasFlag(FlagList))
            {
                ListEnvironments();
                return;
            }

            var target = _args.Shift();
            var currentEnvNameResult = _environments.GetCurrentName();

            // If -cleardata and we are in an environment and we are not setting an environment
            // just clear the data in the current environment.
            if (!target.Exists() && currentEnvNameResult.HasValue && _args.HasFlag(FlagClearData))
            {
                _objectCache.Clear(currentEnvNameResult.Value);
                return;
            }

            // Otherwise we're switching environments
            if (!_args.HasFlag(FlagNotSet) || !_environments.GetCurrentName().HasValue)
                ChangeEnvironment(target, currentEnvNameResult);
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

        private void ChangeEnvironment(IPositionalArgument target, IResult<string> currentEnvNameResult)
        {
            // If invoked with an argument, it is the name or index of an environment. Attempt to set that
            // environment and exit

            var environments = _environments.GetNames();

            // We don't have an arg. If there is exactly 1 option, jump to that. Otherwise prompt
            // the user (if we're in interactive mode)
            if (!target.Exists())
            {
                // If we only have a single environment, switch directly to it with no input from the user
                if (environments.Count == 1)
                {
                    _environments.SetCurrent(environments[0]);
                    if (_args.HasFlag(FlagClearData))
                        _objectCache.Clear(_environments.GetCurrentName().Value);
                    _state.OnEnvironmentChanged();
                    return;
                }

                PromptUserForEnvironment();
                return;
            }

            // Get the env name. If we are given a number, parse it and use it as an index into the
            // list of names
            var envNameOrNumber = target.AsString();
            var envNameResult = GetEnvironmentNameFromUserInput(environments, envNameOrNumber.ToString());
            if (!envNameResult.HasValue)
                throw new ExecutionException($"Cannot switch to environment {envNameOrNumber.ToString()}. Not a valid name or index.");

            var envName = envNameResult.Value;
            if (!_environments.IsValid(envName))
                throw new ExecutionException($"Unknown environment '{envName}'");

            if (_args.HasFlag(FlagClearData))
                _objectCache.Clear(_environments.GetCurrentName().Value);

            if (currentEnvNameResult.Equals(envName))
                return;

            _environments.SetCurrent(envName);
            _state.OnEnvironmentChanged();
        }

        private static IResult<string> GetEnvironmentNameFromUserInput(IReadOnlyList<string> environments, string envNameOrNumber)
        {
            string envName = envNameOrNumber;
            if (!envNameOrNumber.All(char.IsDigit))
                return Result.Success(envName);

            var asInt = int.Parse(envNameOrNumber) - 1;
            if (asInt >= 0 && asInt < environments.Count)
            {
                envName = environments[asInt];
                return Result.Success(envName);
            }

            return FailureResult<string>.Instance;
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
                if (!TrySetEnvironment(envIndex))
                    continue;

                _state.OnEnvironmentChanged();
                break;
            }
        }

        private bool TrySetEnvironment(string arg)
        {
            // No argument, nothing to do. Fail
            if (string.IsNullOrEmpty(arg))
                return false;

            var envNameResult = GetEnvironmentNameFromUserInput(_environments.GetNames(), arg);
            if (!envNameResult.HasValue || !_environments.IsValid(envNameResult.Value))
            {
                _output
                    .Color(ConsoleColor.Red)
                    .WriteLine($"Invalid environment '{envNameResult.GetValueOrDefault("")}'");
                return false;
            }

            _environments.SetCurrent(envNameResult.Value);
            return true;
        }
    }
}
