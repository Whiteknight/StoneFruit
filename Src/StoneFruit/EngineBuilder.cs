using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Environments;
using StoneFruit.Execution.Handlers;
using StoneFruit.Execution.Output;

namespace StoneFruit
{
    /// <summary>
    /// Used to setup all options and dependencies of the Engine.
    /// </summary>
    public sealed class EngineBuilder : IEngineBuilder
    {
        private readonly HandlerSetup _handlers;
        private readonly OutputSetup _output;
        private readonly EngineEventCatalog _eventCatalog;
        private readonly EnvironmentSetup _environments;
        private readonly ParserSetup _parsers;
        private readonly EngineSettings _settings;
        private readonly IServiceCollection _services;

        private EngineBuilder(IServiceCollection? services = null, Action scanForHandlers = null)
        {
            _handlers = new HandlerSetup(scanForHandlers ?? ThrowIfScanRequestedButScannerNotProvided);
            _eventCatalog = new EngineEventCatalog();
            _output = new OutputSetup();
            _environments = new EnvironmentSetup();
            _parsers = new ParserSetup();
            _settings = new EngineSettings();
            _services = new ServiceCollection();
        }

        /// <summary>
        /// Setup verbs and their handlers
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public EngineBuilder SetupHandlers(Action<IHandlerSetup> setup)
        {
            setup?.Invoke(_handlers);
            return this;
        }

        /// <summary>
        /// Setup environments, if any
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public EngineBuilder SetupEnvironments(Action<IEnvironmentSetup> setup)
        {
            setup?.Invoke(_environments);
            return this;
        }

        /// <summary>
        /// Setup argument parsing and handling
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public EngineBuilder SetupArguments(Action<IParserSetup> setup)
        {
            setup?.Invoke(_parsers);
            return this;
        }

        /// <summary>
        /// Setup output
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public EngineBuilder SetupOutput(Action<IOutputSetup> setup)
        {
            setup?.Invoke(_output);
            return this;
        }

        /// <summary>
        /// Setup the scripts which are executed in response to various events
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public EngineBuilder SetupEvents(Action<EngineEventCatalog> setup)
        {
            setup?.Invoke(_eventCatalog);
            return this;
        }

        /// <summary>
        /// Setup the settings for the engine
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        public EngineBuilder SetupSettings(Action<EngineSettings> setup)
        {
            setup?.Invoke(_settings);
            return this;
        }

        /// <summary>
        /// Build the Engine using configured objects
        /// </summary>
        /// <returns></returns>
        public void BuildUp(IServiceCollection services)
        {
            _handlers.BuildUp(services);
            services.AddSingleton(_eventCatalog);
            services.AddSingleton(_settings);
            _environments.BuildUp(services);
            _parsers.BuildUp(services);
            _output.BuildUp(services);
        }

        /// <summary>
        /// Setup all dependencies and container registrations to create and execute the
        /// Engine, using the EngineBuilder.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="build"></param>
        public static void SetupEngineRegistrations(IServiceCollection services, Action<IEngineBuilder> build, Action scanForHandlers)
        {
            var builder = new EngineBuilder(scanForHandlers: scanForHandlers);
            build?.Invoke(builder);
            SetupEngineRegistrations(services);
            builder.BuildUp(services);
        }

        /// <summary>
        /// Setup type registrations necessary to create and execute the Engine
        /// </summary>
        /// <param name="services"></param>
        public static void SetupEngineRegistrations(IServiceCollection services)
        {
            // Register the Engine and the things that the Engine provides. These registrations
            // are required, the user is not expected to inject their own Engine, State or
            // Dispatcher
            services.AddSingleton(_ => new EngineAccessor());
            services.AddSingleton(provider =>
            {
                var accessor = provider.GetRequiredService<EngineAccessor>();
                var handlers = provider.GetRequiredService<IHandlers>();
                var environments = provider.GetRequiredService<IEnvironmentCollection>();
                var parser = provider.GetRequiredService<ICommandParser>();
                var output = provider.GetRequiredService<IOutput>();
                var engineCatalog = provider.GetRequiredService<EngineEventCatalog>();
                var engineSettings = provider.GetRequiredService<EngineSettings>();
                var e = new Engine(handlers, environments, parser, output, engineCatalog, engineSettings);
                accessor.SetEngine(e);
                return e;
            });

            // The Engine creates the EngineState and CommandDispatcher, so those things can be accessed
            // by getting the Engine. We use EngineAccessor here, because we will have circular references
            // with a few Engine dependencies otherwise. Circular references only need to be resolved
            // after the Engine.Run() or variants are called.
            services.AddSingleton(provider => provider.GetRequiredService<EngineAccessor>().Engine.State);
            services.AddSingleton(provider => provider.GetRequiredService<EngineAccessor>().Engine.Dispatcher);

            // The CurrentArguments only exist when the Engine.Run() or a variant is called, and an
            // input command has been entered. We access it from the EngineState, which comes from
            // the Engine, and we do all this to avoid circular references in the DI.
            services.AddScoped(provider => provider.GetRequiredService<EngineState>().CurrentArguments);
        }

        public static void SetupExplicitEnvironmentRegistration<TEnvironment>(IServiceCollection services)
            where TEnvironment : class
        {
            // The user may already have their own registration, so don't overwrite it.
            services.TryAddScoped(provider =>
            {
                var current = provider.GetRequiredService<IEnvironmentCollection>().GetCurrent();
                if (!current.HasValue)
                    throw new InvalidCastException($"Invalid environment. Expected environment {typeof(TEnvironment)} but none found.");
                var env = current.Value as TEnvironment;
                return env ?? throw new InvalidCastException($"Invalid cast. Expected environment {typeof(TEnvironment)} but found {current.GetType()}");
            });
        }

        public static Engine Build(Action<IEngineBuilder> build)
        {
            var services = new ServiceCollection();
            var handlersBuilder = new HandlerSetup(() => ScanForHandlers(services));
            var engineBuilder = new EngineBuilder(services: services);
            build?.Invoke(engineBuilder);
            SetupEngineRegistrations(services);
            handlersBuilder.AddSource(ctx => new ServiceProviderHandlerSource(services, () => services.BuildServiceProvider(), ctx.VerbExtractor));

            // Build up the final Engine registrations. This involves resolving some
            // dependencies which were setup previously
            engineBuilder.BuildUp(services);
            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<Engine>();
        }

        private void ThrowIfScanRequestedButScannerNotProvided()
        {
            throw new EngineBuildException(".Scan() requested but no scanner registered. Are you using a DI container?");
        }

        private static void ScanForHandlers(IServiceCollection services)
        {
            // Scan for handler classes in all assemblies, and setup a source to pull those types out of the
            // provider
            services.Scan(scanner => scanner
                .FromApplicationDependencies()
                .AddClasses(classes => classes.Where(t =>
                {
                    if (!typeof(IHandlerBase).IsAssignableFrom(t))
                        return false;
                    if (!t.IsPublic)
                        return false;
                    return true;
                }))
                .AsSelf()
                .WithTransientLifetime()
            );
        }
    }
}
