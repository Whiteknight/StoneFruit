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

        public IResult<IHandlerBase> GetInstance(IArguments arguments, CommandDispatcher dispatcher)
        {
            Assert.ArgumentNotNull(arguments, nameof(arguments));
            return _sources
                .Select(source => source.GetInstance(arguments, dispatcher))
                .FirstOrDefault(result => result.HasValue) ?? FailureResult<IHandlerBase>.Instance;
        }

        public IEnumerable<IVerbInfo> GetAll()
        {
            var allVerbs = _sources.SelectMany(s => s.GetAll())
                .GroupBy(info => info.Verb)
                .Select(g => g.First())
                .ToDictionary(v => v.Verb);
            return allVerbs.Values;
        }

        public IResult<IVerbInfo> GetByName(Verb verb)
            => _sources
                .Select(source => source.GetByName(verb))
                .FirstOrDefault(result => result.HasValue) ?? FailureResult<IVerbInfo>.Instance;
    }
}
