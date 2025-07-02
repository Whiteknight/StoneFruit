using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Environments;

/// <summary>
/// Environment factory which can return entries from a dictionary
/// </summary>
public class DictionaryEnvironmentFactory<T> : IEnvironmentFactory<T>
{
    private readonly IReadOnlyDictionary<string, T> _environments;

    public DictionaryEnvironmentFactory(IReadOnlyDictionary<string, T> environments)
    {
        _environments = environments;
        ValidEnvironments = _environments.Keys.ToList();
    }

    public IResult<T> Create(string name)
    {
        if (_environments.ContainsKey(name))
            return Result.Success(_environments[name]);
        return FailureResult<T>.Instance;
    }

    public IReadOnlyCollection<string> ValidEnvironments { get; }
}

public class InstanceEnvironmentFactory<T> : IEnvironmentFactory<T>
{
    private readonly T _instance;

    public InstanceEnvironmentFactory(T instance)
    {
        _instance = instance;
    }

    public IResult<T> Create(string name) => Result.Success(_instance);
}
