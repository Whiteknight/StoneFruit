using System;
using System.Collections;
using System.Collections.Generic;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution.Metadata;

/// <summary>
/// Metadata storage for the EngineState. Can be used to hold temporary data items between
/// command execution.
/// </summary>
public class EngineStateMetadataCache : IEnumerable<KeyValuePair<string, object>>
{
    private readonly Dictionary<string, object> _metadata;

    public EngineStateMetadataCache()
    {
        _metadata = [];
    }

    public EngineStateMetadataCache Add(string name, object value, bool allowOverwrite = true)
    {
        if (!_metadata.TryAdd(name, value) && allowOverwrite)
            _metadata[name] = value;
        return this;
    }

    public EngineStateMetadataCache Update<T>(string name, T start, Func<T, T> update)
    {
        NotNull(update);
        if (_metadata.TryGetValue(name, out var value) && value is T typed)
        {
            _metadata[name] = update(typed)!;
            return this;
        }

        return Add(name, update(start)!);
    }

    public EngineStateMetadataCache Remove(string name)
    {
        _metadata.Remove(name);
        return this;
    }

    public Maybe<object> Get(string name)
        => _metadata.TryGetValue(name, out object? value)
            ? new Maybe<object>(value)
            : default;

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _metadata.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _metadata.GetEnumerator();
}
