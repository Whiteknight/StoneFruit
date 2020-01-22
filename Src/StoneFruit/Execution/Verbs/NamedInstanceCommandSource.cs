using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Verbs
{
    public class NamedInstanceCommandSource : ICommandVerbSource
    {
        private readonly Dictionary<string, VerbInfo> _verbs;

        public NamedInstanceCommandSource()
        {
            _verbs = new Dictionary<string, VerbInfo>();
        }

        public void Add(string verb, ICommandVerb verbObject, string description = null, string help = null)
        {
            verb = verb.ToLowerInvariant();
            if (_verbs.ContainsKey(verb))
                throw new Exception("Cannot add verbs with duplicate names");
            var info = new VerbInfo(verb, verbObject, description, help);
            _verbs.Add(verb, info);
        }

        public ICommandVerb GetInstance(CompleteCommand completeCommand, CommandDispatcher dispatcher) 
            => _verbs.ContainsKey(completeCommand.Verb) ? _verbs[completeCommand.Verb].VerbObject : null;

        public ICommandVerb GetInstance<TCommand>(CompleteCommand completeCommand, CommandDispatcher dispatcher) 
            where TCommand : class, ICommandVerb 
            => _verbs.Values.OfType<TCommand>().FirstOrDefault();

        public IEnumerable<IVerbInfo> GetAll() => _verbs.Values;

        public IVerbInfo GetByName(string name) => _verbs.ContainsKey(name) ? _verbs[name] : null;

        private class VerbInfo : IVerbInfo
        {
            public VerbInfo(string verb, ICommandVerb verbObject, string description, string help)
            {
                Verb = verb;
                VerbObject = verbObject;
                Description = description;
                Help = help;
            }

            public ICommandVerb VerbObject { get; }
            public string Verb { get; }
            public string Description { get; }
            public string Help { get; }
            public bool ShouldShowInHelp => true;
        }
    }
}