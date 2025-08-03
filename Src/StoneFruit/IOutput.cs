using System;
using StoneFruit.Execution.IO;
using StoneFruit.Execution.Templating;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

/// <summary>
/// Abstraction for output.
/// </summary>
public interface IOutput
{
    ITemplateParser TemplateParser { get; }

    /// <summary>
    /// Get a new output using the given color palette. If the output does not support
    /// color this will be a no-op.
    /// </summary>
    /// <param name="changeBrush"></param>
    /// <returns></returns>
    IOutput Color(Func<Brush, Brush> changeBrush);

    /// <summary>
    /// Get a new output with the given color for text and the current background color
    /// unchanged. If the implementation does not support color this will be a no-op.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    IOutput Color(ConsoleColor color) => Color(_ => color);

    /// <summary>
    /// Get a new output with the given brush for text and background color. If the
    /// implementation does not support color this will be a no-op.
    /// </summary>
    /// <param name="brush"></param>
    /// <returns></returns>
    IOutput Color(Brush brush) => Color(_ => brush);

    /// <summary>
    /// Write a linebreak to the output.
    /// </summary>
    /// <returns></returns>
    IOutput WriteLine();

    /// <summary>
    /// Write the string representation of the object to the output, with trailing
    /// newline.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    IOutput WriteLine(object obj)
        => obj == null
            ? this
            : WriteLine(obj!.ToString()!);

    /// <summary>
    /// Write the given text followed by a line break to the output.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    IOutput WriteLine(string line);

    /// <summary>
    /// Write the given text to the output.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    IOutput Write(string str);

    /// <summary>
    /// Write the string representation of the object to the output.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    IOutput Write(object obj)
        => obj == null
            ? this
            : Write(obj!.ToString()!);

    IOutput WriteFormatted(string format, object value)
    {
        NotNullOrEmpty(format);
        var template = TemplateParser.Parse(format);
        template.Render(this, value);
        return this;
    }

    IOutput WriteLineFormatted(string format, object value)
        => WriteFormatted(format, value).WriteLine();
}

public interface IInput
{
    /// <summary>
    /// Show a prompt to the user to request input. For non-interactive outputs, this
    /// will be a no-op.
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="mustProvide"></param>
    /// <param name="keepHistory"></param>
    /// <returns></returns>
    Maybe<string> Prompt(string prompt, bool mustProvide = true, bool keepHistory = true);
}
