using System;
using System.Collections.Generic;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution.Environments;

/// <summary>
/// An IEnvironmentCollection implementation which uses an IEnvironmentFactory to create environments
/// on demand.
/// </summary>
public class EnvironmentCollection : IEnvironmentCollection
{
    private readonly EnvironmentsList _nameList;
    private readonly EnvironmentObjectCache _cache;

    private Maybe<string> _currentName;

    public EnvironmentCollection(EnvironmentsList environmentList)
    {
        _nameList = NotNull(environmentList);
        _currentName = default;
        _cache = [];
    }

    public Maybe<string> GetCurrentName() => _currentName;

    public IReadOnlyList<string> GetNames() => _nameList.ValidNames;

    public bool IsValid(string name)
        => name != null && _nameList.Contains(name);

    public Maybe<T> GetCached<T>()
        => GetCurrentName().Bind(_cache.Get<T>);

    public void CacheInstance<T>(T value)
    {
        _cache.Set<T>(GetCurrentName().GetValueOrDefault(Constants.EnvironmentNameDefault), value);
    }

    public void ClearCache()
    {
        GetCurrentName().OnSuccess(_cache.Clear);
    }

    public void SetCurrent(string name)
    {
        if (name == null)
            return;
        if (!_nameList.Contains(name))
            throw new InvalidOperationException($"Environment {name} does not exist");
        _currentName = name;
    }

    public void SetCurrent(int index)
    {
        var allEnvs = GetNames();
        if (index < 0 || index >= allEnvs.Count)
            throw new InvalidOperationException($"Environment {index} is an invalid index. Valid numbers are 1..{allEnvs.Count}");
        SetCurrent(allEnvs[index]);
    }

    private sealed record CurrentEnvironment(string Value) : ICurrentEnvironment;

    public ICurrentEnvironment GetCurrent()
        => new CurrentEnvironment(GetCurrentName().GetValueOrDefault(Constants.EnvironmentNameDefault));
}
