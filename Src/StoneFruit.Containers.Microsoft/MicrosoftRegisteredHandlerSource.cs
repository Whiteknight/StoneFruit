using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution;
using StoneFruit.Utility;

namespace StoneFruit.Containers.Microsoft
{
    /// <summary>
    /// Handler source for the Microsoft DI container which finds types registered with the ServiceCollection
    /// and resolves them using the ServiceProvider. All handler types should be registered with the
    /// ServiceCollection before constructing an instance of this type.
    /// </summary>
    public class MicrosoftRegisteredHandlerSource : IHandlerSource
    {
        private readonly Func<IServiceProvider> _getProvider;
        private readonly VerbTrie<VerbInfo> _verbs;

        public MicrosoftRegisteredHandlerSource(IServiceCollection services, Func<IServiceProvider> getProvider, IVerbExtractor verbExtractor)
        {
            _getProvider = getProvider;

            var handlerRegistrations = services.Where(sd => typeof(IHandlerBase).IsAssignableFrom(sd.ServiceType)).ToList();
            var instances = handlerRegistrations
                .Where(sd => sd.ImplementationInstance != null)
                .SelectMany(sd =>
                    verbExtractor
                        .GetVerbs(sd.ImplementationInstance.GetType())
                        .Select(verb => new VerbInfo(verb, sd.ImplementationInstance.GetType()))
                )
                .ToList();
            var types = handlerRegistrations
                .Where(sd => sd.ImplementationType != null)
                .SelectMany(sd =>
                    verbExtractor
                        .GetVerbs(sd.ImplementationType)
                        .Select(verb => new VerbInfo(verb, sd.ImplementationType))
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

        public IHandlerBase GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        {
            var serviceType = _verbs.Get(arguments);
            if (serviceType == null)
                return null;
            using var scope = _getProvider().CreateScope();
            return scope.ServiceProvider.GetService(serviceType.Type) as IHandlerBase;
        }

        public IEnumerable<IVerbInfo> GetAll() => _verbs.GetAll().Select(kvp => kvp.Value);

        public IVerbInfo GetByName(Verb verb) => _verbs.Get(verb);

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