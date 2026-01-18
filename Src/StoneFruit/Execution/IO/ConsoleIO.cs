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
    private readonly ITemplateParser _templateParser;

    public ConsoleIO(ITemplateParser parser)
    {
        _templateParser = parser;
    }

    public IOutput WriteMessage(OutputMessage message)
    {
        if (message.IsTemplate)
        {
            var template = _templateParser.Parse(message.Text ?? string.Empty);
            var messages = template.Render(message.Object.GetValueOrDefault(new object()));
            foreach (var msg in messages)
                WriteMessage(msg with { IsError = message.IsError });
            if (message.IncludeNewline)
                Console.WriteLine();
            return this;
        }

        if (message.IsError)
            WriteError(message.Text ?? string.Empty, message.Brush);
        else
            ToStandardOutWithBrush(Either(message.Brush, Brush.Default), message.Text ?? string.Empty);

        if (message.IncludeNewline)
            Console.WriteLine();

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

    private static void WriteError(string text, Brush brush)
    {
        // If Error is redirected, we just write it to the error stream
        // Otherwise, if brush is default, we should write it out to standard out with red foreground
        if (Console.IsErrorRedirected)
        {
            Console.Error.Write(text);
            return;
        }

        ToStandardOutWithBrush(Either(brush, ConsoleColor.Red), text);
    }

    private static Brush Either(Brush brush, Brush defaultBrush)
        => brush == default ? defaultBrush : brush;

    private static void ToStandardOutWithBrush(Brush brush, string text)
    {
        if (brush == default || Console.IsOutputRedirected)
        {
            Console.Write(text);
            return;
        }

        var current = Brush.Current;
        var (foreground, background) = brush.GetConsoleColors();
        Console.ForegroundColor = foreground;
        Console.BackgroundColor = background;
        try
        {
            Console.Write(text);
        }
        finally
        {
            (foreground, background) = current.GetConsoleColors();
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }
    }
}
