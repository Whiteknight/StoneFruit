using System.Collections.Generic;
using StoneFruit.Utility;

namespace StoneFruit
{
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
        /// Sets the current environment by name.
        /// </summary>
        /// <param name="name"></param>
        void SetCurrent(string name);

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
        IResult<string> GetCurrentName();
    }

    public static class EnvironmentCollectionExtensions
    {
        /// <summary>
        /// Sets the current environment by position.
        /// </summary>
        /// <param name="environments"></param>
        /// <param name="index"></param>
        public static void SetCurrent(this IEnvironmentCollection environments, int index)
        {
            Assert.ArgumentNotNull(environments, nameof(environments));
            var allEnvs = environments.GetNames();
            if (index < 0 || index >= allEnvs.Count)
                return;
            environments.SetCurrent(allEnvs[index]);
        }

        /// <summary>
        /// Returns true if this is a valid environment position.
        /// </summary>
        /// <param name="environments"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool IsValid(this IEnvironmentCollection environments, int index)
        {
            Assert.ArgumentNotNull(environments, nameof(environments));
            var allEnvs = environments.GetNames();
            return index >= 0 && index < allEnvs.Count;
        }
    }
}
