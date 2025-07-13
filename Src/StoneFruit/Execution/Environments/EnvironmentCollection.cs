using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution.Environments;

/// <summary>
/// An IEnvironmentCollection implementation which uses an IEnvironmentFactory to create environments
/// on demand.
/// </summary>
public class EnvironmentCollection : IEnvironmentCollection
{
    public static IReadOnlyList<string> DefaultNamesList => [Constants.EnvironmentNameDefault];
    private readonly IReadOnlyDictionary<string, IEnvironment> _environments;
    private readonly EnvironmentObjectCache _cache;

    private Maybe<string> _currentName;

    public EnvironmentCollection(IReadOnlyList<string>? names)
    {
        var validNames = names == null || names.Count == 0 ? DefaultNamesList : names;
        _cache = [];
        _environments = validNames.ToDictionary(n => n, n => (IEnvironment)new Environment(n, _cache), StringComparer.OrdinalIgnoreCase);
        _currentName = default;
    }

    public Maybe<string> GetCurrentName() => _currentName;

    public Maybe<IEnvironment> GetCurrent()
        => GetCurrentName().Bind(_environments.MaybeGetValue);

    public IReadOnlyList<string> GetNames() => _environments.Keys.ToList();

    public bool IsValid(string name)
        => name != null && _environments.ContainsKey(name);

    public void SetCurrent(string name)
    {
        if (name == null)
            return;
        if (!_environments.ContainsKey(name))
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
