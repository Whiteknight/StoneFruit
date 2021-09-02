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
        private readonly VerbTrie<Type> _handlers;

        public UnityHandlerSource(IUnityContainer container, IVerbExtractor verbExtractor)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _handlers = _container.Registrations
                .Where(r => typeof(IHandlerBase).IsAssignableFrom(r.MappedToType))
                .Select(r => r.MappedToType ?? r.RegisteredType)
                .Where(t => t?.IsAbstract == false)
                .Distinct()
                .SelectMany(commandType =>
                    verbExtractor
                        .GetVerbs(commandType)
                        .Select(verb => (Verb: verb, Type: commandType))
                )
                .ToVerbTrie(x => x.Verb, x => x.Type);
        }

        public IHandlerBase GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        {
            var type = _handlers.Get(arguments);
            return type == null ? null : ResolveHandler(type);
        }

        public IEnumerable<IVerbInfo> GetAll() => _handlers.GetAll().Select(kvp => new VerbInfo(kvp.Key, kvp.Value));

        public IVerbInfo GetByName(Verb verb)
        {
            var type = _handlers.Get(verb);
            if (type == null)
                return null;
            return new VerbInfo(verb, type);
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
