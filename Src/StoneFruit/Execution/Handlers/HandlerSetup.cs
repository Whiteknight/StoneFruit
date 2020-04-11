using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly CombinedHandlerSource _sources;
        private readonly DelegateHandlerSource _delegates;
        private readonly ScriptHandlerSource _scripts;
        private readonly NamedInstanceHandlerSource _instances;

        public HandlerSetup()
        {
            _sources = new CombinedHandlerSource();
            _delegates = new DelegateHandlerSource();
            _scripts = new ScriptHandlerSource();
            _instances = new NamedInstanceHandlerSource();
        }

        public IHandlerSource Build()
        {
            if (_delegates.Count > 0)
                _sources.Add(_delegates);
            if (_scripts.Count > 0)
                _sources.Add(_scripts);
            if (_instances.Count > 0)
                _sources.Add(_instances);

            // Add some core handlers which are necessary for scripts. These might be already
            // included elsewhere, but we absolutely need to make sure we have them no
            // matter what.
            _sources.Add(new TypeListConstructSource(new[]
            {
                typeof(EchoHandler),
                typeof(EnvironmentChangeHandler),
                typeof(EnvironmentListHandler),
                typeof(ExitHandler),
                typeof(HelpHandler),
                typeof(MetadataRemoveHandler),
            }, null));
            return _sources.Simplify();
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
                _sources.AddAlias(verb, alias);
            return this;
        }
    }
}