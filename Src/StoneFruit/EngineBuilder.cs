using System;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Environments;
using StoneFruit.Execution.Handlers;
using StoneFruit.Execution.Output;

namespace StoneFruit
{
    public interface IEngineBuilder
    {
        /// <summary>
        /// Setup verbs and their handlers
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        EngineBuilder SetupHandlers(Action<IHandlerSetup> setup);

        /// <summary>
        /// Setup environments, if any
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        EngineBuilder SetupEnvironments(Action<IEnvironmentSetup> setup);

        /// <summary>
        /// Setup argument parsing and handling
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        EngineBuilder SetupArguments(Action<IArgumentParserSetup> setup);

        /// <summary>
        /// Setup output
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        EngineBuilder SetupOutput(Action<IOutputSetup> setup);

        /// <summary>
        /// Setup the scripts which are executed in response to various events
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        EngineBuilder SetupEvents(Action<EngineEventCatalog> setup);

        /// <summary>
        /// Setup the settings for the engine
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        EngineBuilder SetupSettings(Action<EngineSettings> setup);
    }

    /// <summary>
    /// Used to setup all options and dependencies of the Engine.
    /// </summary>
    public class EngineBuilder : IEngineBuilder
    {
        private readonly HandlerSetup _handlers;
        private readonly OutputSetup _output;
        private readonly EngineEventCatalog _eventCatalog;
        private readonly EnvironmentSetup _environments;
        private readonly ArgumentParserSetup _parsers;
        private readonly EngineSettings _settings;

        public EngineBuilder()
        {
            _handlers = new HandlerSetup();
            _eventCatalog = new EngineEventCatalog();
            _output = new OutputSetup();
            _environments = new EnvironmentSetup();
            _parsers = new ArgumentParserSetup();
            _settings = new EngineSettings();
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
        public EngineBuilder SetupArguments(Action<IArgumentParserSetup> setup)
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

        public static void SetupEngineRegistrations(IServiceCollection services, Action<EngineBuilder> build)
        {
            var builder = new EngineBuilder();
            build?.Invoke(builder);
            builder.BuildUp(services);

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
            services.AddTransient(provider => provider.GetService<EngineAccessor>().Engine.GetCurrentState());
            services.AddTransient(provider => provider.GetService<EngineAccessor>().Engine.GetCurrentDispatcher());

            // Add an additional handler source for critical built-in handlers so we don't
            // have to worry that the user forgot to register them
            services.AddSingleton<IHandlerSource>(HandlerSource.GetBuiltinHandlerSource());
        }
    }
}