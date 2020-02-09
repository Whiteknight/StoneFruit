using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution;
using StoneFruit.Utility;
using StructureMap;

namespace StoneFruit.StructureMap
{
    public class StructureMapHandlerSource : IHandlerSource
    {
        private readonly IContainer _container;
        private readonly IReadOnlyDictionary<string, Type> _nameMap;

        public StructureMapHandlerSource()
        {
            var container = new Container();
            container.Configure(c => c.ScanForCommandVerbs());
            //var scanned = container.WhatDidIScan();
            //var have = container.WhatDoIHave();
            _container = container;
            _nameMap = SetupNameMapping();
        }

        public StructureMapHandlerSource(IContainer container)
        {
            _container = container;
            _nameMap = SetupNameMapping();
        }

        private IReadOnlyDictionary<string, Type> SetupNameMapping()
        {
            var commandTypes = _container.Model.AllInstances
                .Where(i => typeof(IHandlerBase).IsAssignableFrom(i.PluginType))
                .Select(i => i.ReturnedType ?? i.PluginType)
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
            // TODO: Some of these .With() instances are long-lived and probably can be registered
            // into the container properly instead of treated transiently. I don't know if this is a
            // necessary optimization
            var context = _container
                // long-lived
                .With(dispatcher)
                .With(dispatcher.Environments)
                .With(dispatcher.State)
                .With(dispatcher.Output)
                .With(dispatcher.Parser)
                .With(dispatcher.Commands)

                // transient
                .With(Command)
                .With(Command.Arguments)
                ;
            if (dispatcher.Environments.Current != null)
                context = context.With(dispatcher.Environments.Current.GetType(), dispatcher.Environments.Current);

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