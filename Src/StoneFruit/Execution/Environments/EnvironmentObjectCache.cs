using System;
using System.Collections.Generic;

namespace StoneFruit.Execution.Environments;

// Object so we can cache the environment context objects
public sealed class EnvironmentObjectCache
{
    private readonly Dictionary<string, Dictionary<Type, object>> _cache;

    public EnvironmentObjectCache()
    {
        _cache = new Dictionary<string, Dictionary<Type, object>>();
    }

    public Maybe<T> Get<T>(string environment)
        where T : class
        => _cache.MaybeGetValue(environment)
            .Bind(env => env.MaybeGetValue(typeof(T)))
            .Bind(value => value is T typed ? new Maybe<T>(typed) : default);

    public void Set<T>(string environment, T value)
        where T : class
    {
        if (!_cache.ContainsKey(environment))
            _cache.Add(environment, new Dictionary<Type, object>());
        var env = _cache[environment];
        env[typeof(T)] = value;
    }

    public void Clear(string environment)
    {
        if (_cache.TryGetValue(environment, out var value))
            value.Clear();
    }
}
