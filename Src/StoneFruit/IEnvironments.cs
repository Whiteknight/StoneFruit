using System.Collections.Generic;
using StoneFruit.Execution.Environments;

namespace StoneFruit;

public interface IEnvironment
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
public interface IEnvironments
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
    Result<string, EnvironmentError> GetCurrentName();

    /// <summary>
    /// Get the current environment. Returns a failure result if a valid environment is not set.
    /// </summary>
    /// <returns></returns>
    Result<IEnvironment, EnvironmentError> GetCurrent();

    /// <summary>
    /// Set the current environment by name. Returns the new current environment on success.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Result<IEnvironment, EnvironmentError> SetCurrent(string name);

    /// <summary>
    /// Set the current environment by index, according to the list order from GetNames(). Returns
    /// the new current environment on success.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    Result<IEnvironment, EnvironmentError> SetCurrent(int index);
}
