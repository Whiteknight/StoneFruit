using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.VerbSources
{
    /// <summary>
    /// Combines multiple ICommandSource implementations together in priorty order.
    /// </summary>
    public class CombinedCommandVerbSource : ICommandVerbSource
    {
        private readonly List<ICommandVerbSource> _sources;

        public CombinedCommandVerbSource()
        {
            _sources = new List<ICommandVerbSource>();
        }

        public ICommandVerbBase GetInstance(CompleteCommand completeCommand, CommandDispatcher dispatcher)
        {
            return _sources
                .Select(source => source.GetInstance(completeCommand, dispatcher))
                .FirstOrDefault(commandVerb => commandVerb != null);
        }

        public ICommandVerbBase GetInstance<TCommand>(CompleteCommand completeCommand, CommandDispatcher dispatcher) 
            where TCommand : class, ICommandVerbBase
        {
            return _sources
                .Select(source => source.GetInstance<TCommand>(completeCommand, dispatcher))
                .FirstOrDefault(commandVerb => commandVerb != null);
        }

        public IEnumerable<IVerbInfo> GetAll()
        {
            return _sources.SelectMany(s => s.GetAll())
                .GroupBy(info => info.Verb)
                .Select(g => g.First());
        }

        public void Add(ICommandVerbSource source)
        {
            if (source == null)
                return;
            _sources.Add(source);
        }

        public ICommandVerbSource Simplify()
        {
            return _sources.Count == 1 ? _sources[0] : this;
        }

        public IVerbInfo GetByName(string name)
        {
            return _sources
                .Select(source => source.GetByName(name))
                .FirstOrDefault(info => info != null);
        }
    }
}
