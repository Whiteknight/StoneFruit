using System.Collections.Generic;

namespace StoneFruit.Execution.Environments;

/// <summary>
/// An IEnvironmentCollection implementation for a single environment.
/// </summary>
public class InstanceEnvironmentCollection : IEnvironmentCollection
{
    private readonly Maybe<object> _current;
    private readonly Maybe<string> _currentName;

    public InstanceEnvironmentCollection()
    {
        _current = new object();
        _currentName = string.Empty;
    }

    public InstanceEnvironmentCollection(object environment)
    {
        _current = environment ?? new object();
        _currentName = string.Empty;
    }

    public IReadOnlyList<string> GetNames() => EnvironmentsList.DefaultNamesList;

    public void SetCurrent(string name)
    {
        // This collection represents a singleton instance, there's nothing to set
    }

    public bool IsValid(string name) => name == Constants.EnvironmentNameDefault;

    public Maybe<object> GetCurrent() => _current;

    public Maybe<string> GetCurrentName() => _currentName;
}
