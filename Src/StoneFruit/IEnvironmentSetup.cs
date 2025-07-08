using System.Collections.Generic;

namespace StoneFruit;

/// <summary>
/// Sets up the environments mechanism.
/// </summary>
public interface IEnvironmentSetup
{
    /// <summary>
    /// Set a list of environments.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    IEnvironmentSetup SetEnvironments(IReadOnlyList<string> names);

    /// <summary>
    /// Add a named environment.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IEnvironmentSetup AddEnvironment(string name);
}
