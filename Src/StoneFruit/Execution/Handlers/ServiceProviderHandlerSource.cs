using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Trie;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Handler source for the Microsoft DI container which finds types registered with the ServiceCollection
    /// and resolves them using the ServiceProvider. All handler types should be registered with the
    /// ServiceCollection before constructing an instance of this type.
    /// </summary>
    public class ServiceProviderHandlerSource : IHandlerSource
    {
        private readonly VerbTrie<VerbInfo> _verbs;
        private readonly IServiceProvider _provider;

        public ServiceProviderHandlerSource(IServiceCollection services, IServiceProvider provider, IVerbExtractor verbExtractor)
        {
            // TODO: Redesign. Instead of taking the IServiceCollection here and picking apart the registrations,
            // Wrap scanned handler types into a HandlerTypeWrapper and register those. Then we can inject the list
            // here and decode from there. We don't want to depend on the lifetype of IServiceCollection lasting
            // until this point.
            _provider = provider;

            var handlerRegistrations = services
                .Where(sd => typeof(IHandlerBase).IsAssignableFrom(sd.ServiceType))
                .ToList();

            var instances = handlerRegistrations
                .Where(sd => sd.ImplementationInstance != null)
                .SelectMany(sd =>
                    verbExtractor
                        .GetVerbs(sd.ImplementationInstance!.GetType())
                        .Select(verb => new VerbInfo(verb, sd.ImplementationInstance.GetType()))
                )
                .ToList();

            var types = handlerRegistrations
                .Where(sd => sd.ImplementationType != null)
                .SelectMany(sd =>
                    verbExtractor
                        .GetVerbs(sd.ImplementationType!)
                        .Select(verb => new VerbInfo(verb, sd.ImplementationType!))
                )
                .ToList();
            var factories = handlerRegistrations
                .Where(sd => sd.ImplementationFactory != null && sd.ServiceType.IsClass && !sd.ServiceType.IsAbstract)
                .SelectMany(sd =>
                    verbExtractor
                        .GetVerbs(sd.ServiceType)
                        .Select(verb => new VerbInfo(verb, sd.ServiceType))
                )
                .ToList();

            _verbs = instances
                .Concat(types)
                .Concat(factories)
                .ToVerbTrie(i => i.Verb);
        }

        public IResult<IHandlerBase> GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        {
            var serviceType = _verbs.Get(arguments);
            if (!serviceType.HasValue)
                return FailureResult<IHandlerBase>.Instance;

            using var scope = _provider.CreateScope();
            var created = scope.ServiceProvider.GetService(serviceType.Value.Type);
            return created is IHandlerBase instance ? new SuccessResult<IHandlerBase>(instance) : FailureResult<IHandlerBase>.Instance;
        }

        public IEnumerable<IVerbInfo> GetAll() => _verbs.GetAll().Select(kvp => kvp.Value);

        public IResult<IVerbInfo> GetByName(Verb verb) => _verbs.Get(verb).Transform(i => (IVerbInfo)i);

        private class VerbInfo : IVerbInfo
        {
            public Type Type { get; }

            public VerbInfo(Verb verb, Type type)
            {
                Type = type;
                Verb = verb;
            }

            public Verb Verb { get; }
            public string Description => Type.GetDescription();
            public string Usage => Type.GetUsage();
            public string Group => Type.GetGroup();
            public bool ShouldShowInHelp => Type.ShouldShowInHelp(Verb);
        }
    }
}
