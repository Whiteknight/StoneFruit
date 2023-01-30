using System.Collections.Generic;

namespace StoneFruit
{
    /// <summary>
    /// Sets up the environments mechanism.
    /// </summary>
    public interface IEnvironmentSetup
    {
        IEnvironmentSetup SetEnvironments(IReadOnlyList<string> names);

        /// <summary>
        /// Use a factory object to create environments on demand.
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        IEnvironmentSetup UseFactory<T>(IEnvironmentFactory<T> factory)
            where T : class;

        /// <summary>
        /// Use a single environment instance. Environments will not be switchable.
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        IEnvironmentSetup UseInstance<T>(T environment)
             where T : class;

        /// <summary>
        /// Do not use any environments (default).
        /// </summary>
        /// <returns></returns>
        IEnvironmentSetup None();
    }
}
