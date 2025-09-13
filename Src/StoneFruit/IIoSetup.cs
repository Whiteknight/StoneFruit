using Microsoft.Extensions.Logging;
using StoneFruit.Execution.IO;
using StoneFruit.Execution.Templating;

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
}
