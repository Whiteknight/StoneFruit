using System;
using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Commands;
using StoneFruit.Execution.Environments;

namespace StoneFruit
{
    public class EngineBuilder
    {
        private readonly CombinedCommandSource _commandSource;
        private IEnvironmentCollection _environments;
        private IParser<char, IEnumerable<IArgument>> _argParser;
        private ITerminalOutput _output;

        public EngineBuilder()
        {
            _commandSource = new CombinedCommandSource();
        }

        public EngineBuilder UseCommandSource(ICommandSource source)
        {
            _commandSource.Add(source);
            return this;
        }

        public EngineBuilder UseCommands(IEnumerable<Type> commandTypes)
        {
            _commandSource.Add(new ManualCommandSource(commandTypes ?? Enumerable.Empty<Type>()));
            return this;
        }

        public EngineBuilder UseCommands(params Type[] commandTypes)
        {
            _commandSource.Add(new ManualCommandSource(commandTypes ?? Enumerable.Empty<Type>()));
            return this;
        }

        public EngineBuilder UseEnvironmentFactory(IEnvironmentFactory factory)
        {
            EnsureEnvironmentsNotSet();
            _environments = factory == null ? null : new FactoryEnvironmentCollection(factory);
            return this;
        }

        public EngineBuilder UseSingleEnvironment(object environment)
        {
            EnsureEnvironmentsNotSet();
            _environments = new InstanceEnvironmentCollection(environment);
            return this;
        }

        public EngineBuilder NoEnvironment() => UseSingleEnvironment(null);

        public EngineBuilder UseArgumentParser(IParser<char, IEnumerable<IArgument>> argParser)
        {
            EnsureArgumentParserNotSet();
            _argParser = argParser;
            return this;
        }

        public EngineBuilder UseTerminalOutput(ITerminalOutput output)
        {
            // TODO: should we have some sort of tee/multiplex output?
            EnsureOutputNotSet();
            _output = output;
            return this;
        }

        public Engine Build()
        {
            var commandSource = _commandSource.Simplify();
            var environmentFactory = _environments;
            var argParser = _argParser ?? SimplifiedArgumentGrammar.GetParser();
            return new Engine(commandSource, environmentFactory, argParser, _output);
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