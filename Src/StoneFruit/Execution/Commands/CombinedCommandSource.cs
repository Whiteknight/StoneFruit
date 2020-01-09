using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Commands
{
    public class CombinedCommandSource : ICommandSource
    {
        private readonly List<ICommandSource> _sources;

        public CombinedCommandSource()
        {
            _sources = new List<ICommandSource>();
        }

        public ICommandVerb GetCommandInstance(CompleteCommand command, IEnvironmentCollection environments, EngineState state, ITerminalOutput output)
        {
            foreach (var source in _sources)
            {
                var commandVerb = source.GetCommandInstance(command, environments, state, output);
                if (commandVerb != null)
                    return commandVerb;
            }

            return null;
        }

        public IReadOnlyDictionary<string, Type> GetAll()
        {
            var allPairs = _sources.SelectMany(s => s.GetAll());
            var dict = new Dictionary<string, Type>();
            foreach (var pair in allPairs)
            {
                if (!dict.ContainsKey(pair.Key))
                    dict.Add(pair.Key, pair.Value);
            }

            return dict;
        }

        public void Add(ICommandSource source)
        {
            if (source == null)
                return;
            _sources.Add(source);
        }

        public ICommandSource Simplify()
        {
            if (_sources.Count == 1)
                return _sources[0];
            return this;
        }

        public Type GetCommandTypeByName(string name)
        {
            foreach (var source in _sources)
            {
                var type = source.GetCommandTypeByName(name);
                if (type != null)
                    return type;
            }

            return null;
        }
    }
}
