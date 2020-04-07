using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;

namespace StoneFruit.Execution.HandlerSources
{
    /// <summary>
    /// Combines multiple ICommandSource implementations together in priorty order.
    /// </summary>
    public class CombinedHandlerSource : IHandlerSource
    {
        private readonly List<IHandlerSource> _sources;
        private readonly Dictionary<string, string> _aliases;

        public CombinedHandlerSource()
        {
            _sources = new List<IHandlerSource>();
            _aliases = new Dictionary<string, string>();
        }

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher)
        {
            Assert.ArgumentNotNull(command, nameof(command));
            if (_aliases.ContainsKey(command.Verb))
            {
                var newVerb = _aliases[command.Verb];
                command = command.Rename(newVerb);
            }

            return _sources
                .Select(source => source.GetInstance(command, dispatcher))
                .FirstOrDefault(commandVerb => commandVerb != null);
        }

        public IEnumerable<IVerbInfo> GetAll()
        {
            return _sources.SelectMany(s => s.GetAll())
                .GroupBy(info => info.Verb)
                .Select(g => g.First());
        }

        public void Add(IHandlerSource source)
        {
            if (source != null)
                _sources.Add(source);
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

        public IHandlerSource Simplify() 
            => (_aliases.Count == 0 && _sources.Count == 1) ? _sources[0] : this;

        public IVerbInfo GetByName(string name)
        {
            return _sources
                .Select(source => source.GetByName(name))
                .FirstOrDefault(info => info != null);
        }
    }
}
