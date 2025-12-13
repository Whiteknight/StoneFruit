using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;
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
        _sources = sources
            .OrEmptyIfNull()
            .Where(s => s != null)
            .ToList();
    }

    public Maybe<IHandlerBase> GetInstance(HandlerContext context)
    {
        NotNull(context);
        return _sources
            .Select(source => source.GetInstance(context))
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
