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
        private readonly IContainer _container;
        private readonly VerbTrie<Type> _types;

        public StructureMapHandlerSource(IServiceProvider serviceProvider, IVerbExtractor verbExtractor)
        {
            Assert.ArgumentNotNull(serviceProvider, nameof(serviceProvider));

            _container = (serviceProvider as StructureMapServiceProvider)?.Container;
            if (_container == null)
                throw new ArgumentException("Expected StructureMap Container", nameof(serviceProvider));

            _types = _container.Model.AllInstances
                .Where(i => typeof(IHandlerBase).IsAssignableFrom(i.PluginType))
                .Select(i => i.ReturnedType ?? i.PluginType)
                .SelectMany(commandType => verbExtractor
                    .GetVerbs(commandType)
                    .Select(verb => (Verb: verb, Type: commandType))
                )
                .ToVerbTrie(h => h.Verb, h => h.Type);
        }

        public IHandlerBase GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        {
            var type = _types.Get(arguments);
            return type == null ? null : ResolveHandler(type);
        }

        public IEnumerable<IVerbInfo> GetAll()
            => _types.GetAll().Select(kvp => new VerbInfo(kvp.Key, kvp.Value));

        public IVerbInfo GetByName(Verb verb)
        {
            var type = _types.Get(verb);
            if (type == null)
                return null;
            return new VerbInfo(verb, type);
        }

        private IHandlerBase ResolveHandler(Type type)
        {
            using var scope = _container.CreateChildContainer();
            return scope.GetInstance(type) as IHandlerBase;
        }

        private class VerbInfo : IVerbInfo
        {
            private readonly Type _type;

            public VerbInfo(Verb verb, Type type)
            {
                _type = type;
                Verb = verb;
            }

            public Verb Verb { get; }
            public string Description => _type.GetDescription();
            public string Usage => _type.GetUsage();
            public string Group => _type.GetGroup();
            public bool ShouldShowInHelp => _type.ShouldShowInHelp(Verb);
        }
    }
}