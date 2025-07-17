using System;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Environments;
using StoneFruit.Execution.Handlers;
using StoneFruit.Execution.IO;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

/// <summary>
/// Used to setup all options and dependencies of the Engine.
/// </summary>
public sealed class StoneFruitApplicationBuilder
{
    private readonly HandlerSetup _handlers;
    private readonly IoSetup _output;
    private readonly EngineEventCatalog _eventCatalog;
    private readonly EnvironmentSetup _environments;
    private readonly ParserSetup _parsers;
    private readonly EngineSettings _settings;

    public IServiceCollection Services { get; }

    private StoneFruitApplicationBuilder(IServiceCollection services)
    {
        Services = NotNull(services);
        _handlers = new HandlerSetup(Services);
        _eventCatalog = new EngineEventCatalog();
        _output = new IoSetup();
        _environments = new EnvironmentSetup();
        _parsers = new ParserSetup();
        _settings = new EngineSettings();
    }

    public static StoneFruitApplicationBuilder Create(IServiceCollection? services = null)
    {
        return new StoneFruitApplicationBuilder(services ?? new ServiceCollection());
    }

    /// <summary>
    /// Setup verbs and their handlers.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    public StoneFruitApplicationBuilder SetupHandlers(Action<IHandlerSetup> setup)
    {
        setup?.Invoke(_handlers);
        return this;
    }

    /// <summary>
    /// Setup environments, if any.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    public StoneFruitApplicationBuilder SetupEnvironments(Action<IEnvironmentSetup> setup)
    {
        setup?.Invoke(_environments);
        return this;
    }

    /// <summary>
    /// Setup argument parsing and handling.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    public StoneFruitApplicationBuilder SetupArguments(Action<IParserSetup> setup)
    {
        setup?.Invoke(_parsers);
        return this;
    }

    /// <summary>
    /// Setup output.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    public StoneFruitApplicationBuilder SetupIo(Action<IIoSetup> setup)
    {
        setup?.Invoke(_output);
        return this;
    }

    /// <summary>
    /// Setup the scripts which are executed in response to various events.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    public StoneFruitApplicationBuilder SetupEvents(Action<EngineEventCatalog> setup)
    {
        setup?.Invoke(_eventCatalog);
        return this;
    }

    /// <summary>
    /// Setup the settings for the engine.
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    public StoneFruitApplicationBuilder SetupSettings(Action<EngineSettings> setup)
    {
        setup?.Invoke(_settings);
        return this;
    }

    public StoneFruitApplication Build()
    {
        // Build up the final Engine registrations. This involves resolving some
        // dependencies which were setup previously
        BuildUp(Services);
        var provider = Services.BuildServiceProvider();
        return provider.GetRequiredService<StoneFruitApplication>();
    }

    private void BuildUp(IServiceCollection services)
    {
        SetupCoreEngineRegistrations(services);
        _handlers.BuildUp(services);
        services.AddSingleton(_eventCatalog);
        services.AddSingleton(_settings);
        _environments.BuildUp(services);
        _parsers.BuildUp(services);
        _output.BuildUp(services);
    }

    private static void SetupCoreEngineRegistrations(IServiceCollection services)
    {
        services.AddSingleton<EngineState>();
        services.AddSingleton<CommandDispatcher>();

        // Register the Engine and the things that the Engine provides. These registrations
        // are required, the user is not expected to inject their own Engine, State or
        // Dispatcher
        services.AddSingleton(_ => new StoneFruitApplicationAccessor());
        services.AddSingleton(provider =>
        {
            var e = new StoneFruitApplication(
                provider.GetRequiredService<IEnvironments>(),
                provider.GetRequiredService<ICommandParser>(),
                provider.GetRequiredService<IOutput>(),
                provider.GetRequiredService<IInput>(),
                provider.GetRequiredService<EngineState>(),
                provider.GetRequiredService<ICommandLine>(),
                provider.GetRequiredService<CommandDispatcher>()
            );
            var accessor = provider.GetRequiredService<StoneFruitApplicationAccessor>();
            accessor.Set(e);
            return e;
        });

        // The CurrentArguments only exist when the Engine.Run() or a variant is called, and an
        // input command has been entered. We access it from the EngineState, which comes from
        // the Engine, and we do all this to avoid circular references in the DI.
        services.AddTransient(provider => provider.GetRequiredService<EngineState>().CurrentArguments);
        services.AddTransient(provider => provider.GetRequiredService<EngineState>().HandlerContext);
    }
}
