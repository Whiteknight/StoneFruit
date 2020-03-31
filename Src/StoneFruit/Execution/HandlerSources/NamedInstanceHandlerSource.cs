using System.Collections.Generic;

namespace StoneFruit.Execution.HandlerSources
{
    /// <summary>
    /// Handler source for manually-specified handler instances
    /// </summary>
    public class NamedInstanceHandlerSource : IHandlerSource
    {
        // TODO: Unit tests
        private readonly Dictionary<string, VerbInfo> _verbs;

        public NamedInstanceHandlerSource()
        {
            _verbs = new Dictionary<string, VerbInfo>();
        }

        public void Add(string verb, IHandlerBase handlerObject, string description = null, string usage = null)
        {
            verb = verb.ToLowerInvariant();
            if (_verbs.ContainsKey(verb))
                throw new EngineBuildException("Cannot add verbs with duplicate names");
            var info = new VerbInfo(verb, handlerObject, description, usage);
            _verbs.Add(verb, info);
        }

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher) 
            => _verbs.ContainsKey(command.Verb) ? _verbs[command.Verb].HandlerObject : null;

        public IEnumerable<IVerbInfo> GetAll() => _verbs.Values;

        public IVerbInfo GetByName(string name) 
            => _verbs.ContainsKey(name) ? _verbs[name] : null;

        public int Count => _verbs.Count;

        private class VerbInfo : IVerbInfo
        {
            public VerbInfo(string verb, IHandlerBase handlerObject, string description, string help)
            {
                Verb = verb;
                HandlerObject = handlerObject;
                Description = description;
                Usage = help;
            }

            public IHandlerBase HandlerObject { get; }
            public string Verb { get; }
            public string Description { get; }
            public string Usage { get; }
            public bool ShouldShowInHelp => true;
        }
    }
}