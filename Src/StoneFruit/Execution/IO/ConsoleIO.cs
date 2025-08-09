using System;
using System.Diagnostics.CodeAnalysis;
using StoneFruit.Execution.Templating;
using StoneFruit.Utility;

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

    public IOutput Color(Func<Brush, Brush> changeBrush)
    {
        Assert.NotNull(changeBrush);
        var brush = changeBrush(Brush.Current);
        return brush.Equals(Brush.Current) ? this : new ColoredOutputWrapper(brush, this);
    }

    public IOutput WriteLine()
    {
        Console.WriteLine();
        return this;
    }

    public IOutput WriteLine(string line)
    {
        Console.WriteLine(line);
        return this;
    }

    public IOutput Write(string str)
    {
        Console.Write(str);
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
}
