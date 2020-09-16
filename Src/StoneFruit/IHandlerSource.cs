using System.Collections.Generic;
using StoneFruit.Execution;

namespace StoneFruit
{
    /// <summary>
    /// Manages a list of available handlers, instantiates handlers on request, and
    /// provides metadata about the registered verbs.
    /// </summary>
    public interface IHandlerSource
    {
        /// <summary>
        /// Instantiates the handler for the command. If a suitable handler is not found
        /// null is returned.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dispatcher"></param>
        /// <returns></returns>
        IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher);

        /// <summary>
        /// Get metadata information about all verbs registered with this source
        /// </summary>
        /// <returns></returns>
        IEnumerable<IVerbInfo> GetAll();

        /// <summary>
        /// Get metadata information about a single verb. If the verb is not registered
        /// with this source, null is returned
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IVerbInfo GetByName(string name);
    }
}