using System;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Output;

/// <summary>
/// An ITerminalOutput adaptor for System.Console and ReadLine.
/// </summary>
public class ConsoleOutput : IOutput
{
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
            if (keepHistory)
                ReadLine.AddHistory(value);
            return value;
        }
    }
}
