using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.HandlerSources
{
    /// <summary>
    /// Combines multiple ICommandSource implementations together in priorty order.
    /// </summary>
    public class CombinedHandlerSource : IHandlerSource
    {
        // TODO: More unit test coverage
        private readonly List<IHandlerSource> _sources;

        public CombinedHandlerSource()
        {
            _sources = new List<IHandlerSource>();
        }

        public IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher)
        {
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
            if (source == null)
                return;
            _sources.Add(source);
        }

        public IHandlerSource Simplify()
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
