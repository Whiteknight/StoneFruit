using System;
using System.Diagnostics.CodeAnalysis;
using StoneFruit.Execution.Templating;

namespace StoneFruit.Execution.IO;

/// <summary>
/// An ITerminalOutput adaptor for System.Console and ReadLine.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Hard to test System.Console without capturing IO streams")]
public class ConsoleIO : IOutput, IInput
{
    public ConsoleIO(ITemplateParser parser)
    {
        TemplateParser = parser;
    }

    public ITemplateParser TemplateParser { get; }

    public IOutput WriteLine(string? line = "", Brush brush = default)
    {
        WithBrush(brush, line ?? string.Empty, Console.WriteLine);
        return this;
    }

    public IOutput Write(string str, Brush brush = default)
    {
        WithBrush(brush, str, Console.Write);
        return this;
    }

    public Maybe<string> Prompt(string prompt, bool mustProvide = true, bool keepHistory = true)
    {
        while (true)
        {
            var value = ReadLine.Read(prompt + "> ");
            if (string.IsNullOrEmpty(value) && mustProvide)
                continue;
            if (string.IsNullOrEmpty(value))
                return default;
            if (keepHistory)
                ReadLine.AddHistory(value);
            return value;
        }
    }

    private static void WithBrush(Brush brush, string text, Action<string> act)
    {
        if (brush == default || Console.IsOutputRedirected)
        {
            act(text);
            return;
        }

        var current = Brush.Current;
        var (foreground, background) = brush.GetConsoleColors();
        Console.ForegroundColor = foreground;
        Console.BackgroundColor = background;
        try
        {
            act(text);
        }
        finally
        {
            (foreground, background) = current.GetConsoleColors();
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }
    }
}
