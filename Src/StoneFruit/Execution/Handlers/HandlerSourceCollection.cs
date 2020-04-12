using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    public class AliasMap
    {
        private readonly Dictionary<string, string> _aliases;

        public AliasMap()
        {
            _aliases = new Dictionary<string, string>();
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

        public Command Translate(Command command)
        {
            if (_aliases.ContainsKey(command.Verb))
            {
                var newVerb = _aliases[command.Verb];
                return command.Rename(newVerb);
            }

            return command;
        }
    }

    public interface IHandlers
    {
        IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher);
        IEnumerable<IVerbInfo> GetAll();
        IVerbInfo GetByName(string name);
    }

    /// <summary>
    /// Combines multiple ICommandSource implementations together in priorty order.
    /// </summary>
    public class HandlerSourceCollection : IHandlers
    {
        private readonly AliasMap _aliases;
        private readonly IReadOnlyList<IHandlerSource> _sources;
        

        public HandlerSourceCollection(IEnumerable<IHandlerSource> sources, AliasMap aliases)
        {
            _aliases = aliases;
            _sources = sources.Where(s => s != null).ToList();
        }

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher)
        {
            Assert.ArgumentNotNull(command, nameof(command));
            command = _aliases.Translate(command);
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

        public IVerbInfo GetByName(string name)
        {
            return _sources
                .Select(source => source.GetByName(name))
                .FirstOrDefault(info => info != null);
        }
    }
}
