using System;
using System.Threading.Tasks;
using StoneFruit.Execution;
using StoneFruit.Execution.HandlerSources;

namespace StoneFruit.Setup
{
    public class HandlerSetup : IHandlerSetup
    {
        private readonly CombinedHandlerSource _sources;
        private readonly DelegateHandlerSource _delegates;

        public HandlerSetup()
        {
            _sources = new CombinedHandlerSource();
            _delegates = new DelegateHandlerSource();
        }

        public IHandlerSource Build()
        {
            if (_delegates.Count > 0)
                _sources.Add(_delegates);
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
    }
}