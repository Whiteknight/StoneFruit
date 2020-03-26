using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoneFruit.Execution.Scripts;
using StoneFruit.Utility;

namespace StoneFruit.Execution.HandlerSources
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
    }
}