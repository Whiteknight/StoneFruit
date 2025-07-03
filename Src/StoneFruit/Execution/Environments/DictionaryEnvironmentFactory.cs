using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Environments;

/// <summary>
/// Environment factory which can return entries from a dictionary.
/// </summary>
/// <typeparam name="T"></typeparam>
public class DictionaryEnvironmentFactory<T> : IEnvironmentFactory<T>
{
    private readonly IReadOnlyDictionary<string, T> _environments;

    public DictionaryEnvironmentFactory(IReadOnlyDictionary<string, T> environments)
    {
        _environments = environments;
        ValidEnvironments = _environments.Keys.ToList();
    }

    public Maybe<T> Create(string name)
        => _environments.ContainsKey(name)
            ? _environments[name]
            : default(Maybe<T>);

    public IReadOnlyCollection<string> ValidEnvironments { get; }
}

public class InstanceEnvironmentFactory<T> : IEnvironmentFactory<T>
{
    private readonly T _instance;

    public InstanceEnvironmentFactory(T instance)
    {
        _instance = instance;
    }

    public Maybe<T> Create(string name) => _instance;
}
