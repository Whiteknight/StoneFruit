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
            _environments = factory == null ? null : new FactoryEnvironmentCollection(factory);
            return this;
        }

        public EngineBuilder UseSingleEnvironment(object environment)
        {
            _environments = new InstanceEnvironmentCollection(null);
            return this;
        }

        public EngineBuilder NoEnvironment()
        {
            return UseSingleEnvironment(null);
        }

        public EngineBuilder UseArgumentParser(IParser<char, IEnumerable<IArgument>> argParser)
        {
            _argParser = argParser;
            return this;
        }

        public Engine Build()
        {
            var commandSource = _commandSource.Simplify();
            var environmentFactory = _environments;
            var argParser = _argParser ?? SimplifiedArgumentGrammar.GetParser();
            return new Engine(commandSource, environmentFactory, argParser);
        }
    }
}