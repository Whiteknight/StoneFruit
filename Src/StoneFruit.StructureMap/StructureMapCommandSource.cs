using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution;
using StoneFruit.Utility;
using StructureMap;

namespace StoneFruit.StructureMap
{
    public class StructureMapCommandSource : ICommandVerbSource
    {
        private readonly IContainer _container;
        private readonly IReadOnlyDictionary<string, Type> _nameMap;

        public StructureMapCommandSource()
        {
            var container = new Container();
            container.Configure(c => c.ScanForCommandVerbs());
            _container = container;
            _nameMap = SetupNameMapping();
        }

        public StructureMapCommandSource(IContainer container)
        {
            _container = container;
            _nameMap = SetupNameMapping();
        }

        private IReadOnlyDictionary<string, Type> SetupNameMapping()
        {
            var commandTypes = _container.Model.AllInstances
                .Where(i => typeof(ICommandVerb).IsAssignableFrom(i.PluginType))
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

        public ICommandVerb GetInstance(CompleteCommand completeCommand, CommandDispatcher dispatcher)
        {
            var verb = completeCommand.Verb.ToLowerInvariant();
            var type = _nameMap.ContainsKey(verb) ? _nameMap[verb] : null;
            return type == null ? null : ResolveCommand(completeCommand, dispatcher, type);
        }

        public ICommandVerb GetInstance<TCommand>(CompleteCommand completeCommand, CommandDispatcher dispatcher) 
            where TCommand : class, ICommandVerb 
            => ResolveCommand(completeCommand, dispatcher, typeof(TCommand));

        public IEnumerable<IVerbInfo> GetAll() => _nameMap.Select(kvp => new VerbInfo(kvp.Key, kvp.Value));

        public IVerbInfo GetByName(string name)
            => _nameMap.ContainsKey(name) ? new VerbInfo(name, _nameMap[name]) : null;

        private ICommandVerb ResolveCommand(CompleteCommand completeCommand, CommandDispatcher dispatcher, Type type)
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

                // transient
                .With(completeCommand)
                .With(completeCommand.Arguments)
                .With(typeof(ICommandVerbSource), this);
            if (dispatcher.Environments.Current != null)
                context = context.With(dispatcher.Environments.Current.GetType(), dispatcher.Environments.Current);

            return context.GetInstance(type) as ICommandVerb;
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
            public string Help => _type.GetHelp();
            public bool ShouldShowInHelp => _type.ShouldShowInHelp(Verb);
        }
    }
}