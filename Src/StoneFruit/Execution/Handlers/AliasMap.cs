using System.Collections;
using System.Collections.Generic;

namespace StoneFruit.Execution.Handlers
{
    public class AliasMap : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> _aliases;
        private readonly Dictionary<string, IVerbInfo> _infos;

        public AliasMap()
        {
            _aliases = new Dictionary<string, string>();
            _infos = new Dictionary<string, IVerbInfo>();
        }

        public void AddAlias(string verb, string alias)
        {
            if (string.IsNullOrEmpty(alias) || string.IsNullOrEmpty(verb))
                return;
            alias = alias.ToLowerInvariant();
            if (_aliases.ContainsKey(alias))
                return;
            verb = verb.ToLowerInvariant();
            _aliases.Add(alias, verb);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => ((IEnumerable<KeyValuePair<string, string>>)_aliases).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_aliases).GetEnumerator();

        public Command Translate(Command command)
        {
            if (_aliases.ContainsKey(command.Verb))
            {
                var newVerb = _aliases[command.Verb];
                return command.Rename(newVerb);
            }

            return command;
        }

        public string GetVerb(string alias)
        {
            alias = alias.ToLowerInvariant();
            if (_aliases.ContainsKey(alias))
                return _aliases[alias];
            return null;
        }

        public IVerbInfo GetAliasInfoFromVerbInfo(string alias, string verb, IVerbInfo verbInfo)
        {
            if (_infos.ContainsKey(alias))
                return _infos[alias];

            // We have to set these up lazily, because aliases might be configured before the handlers are
            // configured. If using a DI container, the scanned list of handlers might not be available
            // until later. 
            var desc = string.IsNullOrEmpty(verbInfo.Description) ? $"Alias for {verb}" : verbInfo.Description;
            var usage = $"{alias} is an alias for {verb}";
            if (!string.IsNullOrEmpty(verbInfo.Usage))
                usage += "\n" + verbInfo.Usage;
            var info = new AliasVerbInfo(alias, desc, usage, verbInfo.Group, verbInfo.ShouldShowInHelp);
            _infos.Add(alias, info);
            return info;
        }

        private class AliasVerbInfo : IVerbInfo
        {
            public AliasVerbInfo(string verb, string description, string usage, string group, bool shouldShowInHelp)
            {
                Verb = verb;
                Description = description;
                Usage = usage;
                Group = group;
                ShouldShowInHelp = shouldShowInHelp;
            }

            public string Verb { get; }

            public string Description { get; }

            public string Usage { get; }

            public string Group { get; }

            public bool ShouldShowInHelp { get; }
        }
    }
}
