using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution;
using StoneFruit.Execution.Environments;
using StoneFruit.Execution.Exceptions;
using StoneFruit.Execution.Output;

namespace StoneFruit.Handlers;

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
    private readonly EnvironmentCollection _environments;

    public EnvironmentHandler(IOutput output, IArguments args, EngineState state, IEnvironmentCollection environments)
    {
        _output = output;
        _args = args;
        _state = state;
        _environments = (environments as EnvironmentCollection)
            ?? throw new InvalidOperationException($"Expected EnvironmentCollection but got {environments.GetType().Name}");
    }

    public static string Group => HelpHandler.BuiltinsGroup;

    public static string Description => "List or change environments";

    public static string Usage => $"""
        {Name} ...

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
        """;

    public void Execute()
    {
        // If --list, list all environments and return
        if (_args.HasFlag(FlagList))
        {
            ListEnvironments();
            return;
        }

        MaybeChangeEnvironment()
            .OnSuccess(_ =>
            {
                // If -cleardata and we are in an environment and we are not setting an environment
                // just clear the data in the current environment.
                if (_args.HasFlag(FlagClearData))
                    _environments.GetCurrent().OnSuccess(e => e.ClearCache());
                _state.OnEnvironmentChanged();
            })
            .MapError(err => err switch
            {
                NoEnvironmentSpecified => throw new EnvHandlerException("No environment specified and no default could be selected."),
                NoEnvironmentSpecifiedHeadless => throw new EnvHandlerException("No environment specified in headless mode and no default could be selected."),
                InvalidEnvironment ie => throw new EnvHandlerException($"Invalid environment {ie.NameOrNumber} specified. Environment not changed."),
                NoEnvironmentSet => err,
                _ => err
            });
    }

    private void ListEnvironments()
    {
        var color = new Brush(ConsoleColor.Cyan);
        var envList = _environments.GetNames();
        var currentEnv = _environments.GetCurrentName();
        for (int i = 0; i < envList.Count; i++)
        {
            var index = i + 1;
            var env = envList[i];

            _output
                .Color(ConsoleColor.White).Write(index.ToString())
                .Color(ConsoleColor.DarkGray).Write(") ")
                .Color(currentEnv.Satisfies(ce => ce == env) ? color.Swap() : color).Write(env)
                .Color(Brush.Default).WriteLine();
        }
    }

    private Result<IEnvironment, Error> MaybeChangeEnvironment()
    {
        // If -notset and we have a current env, bail out
        if (_args.HasFlag(FlagNotSet) && _environments.GetCurrentName().IsSuccess)
            return new NoEnvironmentSet();

        // We don't have an arg. If there is exactly 1 option, jump to that. Otherwise prompt
        // the user (if we're in interactive mode)
        var target = _args.Shift();
        if (target.Exists())
            return TrySetEnvironment(target.AsString());

        // If we only have a single environment, switch directly to it with no input from the user
        var environments = _environments.GetNames();
        if (environments.Count == 1)
        {
            _environments.SetCurrent(environments[0]);
            return GetCurrentEnvironment();
        }

        return PromptUserForEnvironment();
    }

    private static Maybe<string> GetEnvironmentNameFromUserInput(IReadOnlyList<string> environments, string envNameOrNumber)
    {
        string envName = envNameOrNumber;
        if (!envNameOrNumber.All(char.IsDigit))
            return envName;

        var asInt = int.Parse(envNameOrNumber) - 1;
        if (asInt >= 0 && asInt < environments.Count)
            return environments[asInt];

        return default;
    }

    private Result<IEnvironment, Error> PromptUserForEnvironment()
    {
        // In headless mode we can't prompt, so at this point we just throw an exception
        if (_state.RunMode == EngineRunMode.Headless)
            return new NoEnvironmentSpecifiedHeadless();

        // Use the env-list verb to show the list, then prompt the user to make a selection. Loop until
        // a valid selection is made.
        while (true)
        {
            _output.Color(ConsoleColor.DarkCyan).WriteLine("Please select an environment:");
            ListEnvironments();

            var envIndex = _output.Prompt("", true, false).GetValueOrThrow();
            var result = TrySetEnvironment(envIndex);
            if (result.IsSuccess)
                return result;
        }
    }

    private Result<IEnvironment, Error> TrySetEnvironment(string arg)
    {
        // No argument, nothing to do. Fail
        if (string.IsNullOrEmpty(arg))
            return new NoEnvironmentSpecified();

        return GetEnvironmentNameFromUserInput(_environments.GetNames(), arg)
            .ToResult<Error>(() => new NoEnvironmentSpecified())
            .Bind(envName =>
            {
                if (!_environments.IsValid(envName))
                    return new InvalidEnvironment(envName);
                _environments.SetCurrent(envName);
                return GetCurrentEnvironment();
            });
    }

    private Result<IEnvironment, Error> GetCurrentEnvironment()
        => _environments.GetCurrent().ToResult<Error>(() => new NoEnvironmentSet());

    private abstract record Error();
    private sealed record NoEnvironmentSpecified() : Error;
    private sealed record NoEnvironmentSpecifiedHeadless() : Error;
    private sealed record InvalidEnvironment(string NameOrNumber) : Error;
    private sealed record NoEnvironmentSet() : Error;

    private sealed class EnvHandlerException : InternalException
    {
        public EnvHandlerException(string message) : base(message)
        {
        }
    }
}
