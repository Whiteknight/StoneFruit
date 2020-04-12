using System;
using System.Collections.Generic;
using System.Linq;
using Lamar;
using StoneFruit.Execution;
using StoneFruit.Utility;

namespace StoneFruit.Containers.Lamar
{
    public class LamarHandlerSource<TEnvironment> : IHandlerSource
        where TEnvironment : class
    {
        private readonly ITypeVerbExtractor _verbExtractor;
        private readonly IContainer _container;
        private readonly Lazy<IReadOnlyDictionary<string, Type>> _nameMap;

        public LamarHandlerSource(IServiceProvider provider, ITypeVerbExtractor verbExtractor)
        {
            //var scanned = container.WhatDidIScan();
            //var have = container.WhatDoIHave();
            //_container = new Lazy<IContainer>(getContainer ?? GetDefaultContainer);
            _container = provider as IContainer;
            _verbExtractor = verbExtractor ?? TypeVerbExtractor.DefaultInstance;
            _nameMap = new Lazy<IReadOnlyDictionary<string, Type>>(SetupNameMapping);
        }

        private static IContainer GetDefaultContainer()
        {
            var registry = new ServiceRegistry();
            registry.Scan(s => s.ScanForHandlers());
            registry.SetupInjectableServices<TEnvironment>();
            return new Container(registry);
        }

        private IReadOnlyDictionary<string, Type> SetupNameMapping()
        {
            var commandTypes = _container.Model.AllInstances
                .Where(i => typeof(IHandlerBase).IsAssignableFrom(i.ImplementationType))
                .Select(i => i.ImplementationType)
                .ToList();

            return commandTypes
                .OrEmptyIfNull()
                .SelectMany(commandType =>
                    _verbExtractor.GetVerbs(commandType)
                    .Select(verb => (verb, commandType))
                )
                .ToDictionaryUnique();
        }

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher)
        {
            var verb = command.Verb.ToLowerInvariant();
            var type = _nameMap.Value.ContainsKey(verb) ? _nameMap.Value[verb] : null;
            return type == null ? null : ResolveCommand(command, dispatcher, type);
        }

        public IHandlerBase GetInstance<TCommand>(Command Command, CommandDispatcher dispatcher)
            where TCommand : class, IHandlerBase
            => ResolveCommand(Command, dispatcher, typeof(TCommand));

        public IEnumerable<IVerbInfo> GetAll() => _nameMap.Value.Select(kvp => new VerbInfo(kvp.Key, kvp.Value));

        public IVerbInfo GetByName(string name)
            => _nameMap.Value.ContainsKey(name) ? new VerbInfo(name, _nameMap.Value[name]) : null;

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