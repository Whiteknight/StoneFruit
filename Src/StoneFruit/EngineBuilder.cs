using System;
using System.Collections.Generic;
using ParserObjects;
using ParserObjects.Parsers;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
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
        private IEnvironmentCollection _environments;
        private IParser<char, IArgument> _argParser;

        public EngineBuilder()
        {
            _handlers = new HandlerSetup();
            _eventCatalog = new EngineEventCatalog();
            _output = new OutputSetup();
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
        /// Specify a factory for available environments, if the user should be able to
        /// select from multiple options
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public EngineBuilder UseEnvironmentFactory(IEnvironmentFactory factory)
        {
            EnsureEnvironmentsNotSet();
            _environments = factory == null ? null : new FactoryEnvironmentCollection(factory);
            return this;
        }

        /// <summary>
        /// Specify a single environment to use. An environment may represent configuration
        /// or execution-context information
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public EngineBuilder UseEnvironment(object environment)
        {
            EnsureEnvironmentsNotSet();
            _environments = new InstanceEnvironmentCollection(environment);
            return this;
        }

        /// <summary>
        /// Specify that the application does not use an environment
        /// </summary>
        /// <returns></returns>
        public EngineBuilder NoEnvironment() => UseEnvironment(null);

        /// <summary>
        /// Specify an argument parser to use
        /// </summary>
        /// <param name="argParser"></param>
        /// <returns></returns>
        public EngineBuilder UseArgumentParser(IParser<char, IArgument> argParser)
        {
            if (argParser == null)
            {
                _argParser = null;
                return this;
            }

            EnsureArgumentParserNotSet();
            _argParser = argParser;
            return this;
        }

        /// <summary>
        /// Specify an argument parser to use
        /// </summary>
        /// <param name="argParser"></param>
        /// <returns></returns>
        public EngineBuilder UseArgumentParser(IParser<char, IEnumerable<IArgument>> argParser)
        {
            var parser = argParser.Flatten<char, IEnumerable<IArgument>, IArgument>();
            return UseArgumentParser(parser);
        }

        public EngineBuilder UseSimplifiedArgumentParser()
            => UseArgumentParser(SimplifiedArgumentGrammar.GetParser());

        public EngineBuilder UsePosixStyleArgumentParser()
            => UseArgumentParser(PosixStyleArgumentGrammar.GetParser());

        public EngineBuilder UsePowershellStyleArgumentParser()
            => UseArgumentParser(PowershellStyleArgumentGrammar.GetParser());

        public EngineBuilder UseWindowsCmdArgumentParser()
            => UseArgumentParser(WindowsCmdArgumentGrammar.GetParser());

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
            var environmentFactory = _environments ?? new InstanceEnvironmentCollection(null);
            var parser = _argParser == null ? CommandParser.GetDefault() : new CommandParser(_argParser);
            var output = _output.Build();
            return new Engine(commandSource, environmentFactory, parser, output, _eventCatalog);
        }

        private void EnsureEnvironmentsNotSet()
        {
            if (_environments != null)
                throw new Exception("Environments are already configured for this builder. You cannot set environments again");
        }

        private void EnsureArgumentParserNotSet()
        {
            if (_argParser != null)
                throw new Exception("Argument parser is already set for this builder. You cannot set a second argument parser.");
        }
    }
}