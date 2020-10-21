using System;
using Microsoft.Extensions.DependencyInjection;
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
    public class EngineBuilder : IEngineBuilder
    {
        private readonly IHandlerSetup _handlers;
        private readonly IOutputSetup _output;
        private readonly EngineEventCatalog _eventCatalog;
        private readonly IEnvironmentSetup _environments;
        private readonly IParserSetup _parsers;
        private readonly EngineSettings _settings;

        public EngineBuilder(IHandlerSetup handlers = null, IOutputSetup output = null, IEnvironmentSetup environments = null, IParserSetup parsers = null, EngineEventCatalog events = null, EngineSettings settings = null)
        {
            _handlers = handlers ?? new HandlerSetup();
            _eventCatalog = events ?? new EngineEventCatalog();
            _output = output ?? new OutputSetup();
            _environments = environments ?? new EnvironmentSetup();
            _parsers = parsers ?? new ParserSetup();
            _settings = settings ?? new EngineSettings();
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
            (_handlers as ISetupBuildable<IHandlers>).BuildUp(services);
            services.AddSingleton(_eventCatalog);
            services.AddSingleton(_settings);
            (_environments as ISetupBuildable<IEnvironmentCollection>).BuildUp(services);
            (_parsers as ISetupBuildable<ICommandParser>).BuildUp(services);
            (_output as ISetupBuildable<IOutput>).BuildUp(services);
        }

        /// <summary>
        /// Setup all dependencies and container registrations to create and execute the
        /// Engine, using the EngineBuilder.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="build"></param>
        public static void SetupEngineRegistrations(IServiceCollection services, Action<EngineBuilder> build)
        {
            var builder = new EngineBuilder();
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
            // Register the Engine and the things that the Engine provides
            services.AddSingleton(provider => new EngineAccessor());
            services.AddSingleton(provider =>
            {
                var accessor = provider.GetService<EngineAccessor>();
                var handlers = provider.GetService<IHandlers>();
                var environments = provider.GetService<IEnvironmentCollection>();
                var parser = provider.GetService<ICommandParser>();
                var output = provider.GetService<IOutput>();
                var engineCatalog = provider.GetService<EngineEventCatalog>();
                var engineSettings = provider.GetService<EngineSettings>();
                var e = new Engine(handlers, environments, parser, output, engineCatalog, engineSettings);
                accessor.SetEngine(e);
                return e;
            });
            services.AddScoped(provider => provider.GetService<EngineAccessor>().Engine.GetCurrentState());
            services.AddScoped(provider => provider.GetService<EngineAccessor>().Engine.GetCurrentDispatcher());
            services.AddScoped(provider => provider.GetService<EngineState>().CurrentArguments);
        }

        public static void SetupExplicitEnvironmentRegistration<TEnvironment>(IServiceCollection services)
            where TEnvironment : class
        {
            services.AddScoped(provider => provider.GetService<IEnvironmentCollection>().Current as TEnvironment);
        }

        public Engine Build()
        {
            var handlers = (_handlers as ISetupBuildable<IHandlers>).Build();
            var environments = (_environments as ISetupBuildable<IEnvironmentCollection>).Build();
            var parser = (_parsers as ISetupBuildable<ICommandParser>).Build();
            var output = (_output as ISetupBuildable<IOutput>).Build();

            return new Engine(handlers, environments, parser, output, _eventCatalog, _settings);
        }
    }
}