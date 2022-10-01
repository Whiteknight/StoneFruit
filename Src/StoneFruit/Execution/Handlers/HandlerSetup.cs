using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StoneFruit.Execution.Scripts;
using StoneFruit.Handlers;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Setup handlers and handler sources
    /// </summary>
    public class HandlerSetup : IHandlerSetup
    {
        private readonly List<Func<HandlerSourceBuildContext, IHandlerSource>> _sourceFactories;
        private readonly DelegateHandlerSource _delegates;
        private readonly ScriptHandlerSource _scripts;
        private readonly NamedInstanceHandlerSource _instances;
        private readonly Action _scanForHandlers;

        private readonly List<Type> _handlerTypes = new List<Type>
        {
            typeof(EchoHandler),
            typeof(EnvironmentHandler),
            typeof(ExitHandler),
            typeof(HelpHandler),
            typeof(MetadataHandler),
        };

        private IVerbExtractor _verbExtractor;
        private IHandlerMethodInvoker _methodInvoker;

        public HandlerSetup(Action scanForHandlers)
        {
            _sourceFactories = new List<Func<HandlerSourceBuildContext, IHandlerSource>>();
            _delegates = new DelegateHandlerSource();
            _scripts = new ScriptHandlerSource();
            _instances = new NamedInstanceHandlerSource();
            _verbExtractor = PriorityVerbExtractor.DefaultInstance;
            _methodInvoker = new DefaultHandlerMethodInvoker();
            _scanForHandlers = scanForHandlers ?? (() => { });
        }

        public void BuildUp(IServiceCollection services)
        {
            services.TryAddSingleton(_verbExtractor);
            services.TryAddSingleton(_methodInvoker);

            // Register these sources only if they have entries. We don't care about pre-existing
            // registrations, because IHandlerSource is expected to exist in multiples
            if (_delegates.Count > 0)
                services.AddSingleton<IHandlerSource>(_delegates);
            if (_scripts.Count > 0)
                services.AddSingleton<IHandlerSource>(_scripts);
            if (_instances.Count > 0)
                services.AddSingleton<IHandlerSource>(_instances);

            foreach (var handlerType in _handlerTypes)
            {
                services.AddTransient(handlerType);
            }
            services.AddSingleton<IHandlerSource>(provider =>
            {
                var ve = provider.GetRequiredService<IVerbExtractor>();
                var source = new TypeListConstructSource(_handlerTypes, (t, a, d) => provider.GetRequiredService(t), ve);
                return source;
            });

            foreach (var sourceFactory in _sourceFactories)
            {
                services.AddSingleton(provider =>
                {
                    var ve = provider.GetRequiredService<IVerbExtractor>();
                    var mi = provider.GetRequiredService<IHandlerMethodInvoker>();
                    var ctx = new HandlerSourceBuildContext(ve, mi);
                    return sourceFactory(ctx);
                });
            }

            // Add the IHandlers, which gets the list of all IHandlerSource instances from the DI
            // This one may be registered by the user already so don't overwrite
            services.TryAddSingleton<IHandlers>(provider =>
            {
                var sources = provider.GetServices<IHandlerSource>();
                return new HandlerSourceCollection(sources);
            });
        }

        public IHandlerSetup UseVerbExtractor(IVerbExtractor verbExtractor)
        {
            Assert.ArgumentNotNull(verbExtractor, nameof(verbExtractor));
            _verbExtractor = verbExtractor;
            return this;
        }

        public IHandlerSetup UseMethodInvoker(IHandlerMethodInvoker invoker)
        {
            Assert.ArgumentNotNull(invoker, nameof(invoker));
            _methodInvoker = invoker;
            return this;
        }

        public IHandlerSetup AddSource(Func<HandlerSourceBuildContext, IHandlerSource> getSource)
        {
            Assert.ArgumentNotNull(getSource, nameof(getSource));
            _sourceFactories.Add(getSource);
            return this;
        }

        public IHandlerSetup Add(Verb verb, Action<IArguments, CommandDispatcher> handle, string description = "", string usage = "", string group = "")
        {
            Assert.ArgumentNotNull(verb, nameof(verb));
            Assert.ArgumentNotNull(handle, nameof(handle));
            _delegates.Add(verb, handle, description, usage, group);
            return this;
        }

        public IHandlerSetup Add(Verb verb, IHandlerBase handler, string description = "", string usage = "", string group = "")
        {
            Assert.ArgumentNotNull(verb, nameof(verb));
            Assert.ArgumentNotNull(handler, nameof(handler));
            _instances.Add(verb, handler, description, usage, group);
            return this;
        }

        public IHandlerSetup AddAsync(Verb verb, Func<IArguments, CommandDispatcher, Task> handleAsync, string description = "", string usage = "", string group = "")
        {
            Assert.ArgumentNotNull(verb, nameof(verb));
            Assert.ArgumentNotNull(handleAsync, nameof(handleAsync));
            _delegates.AddAsync(verb, handleAsync, description, usage, group);
            return this;
        }

        public IHandlerSetup AddScript(Verb verb, IEnumerable<string> lines, string description = "", string usage = "", string group = "")
        {
            Assert.ArgumentNotNull(verb, nameof(verb));
            Assert.ArgumentNotNull(lines, nameof(lines));
            _scripts.AddScript(verb, lines, description, usage, group);
            return this;
        }

        public IHandlerSetup UseHandlerTypes(IEnumerable<Type> commandTypes)
        {
            Assert.ArgumentNotNull(commandTypes, nameof(commandTypes));

            _handlerTypes.AddRange(commandTypes);
            return this;
        }

        public IHandlerSetup Scan()
        {
            _scanForHandlers();
            return this;
        }
    }
}
