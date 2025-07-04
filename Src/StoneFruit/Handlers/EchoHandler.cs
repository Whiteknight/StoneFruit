using System;
using System.Linq;
using StoneFruit.Execution;

namespace StoneFruit.Handlers;

[Verb(Name, Hide = true)]
public class EchoHandler : IHandler
{
    public const string Name = "echo";
    public const string FlagNoNewline = "nonewline";
    public const string FlagNoHeadless = "noheadless";
    public const string ArgColor = "color";

    private readonly EngineState _state;
    private readonly IOutput _output;
    private readonly IArguments _args;

    public EchoHandler(EngineState state, IOutput output, IArguments args)
    {
        _state = state;
        _output = output;
        _args = args;
    }

    public static string Group => HelpHandler.BuiltinsGroup;
    public static string Description => "Writes a string of output to the console";

    public static string Usage => $"""
        {Name} [{ArgColor}=<color>] [-{FlagNoNewline}] [-{FlagNoHeadless}] ...

            Writes all positional arguments to the output. If color is specified, use that color.
            Appends a new-line to the end unless -{FlagNoNewline} is specified.

            -{FlagNoHeadless} outputs nothing if the engine is in headless mode.
        """;

    public void Execute()
    {
        // Some messages, especially internal ones, don't want to display in headless
        if (_args.HasFlag(FlagNoHeadless) && _state.RunMode == EngineRunMode.Headless)
            return;

        var output = GetOutput();

        var strings = _args.GetAllPositionals().Where(p => p.Exists()).Select(p => p.AsString());
        var line = string.Join(" ", strings);
        WriteToOutput(output, line);
    }

    private void WriteToOutput(IOutput output, string line)
    {
        if (_args.HasFlag(FlagNoNewline))
            output.Write(line);
        else
            output.WriteLine(line);
    }

    private IOutput GetOutput()
    {
        var output = _output;
        var colorName = _args.Get(ArgColor).AsString();
        if (string.IsNullOrEmpty(colorName))
            return output;

        var color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colorName);
        return output.Color(color);
    }
}
