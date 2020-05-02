using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution;
using StoneFruit.Utility;
using StructureMap;

namespace StoneFruit.Containers.StructureMap
{
    /// <summary>
    /// Handler source to use the StructureMap DI Container to resolve instances of registered handler types.
    /// All handler types should be registered with the container prior to constructing an instance of this
    /// type.
    /// </summary>
    public class StructureMapHandlerSource : IHandlerSource
    {
        private readonly ITypeVerbExtractor _verbExtractor;
        private readonly IContainer _container;
        private readonly IReadOnlyDictionary<string, Type> _nameMap;

        public StructureMapHandlerSource(IServiceProvider serviceProvider, ITypeVerbExtractor verbExtractor)
        {
            Assert.ArgumentNotNull(serviceProvider, nameof(serviceProvider));

            _container = (serviceProvider as StructureMapServiceProvider)?.Container;
            if (_container == null)
                throw new ArgumentException("Expected StructureMap Container", nameof(serviceProvider));

            _verbExtractor = verbExtractor ?? TypeVerbExtractor.DefaultInstance;
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
                .SelectMany(commandType => 
                    _verbExtractor.GetVerbs(commandType)
                    .Select(verb => (verb, commandType))
                )
                .ToDictionaryUnique();
        }

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher)
        {
            var verb = command.Verb.ToLowerInvariant();
            var type = _nameMap.ContainsKey(verb) ? _nameMap[verb] : null;
            return type == null ? null : ResolveCommand(type);
        }

        public IHandlerBase GetInstance<TCommand>() 
            where TCommand : class, IHandlerBase
            => ResolveCommand(typeof(TCommand));

        public IEnumerable<IVerbInfo> GetAll() => _nameMap.Select(kvp => new VerbInfo(kvp.Key, kvp.Value));

        public IVerbInfo GetByName(string name)
            => _nameMap.ContainsKey(name) ? new VerbInfo(name, _nameMap[name]) : null;

        private IHandlerBase ResolveCommand(Type type) 
            => _container.GetInstance(type) as IHandlerBase;

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