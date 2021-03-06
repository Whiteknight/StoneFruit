﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Extensions.ChildKernel;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using StoneFruit.Execution;
using StoneFruit.Utility;

namespace StoneFruit.Containers.Ninject
{
    /// <summary>
    /// Handler source for the Ninject DI container. Uses Ninject to resolve instances of handler types.
    /// All relevant types should be registered in the Kernel before constructing an instance of this type.
    /// </summary>
    public class NinjectHandlerSource : IHandlerSource
    {
        private readonly ITypeVerbExtractor _verbExtractor;
        private readonly IKernel _kernel;
        private readonly IReadOnlyDictionary<string, Type> _nameMap;

        public NinjectHandlerSource(IKernel kernel, ITypeVerbExtractor verbExtractor)
        {
            Assert.ArgumentNotNull(kernel, nameof(kernel));

            _kernel = kernel;
            _verbExtractor = verbExtractor ?? TypeVerbExtractor.DefaultInstance;
            _nameMap = SetupNameMapping();
        }

        private IReadOnlyDictionary<string, Type> SetupNameMapping()
        {
            var boundType = typeof(IHandlerBase);
            var commandTypes = _kernel.GetBindings(typeof(IHandlerBase))
                .Where(i => typeof(IHandlerBase).IsAssignableFrom(i.Service) && i.Target == BindingTarget.Type)
                .Select(binding =>
                {
                    var req = _kernel.CreateRequest(boundType, metadata => true, new IParameter[0], true, false);
                    var cache = _kernel.Components.Get<ICache>();
                    var planner = _kernel.Components.Get<IPlanner>();
                    var pipeline = _kernel.Components.Get<IPipeline>();
                    var provider = binding.GetProvider(new Context(_kernel, req, binding, cache, planner, pipeline));
                    return provider.Type;
                })
                .ToList();

            return commandTypes
                .OrEmptyIfNull()
                .SelectMany(commandType =>
                    _verbExtractor.GetVerbs(commandType)
                    .Select(verb => (verb, commandType))
                )
                .ToDictionaryUnique();
        }

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher)
        {
            var verb = command.Verb.ToLowerInvariant();
            var type = _nameMap.ContainsKey(verb) ? _nameMap[verb] : null;
            return type == null ? null : ResolveCommand(command, dispatcher, type);
        }

        public IHandlerBase GetInstance<TCommand>(Command command, CommandDispatcher dispatcher)
            where TCommand : class, IHandlerBase
            => ResolveCommand(command, dispatcher, typeof(TCommand));

        public IEnumerable<IVerbInfo> GetAll() => _nameMap.Select(kvp => new VerbInfo(kvp.Key, kvp.Value));

        public IVerbInfo GetByName(string name)
            => _nameMap.ContainsKey(name) ? new VerbInfo(name, _nameMap[name]) : null;

        private IHandlerBase ResolveCommand(Command Command, CommandDispatcher dispatcher, Type type)
        {
            var context = new ChildKernel(_kernel);

            // long-lived
            context.BindInstance(dispatcher);
            context.BindInstance(dispatcher.Environments);
            context.BindInstance(dispatcher.State);
            context.BindInstance(dispatcher.Output);
            context.BindInstance(dispatcher.Parser);
            context.BindInstance(dispatcher.Handlers);

            // transient
            context.BindInstance(Command);
            context.BindInstance(Command.Arguments);

            if (dispatcher.Environments.Current != null)
                context.Bind(dispatcher.Environments.Current.GetType()).ToConstant(dispatcher.Environments.Current);

            var instance = context.Get(type);
            context.Dispose();
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