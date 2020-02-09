using System.Collections.Generic;

namespace StoneFruit
{
    /// <summary>
    /// Manages the list of possible environments. Environments are cached after creation, each environment
    /// is only created once. Environments are ordered and can be accessed by name or number
    /// </summary>
    public interface IEnvironmentCollection
    {
        /// <summary>
        /// Gets a list of valid environment names and a relative position situations which
        /// want to refer to the environment by number
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<int, string> GetNames();

        /// <summary>
        /// Get the name of the environment at the given position
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        string GetName(int index);

        /// <summary>
        /// Sets the current environment by name
        /// </summary>
        /// <param name="name"></param>
        void SetCurrent(string name);

        /// <summary>
        /// Sets the current environment by position
        /// </summary>
        /// <param name="index"></param>
        void SetCurrent(int index);

        /// <summary>
        /// Returns the name of the current environment
        /// </summary>
        string CurrentName { get; }

        /// <summary>
        /// Returns true if this is a valid environment name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsValid(string name);

        /// <summary>
        /// Returns true if this is a valid environment position
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool IsValid(int index);

        /// <summary>
        /// Gets the environment object for the environment of the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object Get(string name);

        /// <summary>
        /// Gets the environment object for the environment at the given position
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        object Get(int idx);

        /// <summary>
        /// The current environment object, if any.
        /// </summary>
        object Current { get; }
    }
}