using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution;
using StoneFruit.Utility;
using Unity;

namespace StoneFruit.Containers.Unity
{
    public class UnityHandlerSource : IHandlerSource
    {
        private readonly IUnityContainer _container;
        private readonly Lazy<IReadOnlyDictionary<string, Type>> _nameMap;
        private readonly ITypeVerbExtractor _verbExtractor;

        public UnityHandlerSource(IUnityContainer container, ITypeVerbExtractor verbExtractor)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _verbExtractor = verbExtractor ?? throw new ArgumentNullException(nameof(verbExtractor));
            _nameMap = new Lazy<IReadOnlyDictionary<string, Type>>(SetupNameMapping);
        }

        private IReadOnlyDictionary<string, Type> SetupNameMapping()
        {
            var commandTypes = _container.Registrations
                .Where(r => typeof(IHandlerBase).IsAssignableFrom(r.MappedToType))
                .Select(r => r.MappedToType ?? r.RegisteredType)
                .Where(t => t?.IsAbstract == false)
                .Distinct()
                .ToList();

            return commandTypes
                .SelectMany(commandType =>
                    _verbExtractor.GetVerbs(commandType).Select(verb => (verb, commandType))
                )
                .ToDictionaryUnique();
        }

        public IEnumerable<IVerbInfo> GetAll() => _nameMap.Value.Select(kvp => new VerbInfo(kvp.Key, kvp.Value));

        public IVerbInfo GetByName(string name)
            => _nameMap.Value.ContainsKey(name) ? new VerbInfo(name, _nameMap.Value[name]) : null;

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher)
        {
            var verb = command.Verb.ToLowerInvariant();
            var type = _nameMap.Value.ContainsKey(verb) ? _nameMap.Value[verb] : null;
            return type == null ? null : ResolveHandler(type);
        }

        private IHandlerBase ResolveHandler(Type type)
        {
            // TODO: Is .CreateChildContainer() sufficiently close to a Scope from modern Microsoft DI abstractions?
            using var scope = _container.CreateChildContainer();
            var instance = scope.Resolve(type);
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
            public string Group => _type.GetGroup();
            public bool ShouldShowInHelp => _type.ShouldShowInHelp(Verb);
        }
    }
}