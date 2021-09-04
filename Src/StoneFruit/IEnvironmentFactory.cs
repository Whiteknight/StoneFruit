using System.Collections.Generic;

namespace StoneFruit
{
    /// <summary>
    /// Provides a list of valid environments and creates environments by name.
    /// </summary>
    public interface IEnvironmentFactory
    {
        /// <summary>
        /// Create the environment object for the environment of the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IResult<object> Create(string name);

        /// <summary>
        /// Returns a list of valid environment names which can be passed to Create without
        /// causing a problem.
        /// </summary>
        IReadOnlyCollection<string> ValidEnvironments { get; }
    }
}
