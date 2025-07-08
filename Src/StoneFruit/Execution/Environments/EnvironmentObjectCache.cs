using System;
using System.Collections.Generic;

namespace StoneFruit.Execution.Environments;

// Object so we can cache the environment context objects
public sealed class EnvironmentObjectCache : Dictionary<string, Dictionary<Type, object>>
{
    public Maybe<T> Get<T>(string environment)
        => this.MaybeGetValue(environment)
            .Bind(env => env.MaybeGetValue(typeof(T)))
            .Bind(value => value is T typed ? new Maybe<T>(typed) : default);

    public void Set<T>(string environment, T value)
    {
        if (value is null)
            return;
        if (!ContainsKey(environment))
            Add(environment, []);
        this[environment][typeof(T)] = value;
    }

    public void Clear(string environment)
    {
        if (TryGetValue(environment, out var value))
            value.Clear();
    }
}
