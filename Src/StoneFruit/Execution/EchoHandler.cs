using System;
using System.Linq;
using StoneFruit;
using StoneFruit.Execution.Help;

namespace StoneFruit.Execution;

[Verb(Name, Hide = true)]
public class EchoHandler : IHandler
{
    public const string Name = "echo";
    public const string FlagNoNewline = "nonewline";
    public const string FlagNoHeadless = "noheadless";
    public const string FlagIgnoreEmpty = "ignoreempty";
    public const string ArgColor = "color";

    public static string Group => HelpHandler.BuiltinsGroup;
    public static string Description => "Writes a string of output to the console";

    public static string Usage => $"""
        {Name} [{ArgColor}=<color>] [-{FlagNoNewline}] [-{FlagNoHeadless}] ...

            Writes all positional arguments to the output. If color is specified, use that color.
            Appends a new-line to the end unless -{FlagNoNewline} is specified.

            -{FlagNoHeadless} outputs nothing if the engine is in headless mode.
            -{FlagIgnoreEmpty} do nothing if there are no non-empty arguments
        """;

    public void Execute(IArguments arguments, HandlerContext context)
    {
        var state = context.State;
        // Some messages, especially internal ones, don't want to display in headless
        if (arguments.HasFlag(FlagNoHeadless) && state.RunMode == EngineRunMode.Headless)
            return;

        var output = GetOutput(arguments, context.Output);

        var strings = arguments.GetAllPositionals().Where(p => p.Exists()).Select(p => p.AsString());
        var line = string.Join(" ", strings);

        if (string.IsNullOrWhiteSpace(line) && arguments.HasFlag(FlagIgnoreEmpty))
            return;

        WriteToOutput(arguments, output, line);
    }

    private static void WriteToOutput(IArguments arguments, IOutput output, string line)
    {
        if (arguments.HasFlag(FlagNoNewline))
            output.Write(line);
        else
            output.WriteLine(line);
    }

    private static IOutput GetOutput(IArguments arguments, IOutput output)
    {
        var colorName = arguments.Get(ArgColor).AsString();
        if (string.IsNullOrEmpty(colorName))
            return output;

        var color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colorName);
        return output.Color(color);
    }
}
