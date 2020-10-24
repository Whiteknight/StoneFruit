using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Combines multiple ICommandSource implementations together in priorty order.
    /// </summary>
    public class HandlerSourceCollection : IHandlers
    {
        private readonly IReadOnlyList<IHandlerSource> _sources;

        public HandlerSourceCollection(IEnumerable<IHandlerSource> sources)
        {
            _sources = sources.Where(s => s != null).ToList();
        }

        public IHandlerBase GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        {
            Assert.ArgumentNotNull(arguments, nameof(arguments));
            foreach (var source in _sources)
            {
                var handler = source.GetInstance(arguments, dispatcher);
                if (handler != null)
                    return handler;
            }
            return null;
        }

        public IEnumerable<IVerbInfo> GetAll()
        {
            var allVerbs = _sources.SelectMany(s => s.GetAll())
                .GroupBy(info => info.Verb)
                .Select(g => g.First())
                .ToDictionary(v => v.Verb);
            return allVerbs.Values;
        }

        public IVerbInfo GetByName(Verb verb)
        {
            var byName = _sources
                .Select(source => source.GetByName(verb))
                .FirstOrDefault(info => info != null);
            if (byName != null)
                return byName;

            return null;
        }
    }
}
