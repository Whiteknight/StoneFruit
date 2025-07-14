using System;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution;

namespace StoneFruit;

/// <summary>
/// Used for building up an engine and setting all necessary options.
/// </summary>
public interface IEngineBuilder
{
    public IServiceCollection Services { get; }

    /// <summary>
    /// Setup verbs and their handlers.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    IEngineBuilder SetupHandlers(Action<IHandlerSetup> setup);

    /// <summary>
    /// Setup environments, if any.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    IEngineBuilder SetupEnvironments(Action<IEnvironmentSetup> setup);

    /// <summary>
    /// Setup argument parsing and handling.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    IEngineBuilder SetupArguments(Action<IParserSetup> setup);

    /// <summary>
    /// Setup output.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    IEngineBuilder SetupIo(Action<IIoSetup> setup);

    /// <summary>
    /// Setup the scripts which are executed in response to various events.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    IEngineBuilder SetupEvents(Action<EngineEventCatalog> setup);

    /// <summary>
    /// Setup the settings for the engine.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    IEngineBuilder SetupSettings(Action<EngineSettings> setup);

    /// <summary>
    /// Set the accessor used to retrieve commandline arguments.
    /// </summary>
    /// <param name="commandLine"></param>
    /// <returns></returns>
    IEngineBuilder SetCommandLine(ICommandLine commandLine);
}

public static class EngineBuilderExtensions
{
    public static IEngineBuilder SetCommandLine(this IEngineBuilder builder, string commandLine)
        => builder.SetCommandLine(new StringCommandLine(commandLine));
}
