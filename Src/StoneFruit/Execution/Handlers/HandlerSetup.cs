using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Scripts;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Setup handlers and handler sources
    /// </summary>
    public class HandlerSetup : IHandlerSetup
    {
        private readonly List<IHandlerSource> _sources;
        private readonly DelegateHandlerSource _delegates;
        private readonly ScriptHandlerSource _scripts;
        private readonly NamedInstanceHandlerSource _instances;
        private readonly AliasMap _aliases;

        public HandlerSetup()
        {
            _sources = new List<IHandlerSource>();
            _delegates = new DelegateHandlerSource();
            _scripts = new ScriptHandlerSource();
            _instances = new NamedInstanceHandlerSource();
            _aliases = new AliasMap();
        }

        public void BuildUp(IServiceCollection services)
        {
            services.AddSingleton(_aliases);
            if (_delegates.Count > 0)
                services.AddSingleton<IHandlerSource>(_delegates);
            if (_scripts.Count > 0)
                services.AddSingleton<IHandlerSource>(_scripts);
            if (_instances.Count > 0)
                services.AddSingleton<IHandlerSource>(_instances);
            foreach (var source in _sources)
                services.AddSingleton(source);
            services.AddSingleton<IHandlers>(provider =>
            {
                var sources = provider.GetServices<IHandlerSource>();
                var aliases = provider.GetService<AliasMap>();
                return new HandlerSourceCollection(sources, aliases);
            });
        }

        public IHandlerSetup AddSource(IHandlerSource source)
        {
            Assert.ArgumentNotNull(source, nameof(source));
            _sources.Add(source);
            return this;
        }

        public IHandlerSetup Add(string verb, Action<Command, CommandDispatcher> handle, string description = null, string usage = null)
        {
            Assert.ArgumentNotNullOrEmpty(verb, nameof(verb));
            Assert.ArgumentNotNull(handle, nameof(handle));
            _delegates.Add(verb, handle, description, usage);
            return this;
        }

        public IHandlerSetup AddAsync(string verb, Func<Command, CommandDispatcher, Task> handleAsync, string description = null, string usage = null)
        {
            Assert.ArgumentNotNullOrEmpty(verb, nameof(verb));
            Assert.ArgumentNotNull(handleAsync, nameof(handleAsync));
            _delegates.AddAsync(verb, handleAsync, description, usage);
            return this;
        }

        public IHandlerSetup Add(string verb, IHandlerBase handler, string description = null, string usage = null)
        {
            Assert.ArgumentNotNullOrEmpty(verb, nameof(verb));
            Assert.ArgumentNotNull(handler, nameof(handler));
            _instances.Add(verb, handler, description, usage);
            return this;
        }

        public IHandlerSetup AddScript(string verb, IEnumerable<string> lines, string description = null, string usage = null)
        {
            Assert.ArgumentNotNullOrEmpty(verb, nameof(verb));
            Assert.ArgumentNotNull(lines, nameof(lines));
            _scripts.AddScript(verb, lines, description, usage);
            return this;
        }

        public IHandlerSetup AddAlias(string verb, params string[] aliases)
        {
            Assert.ArgumentNotNullOrEmpty(verb, nameof(verb));
            Assert.ArgumentNotNull(aliases, nameof(aliases));
            foreach (var alias in aliases)
                _aliases.AddAlias(verb, alias);
            return this;
        }
    }
}