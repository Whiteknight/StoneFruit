using System.Collections;
using System.Collections.Generic;

namespace StoneFruit.Execution;

/// <summary>
/// Metadata storage for the EngineState. Can be used to hold temporary data items between
/// command execution.
/// </summary>
public class EngineStateMetadataCache : IEnumerable<KeyValuePair<string, object>>
{
    private readonly Dictionary<string, object> _metadata;

    public EngineStateMetadataCache()
    {
        _metadata = new Dictionary<string, object>();
    }

    public void Add(string name, object value, bool allowOverwrite = true)
    {
        if (_metadata.TryAdd(name, value))
            return;

        if (!allowOverwrite)
            return;

        _metadata.Remove(name);
        _metadata.Add(name, value);
    }

    public Maybe<object> Get(string name)
        => _metadata.TryGetValue(name, out object? value) ? new Maybe<object>(value) : default;

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _metadata.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _metadata.GetEnumerator();

    public void Remove(string name)
    {
        _metadata.Remove(name);
    }
}
