using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoneFruit.Execution.Scripts;

namespace StoneFruit.Execution.HandlerSources
{
    // TODO: Validate args
    public class HandlerSetup : IHandlerSetup
    {
        private readonly CombinedHandlerSource _sources;
        private readonly DelegateHandlerSource _delegates;
        private readonly ScriptHandlerSource _scripts;

        public HandlerSetup()
        {
            _sources = new CombinedHandlerSource();
            _delegates = new DelegateHandlerSource();
            _scripts = new ScriptHandlerSource();
        }

        public IHandlerSource Build()
        {
            if (_delegates.Count > 0)
                _sources.Add(_delegates);
            if (_scripts.Count > 0)
                _sources.Add(_scripts);
            return _sources.Simplify();
        }

        public IHandlerSetup AddSource(IHandlerSource source)
        {
            _sources.Add(source);
            return this;
        }

        public IHandlerSetup Add(string verb, Action<Command, CommandDispatcher> handle, string description = null, string usage = null)
        {
            _delegates.Add(verb, handle, description, usage);
            return this;
        }

        public IHandlerSetup AddAsync(string verb, Func<Command, CommandDispatcher, Task> handleAsync, string description = null, string usage = null)
        {
            _delegates.AddAsync(verb, handleAsync, description, usage);
            return this;
        }

        public IHandlerSetup AddScript(string verb, IEnumerable<string> lines, string description = null, string usage = null)
        {
            _scripts.AddScript(verb, lines, description, usage);
            return this;
        }
    }
}