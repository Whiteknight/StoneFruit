using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Environments;

public sealed record EnvironmentChangedObserver(Action<string> Action);

/// <summary>
/// Keeps track of the list of available environments including which one is the current
/// environment.
/// </summary>
public class EnvironmentCollection : IEnvironments
{
    public static IReadOnlyList<string> DefaultNamesList => [Constants.EnvironmentNameDefault];

    private readonly IReadOnlyDictionary<string, IEnvironment> _environments;
    private readonly EnvironmentObjectCache _cache;
    private readonly IReadOnlyList<EnvironmentChangedObserver> _observers;

    private Maybe<string> _currentName;

    public EnvironmentCollection(IReadOnlyList<string>? names, IEnumerable<EnvironmentChangedObserver> changeObservers)
    {
        var validNames = names == null || names.Count == 0 ? DefaultNamesList : names;
        _cache = [];
        _environments = validNames.ToDictionary(n => n, n => (IEnvironment)new Environment(n, _cache), StringComparer.OrdinalIgnoreCase);
        _currentName = default;
        _observers = changeObservers.OrEmptyIfNull().ToArray();
    }

    public Result<string, EnvironmentError> GetCurrentName()
        => _currentName.ToResult<EnvironmentError>(() => new NoEnvironmentSet());

    public Result<IEnvironment, EnvironmentError> GetCurrent()
        => GetCurrentName().Map(r => _environments[r]);

    public IReadOnlyList<string> GetNames() => _environments.Keys.ToList();

    public bool IsValid(string name)
        => name != null && _environments.ContainsKey(name);

    public Result<IEnvironment, EnvironmentError> SetCurrent(string name)
    {
        if (name == null)
            return GetCurrent();
        if (!_environments.ContainsKey(name))
            return new InvalidEnvironment(name);
        if (!_currentName.Is(name))
        {
            _currentName = name;
            NotifyChange(_currentName.GetValueOrThrow());
        }

        return GetCurrent();
    }

    public Result<IEnvironment, EnvironmentError> SetCurrent(int index)
    {
        var allEnvs = GetNames();
        if (index < 0 || index >= allEnvs.Count)
            return new InvalidEnvironment(index.ToString());
        SetCurrent(allEnvs[index]);
        return GetCurrent();
    }

    private void NotifyChange(string name)
    {
        foreach (var observer in _observers)
            observer.Action(name);
    }

    private sealed class Environment : IEnvironment
    {
        private readonly EnvironmentObjectCache _cache;

        public Environment(string name, EnvironmentObjectCache cache)
        {
            Name = name;
            _cache = cache;
        }

        public string Name { get; }

        public Maybe<T> GetCached<T>()
        => _cache.Get<T>(Name);

        public void CacheInstance<T>(T value)
        {
            _cache.Set(Name, value);
        }

        public void ClearCache()
        {
            _cache.Clear(Name);
        }
    }
}
