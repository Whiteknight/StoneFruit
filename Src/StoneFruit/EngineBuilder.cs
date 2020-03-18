using System;
using StoneFruit.Execution;
using StoneFruit.Execution.Environments;
using StoneFruit.Execution.HandlerSources;
using StoneFruit.Execution.Output;

namespace StoneFruit
{
    public class EngineBuilder
    {
        private readonly HandlerSetup _handlers;
        private readonly OutputSetup _output;
        private readonly EngineEventCatalog _eventCatalog;
        private readonly EnvironmentSetup _environments;
        private readonly ParserSetup _parsers;

        public EngineBuilder()
        {
            _handlers = new HandlerSetup();
            _eventCatalog = new EngineEventCatalog();
            _output = new OutputSetup();
            _environments = new EnvironmentSetup();
            _parsers = new ParserSetup();
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

        public EngineBuilder SetupEnvironments(Action<IEnvironmentSetup> setup)
        {
            setup?.Invoke(_environments);
            return this;
        }

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
        /// Build the Engine using configured objects
        /// </summary>
        /// <returns></returns>
        public Engine Build()
        {
            var commandSource = _handlers.Build();
            var environmentFactory = _environments.Build();
            var parser = _parsers.Build();
            var output = _output.Build();
            return new Engine(commandSource, environmentFactory, parser, output, _eventCatalog);
        }
    }
}