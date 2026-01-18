using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Help;
using StoneFruit.Execution.IO;

namespace StoneFruit.Execution.Environments;

[Verb(Name)]
public class EnvironmentHandler : IHandler
{
    public const string Name = "env";
    public const string FlagList = "list";
    public const string FlagNotSet = "notset";
    public const string FlagClearData = "cleardata";

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

    public void Execute(IArguments arguments, HandlerContext context)
    {
        // If --list, list all environments and return
        if (arguments.HasFlag(FlagList))
        {
            ListEnvironments(context.Environments, context.Output);
            return;
        }

        MaybeChangeEnvironment(arguments, context)
            .OnSuccess(e =>
            {
                // If -cleardata and we are in an environment and we are not setting an environment
                // just clear the data in the current environment.
                if (arguments.HasFlag(FlagClearData))
                    e.ClearCache();
                context.State.OnEnvironmentChanged(e.Name);
            })
            .OnFailure(err => EnvironmentsException.Throw(err));
    }

    private static void ListEnvironments(IEnvironments environments, IOutput output)
    {
        var color = new Brush(ConsoleColor.Cyan);
        var envList = environments.GetNames();
        var currentEnv = environments.GetCurrentName();
        for (int i = 0; i < envList.Count; i++)
        {
            var index = i + 1;
            var env = envList[i];

            output
                .Write(index.ToString(), ConsoleColor.White)
                .Write(") ", ConsoleColor.DarkGray)
                .Write(env, currentEnv.Satisfies(ce => ce == env) ? color.Swap() : color)
                .WriteLine();
        }
    }

    private static Result<IEnvironment, EnvironmentError> MaybeChangeEnvironment(IArguments arguments, HandlerContext context)
    {
        var environments = context.Environments;
        // If -notset and we have a current env, bail out
        // It's an error, but not one that turns into an exception.
        if (arguments.HasFlag(FlagNotSet) && environments.GetCurrentName().IsSuccess)
            return new EnvironmentNotChanged();

        // We don't have an arg. If there is exactly 1 option, jump to that. Otherwise prompt
        // the user (if we're in interactive mode)
        var target = arguments.Shift();
        if (target.Exists())
            return TrySetEnvironment(target.AsString(), environments);

        // If we only have a single environment, switch directly to it with no input from the user
        // We can skip some checks because this name is from a known list of valid environment names
        var envList = environments.GetNames();
        if (envList.Count == 1)
            return environments.SetCurrent(envList[0]);

        return PromptUserForEnvironment(context);
    }

    private static Result<IEnvironment, EnvironmentError> PromptUserForEnvironment(HandlerContext context)
    {
        var state = context.State;
        var output = context.Output;
        // In headless mode we can't prompt, so at this point we just throw an exception
        if (state.RunMode == EngineRunMode.Headless)
            return new NoEnvironmentSpecifiedHeadless();

        // Use the env-list verb to show the list, then prompt the user to make a selection. Loop until
        // a valid selection is made.
        while (true)
        {
            output.WriteLine("Please select an environment:", ConsoleColor.DarkCyan);
            ListEnvironments(context.Environments, context.Output);

            var envIndex = context.Input.Prompt("", true, false).GetValueOrThrow();
            var result = TrySetEnvironment(envIndex, context.Environments);
            if (result.IsSuccess)
                return result;
        }
    }

    private static Result<IEnvironment, EnvironmentError> TrySetEnvironment(string arg, IEnvironments environments)
    {
        // No argument, nothing to do. Fail
        return Validate.IsNotNullOrEmpty(arg).ToResult(() => (EnvironmentError)new NoEnvironmentSpecified())
            .Bind(a => GetEnvironmentNameFromUserInput(environments.GetNames(), a))
            .Bind(environments.SetCurrent);
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
