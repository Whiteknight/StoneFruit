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
        private readonly EnvironmentSetup _environments;
        private IParser<char, IArgument> _argParser;

        public EngineBuilder()
        {
            _handlers = new HandlerSetup();
            _eventCatalog = new EngineEventCatalog();
            _output = new OutputSetup();
            _environments = new EnvironmentSetup();
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
            var environmentFactory = _environments.Build();
            var parser = _argParser == null ? CommandParser.GetDefault() : new CommandParser(_argParser);
            var output = _output.Build();
            return new Engine(commandSource, environmentFactory, parser, output, _eventCatalog);
        }

        private void EnsureArgumentParserNotSet()
        {
            if (_argParser != null)
                throw new Exception("Argument parser is already set for this builder. You cannot set a second argument parser.");
        }
    }
}