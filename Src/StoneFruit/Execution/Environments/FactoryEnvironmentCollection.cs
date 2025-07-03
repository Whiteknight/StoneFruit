using System;
using System.Collections.Generic;

namespace StoneFruit.Execution.Environments;

/// <summary>
/// An IEnvironmentCollection implementation which uses an IEnvironmentFactory to create environments
/// on demand.
/// </summary>
public class FactoryEnvironmentCollection : IEnvironmentCollection
{
    private readonly EnvironmentsList _nameList;

    private Maybe<string> _currentName;

    public FactoryEnvironmentCollection(EnvironmentsList environmentList)
    {
        _nameList = environmentList;
        _currentName = default;
    }

    public Maybe<string> GetCurrentName() => _currentName;

    public IReadOnlyList<string> GetNames() => _nameList.ValidNames;

    public bool IsValid(string name)
        => name != null && _nameList.Contains(name);

    public void SetCurrent(string name)
    {
        if (name == null)
            return;
        if (!_nameList.Contains(name))
            throw new InvalidOperationException($"Environment {name} does not exist");
        _currentName = name;
    }
}
