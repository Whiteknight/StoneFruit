using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.IO;
using StoneFruit.Handlers;

namespace StoneFruit.Execution.Environments;

[Verb(Name)]
public class EnvironmentHandler : IHandler
{
    public const string Name = "env";
    public const string FlagList = "list";
    public const string FlagNotSet = "notset";
    public const string FlagClearData = "cleardata";

    private readonly IOutput _output;
    private readonly IInput _input;
    private readonly IArguments _args;
    private readonly EngineState _state;
    private readonly IEnvironments _environments;

    public EnvironmentHandler(IOutput output, IInput input, IArguments args, EngineState state, IEnvironments environments)
    {
        _output = output;
        _input = input;
        _args = args;
        _state = state;
        _environments = environments;
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
            .OnSuccess(e =>
            {
                // If -cleardata and we are in an environment and we are not setting an environment
                // just clear the data in the current environment.
                if (_args.HasFlag(FlagClearData))
                    e.ClearCache();
                _state.OnEnvironmentChanged(e.Name);
            })
            .OnFailure(err => EnvironmentsException.Throw(err));
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

    private Result<IEnvironment, EnvironmentError> MaybeChangeEnvironment()
    {
        // If -notset and we have a current env, bail out
        // It's an error, but not one that turns into an exception.
        if (_args.HasFlag(FlagNotSet) && _environments.GetCurrentName().IsSuccess)
            return new EnvironmentNotChanged();

        // We don't have an arg. If there is exactly 1 option, jump to that. Otherwise prompt
        // the user (if we're in interactive mode)
        var target = _args.Shift();
        if (target.Exists())
            return TrySetEnvironment(target.AsString());

        // If we only have a single environment, switch directly to it with no input from the user
        // We can skip some checks because this name is from a known list of valid environment names
        var environments = _environments.GetNames();
        if (environments.Count == 1)
            return _environments.SetCurrent(environments[0]);

        return PromptUserForEnvironment();
    }

    private Result<IEnvironment, EnvironmentError> PromptUserForEnvironment()
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

            var envIndex = _input.Prompt("", true, false).GetValueOrThrow();
            var result = TrySetEnvironment(envIndex);
            if (result.IsSuccess)
                return result;
        }
    }

    private Result<IEnvironment, EnvironmentError> TrySetEnvironment(string arg)
    {
        // No argument, nothing to do. Fail
        if (string.IsNullOrEmpty(arg))
            return new NoEnvironmentSpecified();

        return GetEnvironmentNameFromUserInput(_environments.GetNames(), arg)
            .Bind(_environments.SetCurrent);
    }

    private static Result<string, EnvironmentError> GetEnvironmentNameFromUserInput(IReadOnlyList<string> environments, string envNameOrNumber)
    {
        if (int.TryParse(envNameOrNumber, out var asInt) && asInt > 0 && asInt <= environments.Count)
            return environments[asInt - 1];

        if (environments.Contains(envNameOrNumber))
            return envNameOrNumber;

        return new InvalidEnvironment(envNameOrNumber);
    }
}
