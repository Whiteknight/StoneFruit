using Microsoft.Extensions.Logging;
using StoneFruit.Execution.IO;
using StoneFruit.Execution.Templating;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

/// <summary>
/// Setup the output streams.
/// </summary>
public interface IIoSetup
{
    /// <summary>
    /// Specify the type of a custom IOutput implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IIoSetup UseOutput<T>()
        where T : class, IOutput;

    /// <summary>
    /// Provide a custom IOutput instance to use.
    /// </summary>
    /// <param name="output"></param>
    /// <returns></returns>
    IIoSetup UseOutput(IOutput output);

    /// <summary>
    /// Specify the type of a custom IInput implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IIoSetup UseInput<T>()
        where T : class, IInput;

    /// <summary>
    /// Provide a custom IInput instance to use.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    IIoSetup UseInput(IInput input);

    /// <summary>
    /// Set an object representing commandline arguments passed into the application.
    /// </summary>
    /// <param name="commandLine"></param>
    /// <returns></returns>
    IIoSetup SetCommandLine(ICommandLine commandLine);

    /// <summary>
    /// Set a string of arguments that you would like to use as the commandline arguments passed
    /// into the project.
    /// </summary>
    /// <param name="commandLine"></param>
    /// <returns></returns>
    IIoSetup SetCommandLine(string commandLine);

    /// <summary>
    /// Specify a custom template parser instance to customize parsing syntax and semantics.
    /// </summary>
    /// <param name="parser"></param>
    /// <returns></returns>
    IIoSetup UseTemplateParser(ITemplateParser parser);

    /// <summary>
    /// Specify the log level to use for the default logger implementation.
    /// This only affects the built-in logger implementation. If you are using a custom logger this
    /// value will be ignored.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    IIoSetup UseLogLevel(LogLevel logLevel);

    /// <summary>
    /// Specify a color to associate with each log level.
    /// This only affects the built-in logger implementation. If you are using a custom logger this
    /// value will be ignored.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="brush"></param>
    /// <returns></returns>
    IIoSetup SetLogColor(LogLevel logLevel, Brush brush);

    /// <summary>
    /// For handlers which return a value, specify a custom object writer to handle writing those
    /// objects the output.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IIoSetup UseObjectWriter<T>()
        where T : class, IObjectOutputWriter;

    /// <summary>
    /// For handlers which return a value, write them to the output as formatted json.
    /// </summary>
    /// <returns></returns>
    public IIoSetup UseJsonObjectWriter()
        => UseObjectWriter<JsonObjectOutputWriter>();
}

public record struct OutputMessage(string Text, Maybe<object> Object = default, Brush Brush = default, bool IncludeNewline = false, bool IsError = false, bool IsTemplate = false);

/// <summary>
/// Abstraction for output.
/// </summary>
public interface IOutput
{
    IOutput WriteMessage(OutputMessage message);

    /// <summary>
    /// Write a linebreak to the output.
    /// </summary>
    /// <returns></returns>
    IOutput WriteLine(string line = "", Brush brush = default)
        => WriteMessage(new OutputMessage(line ?? string.Empty, Brush: brush, IncludeNewline: true));

    /// <summary>
    /// Write the string representation of the object to the output, with trailing
    /// newline.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    IOutput WriteLine(object obj, Brush brush = default)
        => WriteLine(obj?.ToString() ?? string.Empty, brush);

    IOutput Write(string str, Brush brush = default)
        => WriteMessage(new OutputMessage(str ?? string.Empty, Brush: brush));

    /// <summary>
    /// Write the string representation of the object to the output.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    IOutput Write(object obj, Brush brush = default)
        => obj == null
            ? this
            : Write(obj!.ToString()!, brush);

    IOutput WriteError(string str)
        => WriteMessage(new OutputMessage(str ?? string.Empty, IncludeNewline: true, IsError: true));

    IOutput WriteFormatted(string format, object? value = null)
        => WriteMessage(new OutputMessage(NotNull(format), value, IsTemplate: true));

    IOutput WriteLineFormatted(string format, object? value = null)
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

public interface IObjectOutputWriter
{
    void WriteObject<T>(T obj);

    void MaybeWriteObject<T>(T? obj)
        where T : class
    {
        if (obj != null)
            WriteObject(obj);
    }
}
