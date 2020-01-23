using System;
using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Environments;
using StoneFruit.Execution.VerbSources;

namespace StoneFruit
{
    public class EngineBuilder
    {
        private readonly CombinedCommandSource _commandSource;
        private readonly EngineEventCatalog _eventCatalog;
        private IEnvironmentCollection _environments;
        private IParser<char, CommandArguments> _argParser;
        private ITerminalOutput _output;

        public EngineBuilder()
        {
            _commandSource = new CombinedCommandSource();
            _eventCatalog = new EngineEventCatalog();
        }

        /// <summary>
        /// Specify an ICommandSource instance to use to find and instantiate ICommandVerb
        /// instances
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public EngineBuilder UseCommandSource(ICommandVerbSource source)
        {
            _commandSource.Add(source);
            return this;
        }

        /// <summary>
        /// Specify a list of Types of ICommandVerb classes to use
        /// </summary>
        /// <param name="commandTypes"></param>
        /// <returns></returns>
        public EngineBuilder UseCommandTypes(IEnumerable<Type> commandTypes)
        {
            _commandSource.Add(new TypeListConstructCommandSource(commandTypes ?? Enumerable.Empty<Type>()));
            return this;
        }

        /// <summary>
        /// Specify a list of Types of ICommandVebr classes to use
        /// </summary>
        /// <param name="commandTypes"></param>
        /// <returns></returns>
        public EngineBuilder UseCommandType(params Type[] commandTypes)
        {
            _commandSource.Add(new TypeListConstructCommandSource(commandTypes ?? Enumerable.Empty<Type>()));
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
        public EngineBuilder UseSingleEnvironment(object environment)
        {
            EnsureEnvironmentsNotSet();
            _environments = new InstanceEnvironmentCollection(environment);
            return this;
        }

        /// <summary>
        /// Specify that the application does not use an environment
        /// </summary>
        /// <returns></returns>
        public EngineBuilder NoEnvironment() => UseSingleEnvironment(null);

        /// <summary>
        /// Specify an argument parser to use
        /// </summary>
        /// <param name="argParser"></param>
        /// <returns></returns>
        public EngineBuilder UseArgumentParser(IParser<char, IArgument> argParser)
        {
            var parser = CommandArgumentsGrammar.GetParser(argParser);
            return UseArgumentParser(parser);
        }

        /// <summary>
        /// Specify an argument parser to use
        /// </summary>
        /// <param name="argParser"></param>
        /// <returns></returns>
        public EngineBuilder UseArgumentParser(IParser<char, IEnumerable<IArgument>> argParser)
        {
            var parser = CommandArgumentsGrammar.GetParser(argParser);
            return UseArgumentParser(parser);
        }

        /// <summary>
        /// Specify an argument parser to use
        /// </summary>
        /// <param name="argParser"></param>
        /// <returns></returns>
        public EngineBuilder UseArgumentParser(IParser<char, CommandArguments> argParser)
        {
            EnsureArgumentParserNotSet();
            _argParser = argParser;
            return this;
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
        /// Specify the object to use for user I/O and interaction
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public EngineBuilder UseTerminalOutput(ITerminalOutput output)
        {
            // TODO: should we have some sort of tee/multiplex output?
            EnsureOutputNotSet();
            _output = output;
            return this;
        }

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
            var commandSource = _commandSource.Simplify();
            var environmentFactory = _environments;
            var parser = new CommandParser(_argParser);
            return new Engine(commandSource, environmentFactory, parser, _output, _eventCatalog);
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

        private void EnsureOutputNotSet()
        {
            if (_output != null)
                throw new Exception("Terminal Output already setup for this builder. You cannot set a second terminal output.");
        }
    }
}