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
        private readonly IContainer _container;
        private readonly VerbTrie<Type> _handlers;

        public LamarHandlerSource(IServiceProvider provider, IVerbExtractor verbExtractor)
        {
            _container = provider as IContainer ?? throw new ArgumentException("Expected a Lamar Container", nameof(provider));
            _handlers = _container.Model.AllInstances
                .Where(i => typeof(IHandlerBase).IsAssignableFrom(i.ImplementationType))
                .Select(i => i.ImplementationType ?? i.ServiceType)
                .Where(t => t != null && t.IsClass && !t.IsAbstract)
                .Distinct()
                .SelectMany(commandType => verbExtractor
                    .GetVerbs(commandType)
                    .Select(verb => (Verb: verb, Type: commandType))
                )
                .ToVerbTrie(x => x.Verb, x => x.Type);
        }

        public IResult<IHandlerBase> GetInstance(IArguments arguments, CommandDispatcher dispatcher)
            => _handlers.Get(arguments).Transform(type => ResolveHandler(type));

        public IEnumerable<IVerbInfo> GetAll() => _handlers.GetAll().Select(kvp => new VerbInfo(kvp.Key, kvp.Value));

        public IResult<IVerbInfo> GetByName(Verb verb)
            => _handlers.Get(verb).Transform(type => (IVerbInfo)new VerbInfo(verb, type));

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
