namespace StoneFruit;

/// <summary>
/// Provides a list of valid environments and creates environments by name.
/// </summary>
public interface IEnvironmentFactory<T>
{
    /// <summary>
    /// Create the environment object for the environment of the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Maybe<T> Create(string name);
}
