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
        private readonly IReadOnlyDictionary<string, VerbInfo> _verbs;

        public MicrosoftRegisteredHandlerSource(IServiceCollection services, Func<IServiceProvider> getProvider, ITypeVerbExtractor verbExtractor)
        {
            _getProvider = getProvider;
            _verbs = SetupVerbMapping(services, verbExtractor);
        }

        private IReadOnlyDictionary<string, VerbInfo> SetupVerbMapping(IServiceCollection services, ITypeVerbExtractor verbExtractor)
        {
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
            return instances
                .Concat(types)
                .Concat(factories)
                .ToDictionaryUnique(vi => vi.Verb, vi => vi);
        }

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher)
        {
            if (!_verbs.ContainsKey(command.Verb))
                return null;
            var serviceType = _verbs[command.Verb].Type;
            using var scope = _getProvider().CreateScope();
            return scope.ServiceProvider.GetService(serviceType) as IHandlerBase;
        }

        public IEnumerable<IVerbInfo> GetAll() => _verbs.Values;

        public IVerbInfo GetByName(string name) => _verbs.ContainsKey(name) ? _verbs[name] : null;

        private class VerbInfo : IVerbInfo
        {
            public Type Type { get; }

            public VerbInfo(string verb, Type type)
            {
                Type = type;
                Verb = verb;
            }

            public string Verb { get; }
            public string Description => Type.GetDescription();
            public string Usage => Type.GetUsage();
            public bool ShouldShowInHelp => Type.ShouldShowInHelp(Verb);
        }
    }
}