using System.Collections.Generic;

namespace StoneFruit.Execution.Handlers
{
    /// <summary>
    /// Represents all the handlers known by the system. Handlers may be of different types and
    /// come from a variety of different sources.
    /// </summary>
    public interface IHandlers
    {
        /// <summary>
        /// Given a list of arguments, find a handler with a matching verb. Returns null if no
        /// matching handler is found
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="dispatcher"></param>
        /// <returns></returns>
        IResult<IHandlerBase> GetInstance(IArguments arguments, CommandDispatcher dispatcher);

        /// <summary>
        /// Return help information for all handlers. Used mostly by the  "help" handler to display
        /// help information
        /// </summary>
        /// <returns></returns>
        IEnumerable<IVerbInfo> GetAll();

        /// <summary>
        /// Get detailed information about a specific handler by verb. Returns null if a handler
        /// is not registered with that verb.
        /// </summary>
        /// <param name="verb"></param>
        /// <returns></returns>
        IResult<IVerbInfo> GetByName(Verb verb);
    }
}
