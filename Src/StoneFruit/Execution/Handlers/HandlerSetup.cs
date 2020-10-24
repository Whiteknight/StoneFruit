using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Scripts;
using StoneFruit.Handlers;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Setup handlers and handler sources
    /// </summary>
    public class HandlerSetup : IHandlerSetup, ISetupBuildable<IHandlers>
    {
        private readonly List<Func<HandlerSourceBuildContext, IHandlerSource>> _sourceFactories;
        private readonly DelegateHandlerSource _delegates;
        private readonly ScriptHandlerSource _scripts;
        private readonly NamedInstanceHandlerSource _instances;
        private IVerbExtractor _verbExtractor;

        public HandlerSetup()
        {
            _sourceFactories = new List<Func<HandlerSourceBuildContext, IHandlerSource>>();
            _delegates = new DelegateHandlerSource();
            _scripts = new ScriptHandlerSource();
            _instances = new NamedInstanceHandlerSource();
        }

        public void BuildUp(IServiceCollection services)
        {
            var verbExtractor = _verbExtractor ?? PriorityVerbExtractor.DefaultInstance;
            services.AddSingleton(verbExtractor);

            if (_delegates.Count > 0)
                services.AddSingleton<IHandlerSource>(_delegates);
            if (_scripts.Count > 0)
                services.AddSingleton<IHandlerSource>(_scripts);
            if (_instances.Count > 0)
                services.AddSingleton<IHandlerSource>(_instances);

            var buildContext = new HandlerSourceBuildContext(verbExtractor);
            _sourceFactories.Add(GetBuiltinHandlerSource);
            foreach (var sourceFactory in _sourceFactories)
            {
                var source = sourceFactory(buildContext);
                services.AddSingleton(source);
            }

            services.AddSingleton<IHandlers>(provider =>
            {
                var sources = provider.GetServices<IHandlerSource>();
                return new HandlerSourceCollection(sources);
            });
        }

        public IHandlers Build()
        {
            var sources = new List<IHandlerSource>();

            if (_delegates.Count > 0)
                sources.Add(_delegates);
            if (_scripts.Count > 0)
                sources.Add(_scripts);
            if (_instances.Count > 0)
                sources.Add(_instances);

            var verbExtractor = _verbExtractor ?? PriorityVerbExtractor.DefaultInstance;
            var buildContext = new HandlerSourceBuildContext(verbExtractor);
            _sourceFactories.Add(GetBuiltinHandlerSource);
            foreach (var sourceFactory in _sourceFactories)
            {
                var source = sourceFactory(buildContext);
                sources.Add(source);
            }

            return new HandlerSourceCollection(sources);
        }

        public IHandlerSetup UseVerbExtractor(IVerbExtractor verbExtractor)
        {
            _verbExtractor = verbExtractor;
            return this;
        }

        public IHandlerSetup AddSource(IHandlerSource source)
        {
            Assert.ArgumentNotNull(source, nameof(source));
            return AddSource(_ => source);
        }

        public IHandlerSetup AddSource(Func<HandlerSourceBuildContext, IHandlerSource> getSource)
        {
            Assert.ArgumentNotNull(getSource, nameof(getSource));
            _sourceFactories.Add(getSource);
            return this;
        }

        public IHandlerSetup Add(Verb verb, Action<IArguments, CommandDispatcher> handle, string description = null, string usage = null, string group = null)
        {
            Assert.ArgumentNotNull(verb, nameof(verb));
            Assert.ArgumentNotNull(handle, nameof(handle));
            _delegates.Add(verb, handle, description, usage, group);
            return this;
        }

        public IHandlerSetup Add(Verb verb, IHandlerBase handler, string description = null, string usage = null, string group = null)
        {
            Assert.ArgumentNotNull(verb, nameof(verb));
            Assert.ArgumentNotNull(handler, nameof(handler));
            _instances.Add(verb, handler, description, usage, group);
            return this;
        }

        public IHandlerSetup AddAsync(Verb verb, Func<IArguments, CommandDispatcher, Task> handleAsync, string description = null, string usage = null, string group = null)
        {
            Assert.ArgumentNotNull(verb, nameof(verb));
            Assert.ArgumentNotNull(handleAsync, nameof(handleAsync));
            _delegates.AddAsync(verb, handleAsync, description, usage, group);
            return this;
        }

        public IHandlerSetup AddScript(Verb verb, IEnumerable<string> lines, string description = null, string usage = null, string group = null)
        {
            Assert.ArgumentNotNull(verb, nameof(verb));
            Assert.ArgumentNotNull(lines, nameof(lines));
            _scripts.AddScript(verb, lines, description, usage, group);
            return this;
        }

        private static IHandlerSource GetBuiltinHandlerSource(HandlerSourceBuildContext context)
        {
            var requiredHandlers = new[]
            {
                typeof(EchoHandler),
                typeof(EnvironmentHandler),
                typeof(ExitHandler),
                typeof(HelpHandler),
                typeof(MetadataHandler),
            };
            return new TypeListConstructSource(requiredHandlers, null, context.VerbExtractor);
        }
    }
}