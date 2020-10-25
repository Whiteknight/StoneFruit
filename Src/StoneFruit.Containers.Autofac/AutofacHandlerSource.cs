using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution;
using StoneFruit.Utility;

namespace StoneFruit.Containers.Autofac
{
    public class AutofacHandlerSource : IHandlerSource
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly VerbTrie<Type> _handlers;

        public AutofacHandlerSource(IServiceProvider provider, IVerbExtractor verbExtractor)
        {
            _serviceProvider = provider;
            _handlers = (provider as AutofacServiceProvider).LifetimeScope.ComponentRegistry.Registrations
                .Where(r => typeof(IHandlerBase).IsAssignableFrom(r.Activator.LimitType))
                .Select(r => r.Activator.LimitType)
                .Where(t => t != null && t.IsClass && !t.IsAbstract)
                .Distinct()
                .SelectMany(commandType => verbExtractor
                    .GetVerbs(commandType)
                    .Select(verb => (Verb: verb, Type: commandType))
                )
                .ToVerbTrie(x => x.Verb, x => x.Type);
        }

        public IEnumerable<IVerbInfo> GetAll() => _handlers.GetAll().Select(kvp => new VerbInfo(kvp.Key, kvp.Value));

        public IVerbInfo GetByName(Verb verb)
        {
            var type = _handlers.Get(verb);
            if (type == null)
                return null;
            return new VerbInfo(verb, type);
        }

        public IHandlerBase GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        {
            var type = _handlers.Get(arguments);
            return type == null ? null : ResolveHandler(type);
        }

        private IHandlerBase ResolveHandler(Type type)
        {
            using var scope = _serviceProvider.CreateScope();
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
