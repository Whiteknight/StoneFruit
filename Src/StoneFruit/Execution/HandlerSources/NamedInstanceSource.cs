using System;
using System.Collections.Generic;

namespace StoneFruit.Execution.HandlerSources
{
    public class NamedInstanceSource : IHandlerSource
    {
        // TODO: Unit tests
        private readonly Dictionary<string, VerbInfo> _verbs;

        public NamedInstanceSource()
        {
            _verbs = new Dictionary<string, VerbInfo>();
        }

        public void Add(string verb, IHandler handlerObject, string description = null, string help = null)
        {
            verb = verb.ToLowerInvariant();
            if (_verbs.ContainsKey(verb))
                throw new Exception("Cannot add verbs with duplicate names");
            var info = new VerbInfo(verb, handlerObject, description, help);
            _verbs.Add(verb, info);
        }

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher) 
            => _verbs.ContainsKey(command.Verb) ? _verbs[command.Verb].HandlerObject : null;

        public IEnumerable<IVerbInfo> GetAll() => _verbs.Values;

        public IVerbInfo GetByName(string name) => _verbs.ContainsKey(name) ? _verbs[name] : null;

        private class VerbInfo : IVerbInfo
        {
            public VerbInfo(string verb, IHandler handlerObject, string description, string help)
            {
                Verb = verb;
                HandlerObject = handlerObject;
                Description = description;
                Usage = help;
            }

            public IHandler HandlerObject { get; }
            public string Verb { get; }
            public string Description { get; }
            public string Usage { get; }
            public bool ShouldShowInHelp => true;
        }
    }
}