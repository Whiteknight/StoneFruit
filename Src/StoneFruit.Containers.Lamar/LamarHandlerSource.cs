using System;
using System.Collections.Generic;
using System.Linq;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution;
using StoneFruit.Utility;

namespace StoneFruit.Containers.Lamar
{
    /// <summary>
    /// Lamar Container-based handler source. Available handlers are read from the container and handlers
    /// are resolved using the container. All handlers should be registered with the container prior to
    /// constructing an instance of this type.
    /// </summary>
    public class LamarHandlerSource : IHandlerSource
    {
        // TODO: V2 should be able to handle registrations made AFTER .SetupEngine()
        private readonly IContainer _container;
        private readonly VerbTrie<Type> _handlers;

        public LamarHandlerSource(IServiceProvider provider, IVerbExtractor verbExtractor)
        {
            _container = provider as IContainer ?? throw new ArgumentException("Expected a Lamar Container", nameof(provider));
            _handlers = new VerbTrie<Type>();
            var commandTypes = _container.Model.AllInstances
                .Where(i => typeof(IHandlerBase).IsAssignableFrom(i.ImplementationType))
                .Select(i => i.ImplementationType ?? i.ServiceType)
                .Where(t => t != null && t.IsClass && !t.IsAbstract)
                .Distinct()
                .ToList();

            var verbAndTypes = commandTypes
                .OrEmptyIfNull()
                .SelectMany(commandType =>
                    verbExtractor.GetVerbs(commandType)
                    .Select(verb => (Verb: verb, Type: commandType))
                );
            foreach (var verbAndType in verbAndTypes)
                _handlers.Insert(verbAndType.Verb, verbAndType.Type);
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
            using var scope = _container.CreateScope();
            var instance = scope.ServiceProvider.GetService(type);
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