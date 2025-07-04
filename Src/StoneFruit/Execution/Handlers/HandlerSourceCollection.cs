using System.Collections.Generic;
using System.Linq;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Combines multiple ICommandSource implementations together in priorty order.
/// </summary>
public class HandlerSourceCollection : IHandlers
{
    private readonly IReadOnlyList<IHandlerSource> _sources;

    public HandlerSourceCollection(IEnumerable<IHandlerSource> sources)
    {
        _sources = NotNull(sources)
            .Where(s => s != null)
            .ToList();
    }

    public Maybe<IHandlerBase> GetInstance(IArguments arguments, CommandDispatcher dispatcher)
    {
        NotNull(arguments);
        return _sources
            .Select(source => source.GetInstance(arguments, dispatcher))
            .FirstOrDefault(result => result.IsSuccess);
    }

    public IEnumerable<IVerbInfo> GetAll()
        => _sources.SelectMany(s => s.GetAll())
            .GroupBy(info => info.Verb)
            .Select(g => g.First())
            .ToDictionary(v => v.Verb)
            .Values;

    public Maybe<IVerbInfo> GetByName(Verb verb)
        => _sources
            .Select(source => source.GetByName(verb))
            .FirstOrDefault(result => result.IsSuccess);
}
