using System.Collections.Generic;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

public interface ICurrentEnvironment
{
    string Name { get; }
    Maybe<T> GetCached<T>();

    void CacheInstance<T>(T value);

    void ClearCache();
}

/// <summary>
/// Manages the list of possible environments. Environments are cached after creation, each environment
/// is only created once. Environments are ordered and can be accessed by name or number.
/// </summary>
public interface IEnvironmentCollection
{
    /// <summary>
    /// Gets a list of valid environment names.
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<string> GetNames();

    /// <summary>
    /// Returns true if this is a valid environment name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool IsValid(string name);

    /// <summary>
    /// Get the name of the current environment. Returns a failure result if a valid environment
    /// is not set.
    /// </summary>
    /// <returns></returns>
    Maybe<string> GetCurrentName();

    Maybe<ICurrentEnvironment> GetCurrent();
}

public static class EnvironmentCollectionExtensions
{
    /// <summary>
    /// Returns true if this is a valid environment position.
    /// </summary>
    /// <param name="environments"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static bool IsValid(this IEnvironmentCollection environments, int index)
    {
        var allEnvs = NotNull(environments).GetNames();
        return index >= 0 && index < allEnvs.Count;
    }
}
