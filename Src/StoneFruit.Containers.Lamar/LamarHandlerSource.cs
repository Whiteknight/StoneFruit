using System;
using System.Collections.Generic;
using System.Linq;
using Lamar;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Utility;

namespace StoneFruit.Containers.Lamar
{
    public class LamarHandlerSource<TEnvironment> : IHandlerSource
        where TEnvironment : class
    {
        private readonly IContainer _container;
        private readonly IReadOnlyDictionary<string, Type> _nameMap;

        public LamarHandlerSource()
        {
            var registry = new ServiceRegistry();
            registry.Scan(s => s.ScanForCommandVerbs());
            registry.Injectable<CommandDispatcher>();
            registry.Injectable<IEnvironmentCollection>();
            registry.Injectable<EngineState>();
            registry.Injectable<IOutput>();
            registry.Injectable<CommandParser>();
            registry.Injectable<IHandlerSource>();
            registry.Injectable<Command>();
            registry.Injectable<IArguments>();
            if (typeof(TEnvironment) != typeof(object))
                registry.Injectable<TEnvironment>();

            var container = new Container(registry);
            //var scanned = container.WhatDidIScan();
            //var have = container.WhatDoIHave();
            _container = container;
            _nameMap = SetupNameMapping();
        }

        public LamarHandlerSource(IContainer container)
        {
            _container = container;
            _nameMap = SetupNameMapping();
        }

        private IReadOnlyDictionary<string, Type> SetupNameMapping()
        {
            var commandTypes = _container.Model.AllInstances
                .Where(i => typeof(IHandlerBase).IsAssignableFrom(i.ImplementationType))
                .Select(i => i.ImplementationType)
                .ToList();

            return commandTypes
                .OrEmptyIfNull()
                .SelectMany(commandType => commandType
                    .GetVerbs()
                    .Select(verb => (verb, commandType))
                )
                .ToDictionaryUnique();
        }

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher)
        {
            var verb = command.Verb.ToLowerInvariant();
            var type = _nameMap.ContainsKey(verb) ? _nameMap[verb] : null;
            return type == null ? null : ResolveCommand(command, dispatcher, type);
        }

        public IHandlerBase GetInstance<TCommand>(Command Command, CommandDispatcher dispatcher)
            where TCommand : class, IHandlerBase
            => ResolveCommand(Command, dispatcher, typeof(TCommand));

        public IEnumerable<IVerbInfo> GetAll() => _nameMap.Select(kvp => new VerbInfo(kvp.Key, kvp.Value));

        public IVerbInfo GetByName(string name)
            => _nameMap.ContainsKey(name) ? new VerbInfo(name, _nameMap[name]) : null;

        private IHandlerBase ResolveCommand(Command Command, CommandDispatcher dispatcher, Type type)
        {
            var context = _container.GetNestedContainer();

            // long-lived
            context.Inject(dispatcher);
            context.Inject(dispatcher.Environments);
            context.Inject(dispatcher.State);
            context.Inject(dispatcher.Output);
            context.Inject(dispatcher.Parser);
            context.Inject(dispatcher.Commands);

            // transient
            context.Inject(Command);
            context.Inject(Command.Arguments);

            if (dispatcher.Environments.Current != null)
                context.Inject(dispatcher.Environments.Current.GetType(), dispatcher.Environments.Current, true);

            var instance = context.GetInstance(type);
            return instance as IHandlerBase;
        }

        private class VerbInfo : IVerbInfo
        {
            private readonly Type _type;

            public VerbInfo(string verb, Type type)
            {
                _type = type;
                Verb = verb;
            }

            public string Verb { get; }
            public string Description => _type.GetDescription();
            public string Usage => _type.GetUsage();
            public bool ShouldShowInHelp => _type.ShouldShowInHelp(Verb);
        }
    }
}