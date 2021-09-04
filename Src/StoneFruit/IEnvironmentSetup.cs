using System.Collections.Generic;

namespace StoneFruit
{
    /// <summary>
    /// Sets up the environments mechanism.
    /// </summary>
    public interface IEnvironmentSetup
    {
        /// <summary>
        /// Use a factory object to create environments on demand.
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        IEnvironmentSetup UseFactory(IEnvironmentFactory factory);

        /// <summary>
        /// Use a single environment instance. Environments will not be switchable.
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        IEnvironmentSetup UseInstance(object environment);

        /// <summary>
        /// Use named environment instances which can be switched.
        /// </summary>
        /// <param name="environments"></param>
        /// <returns></returns>
        IEnvironmentSetup UseInstances(IReadOnlyDictionary<string, object> environments);

        /// <summary>
        /// Do not use any environments (default).
        /// </summary>
        /// <returns></returns>
        IEnvironmentSetup None();
    }
}
