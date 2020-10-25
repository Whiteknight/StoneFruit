using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using StoneFruit.Execution;
using StoneFruit.Utility;

namespace StoneFruit.Containers.Autofac
{
    public class AutofacHandlerSource : IHandlerSource
    {
        private readonly IContainer _container;
        private readonly VerbTrie<Type> _types;

        public AutofacHandlerSource(IServiceProvider provider, IVerbExtractor verbExtractor)
        {
            //_container = container;
            var handlerTypes = (provider as AutofacServiceProvider).LifetimeScope.ComponentRegistry.Registrations
                .Where(r => typeof(IHandlerBase).IsAssignableFrom(r.Activator.LimitType))
                .Select(r => r.Activator.LimitType)
                .ToList();

        }

        public IEnumerable<IVerbInfo> GetAll() => throw new NotImplementedException();
        public IVerbInfo GetByName(Verb verb) => throw new NotImplementedException();
        public IHandlerBase GetInstance(IArguments arguments, CommandDispatcher dispatcher) => throw new NotImplementedException();
    }
}
