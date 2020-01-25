using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.HandlerSources
{
    /// <summary>
    /// Combines multiple ICommandSource implementations together in priorty order.
    /// </summary>
    public class CombinedCommandHandlerSource : ICommandHandlerSource
    {
        private readonly List<ICommandHandlerSource> _sources;

        public CombinedCommandHandlerSource()
        {
            _sources = new List<ICommandHandlerSource>();
        }

        public ICommandHandlerBase GetInstance(CompleteCommand completeCommand, CommandDispatcher dispatcher)
        {
            return _sources
                .Select(source => source.GetInstance(completeCommand, dispatcher))
                .FirstOrDefault(commandVerb => commandVerb != null);
        }

        public ICommandHandlerBase GetInstance<TCommand>(CompleteCommand completeCommand, CommandDispatcher dispatcher) 
            where TCommand : class, ICommandHandlerBase
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

        public void Add(ICommandHandlerSource source)
        {
            if (source == null)
                return;
            _sources.Add(source);
        }

        public ICommandHandlerSource Simplify()
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
