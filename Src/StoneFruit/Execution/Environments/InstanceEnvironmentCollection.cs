using System.Collections.Generic;

namespace StoneFruit.Execution.Environments;

/// <summary>
/// An IEnvironmentCollection implementation for a single environment
/// </summary>
public class InstanceEnvironmentCollection : IEnvironmentCollection
{
    private readonly IResult<object> _current;
    private readonly IResult<string> _currentName;

    public InstanceEnvironmentCollection()
    {
        _current = Result.Success(new object());
        _currentName = Result.Success("");
    }

    public InstanceEnvironmentCollection(object environment)
    {
        _current = Result.Success(environment ?? new object());
        _currentName = Result.Success("");
    }

    public IReadOnlyList<string> GetNames() => new[] { Constants.EnvironmentNameDefault };

    public void SetCurrent(string name)
    {
        // This collection represents a singleton instance, there's nothing to set
    }

    public bool IsValid(string name) => name == Constants.EnvironmentNameDefault;

    public IResult<object> GetCurrent() => _current;

    public IResult<string> GetCurrentName() => _currentName;
}
