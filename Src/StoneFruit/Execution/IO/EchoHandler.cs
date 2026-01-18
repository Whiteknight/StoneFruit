using System.Linq;
using StoneFruit;
using StoneFruit.Execution.Help;

namespace StoneFruit.Execution.IO;

[Verb(Name, Hide = true)]
public class EchoHandler : IHandler
{
    public const string Name = "echo";
    public const string FlagNoNewline = "nonewline";
    public const string FlagNoHeadless = "noheadless";
    public const string FlagIgnoreEmpty = "ignoreempty";
    public const string FlagError = "error";
    public const string ArgColor = "color";

    public static string Group => HelpHandler.BuiltinsGroup;
    public static string Description => "Writes a string of output to the console";

    public static string Usage => $"""
        {Name} [{ArgColor}=<color>] [-{FlagNoNewline}] [-{FlagNoHeadless}] [-{FlagError}] ...

            Writes all positional arguments to the output. If color is specified, use that color.
            Appends a new-line to the end unless -{FlagNoNewline} is specified.

            -{FlagNoHeadless} outputs nothing if the engine is in headless mode.
            -{FlagIgnoreEmpty} do nothing if there are no non-empty arguments.
            -{FlagError} Treats the output as an error message.
        """;

    public void Execute(IArguments arguments, HandlerContext context)
    {
        var state = context.State;
        // Some messages, especially internal ones, don't want to display in headless
        if (arguments.HasFlag(FlagNoHeadless) && state.RunMode == EngineRunMode.Headless)
            return;

        Brush brush = GetBrush(arguments);
        string line = GetText(arguments);

        if (string.IsNullOrWhiteSpace(line) && arguments.HasFlag(FlagIgnoreEmpty))
            return;

        context.Output.WriteMessage(new OutputMessage(line, default, brush, !arguments.HasFlag(FlagNoNewline), arguments.HasFlag(FlagError)));
    }

    private static string GetText(IArguments arguments)
    {
        var strings = arguments.GetAllPositionals().Where(p => p.Exists()).Select(p => p.AsString());
        return string.Join(" ", strings);
    }

    private static Brush GetBrush(IArguments arguments)
    {
        var colorArg = arguments.Get(ArgColor);
        if (!colorArg.Exists())
            return default;

        var colorName = colorArg.AsString();
        return string.IsNullOrEmpty(colorName)
            ? default
            : Brush.Parse(colorName);
    }
}
