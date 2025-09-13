using System;
using StoneFruit.Execution.Templating;

namespace StoneFruit.Execution.IO;

public class ConsoleColorOutput : IOutput
{
    private readonly Brush _brush;

    public ITemplateParser TemplateParser => Inner.TemplateParser;

    public ConsoleColorOutput(Brush color, IOutput inner)
    {
        _brush = color;
        Inner = inner;
    }

    public IOutput Inner { get; }

    public IColorOutputFactory ColorOutputFactory => Inner.ColorOutputFactory;

    public IOutput WriteLine() => WithBrush(string.Empty, static (inner, _) => inner.WriteLine());

    public IOutput WriteLine(string line) => WithBrush(line, static (inner, t) => inner.WriteLine(t));

    public IOutput Write(string str) => WithBrush(str, static (inner, t) => inner.Write(t));

    private ConsoleColorOutput WithBrush(string text, Action<IOutput, string> act)
    {
        var current = Brush.Current;
        var (foreground, background) = _brush.GetConsoleColors();
        Console.ForegroundColor = foreground;
        Console.BackgroundColor = background;
        try
        {
            act(Inner, text);
        }
        finally
        {
            (foreground, background) = current.GetConsoleColors();
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        return this;
    }
}
