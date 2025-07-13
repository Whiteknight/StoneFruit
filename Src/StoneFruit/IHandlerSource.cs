using System.Collections.Generic;

namespace StoneFruit;

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
    /// <param name="context"></param>
    /// <returns></returns>
    Maybe<IHandlerBase> GetInstance(HandlerContext context);

    /// <summary>
    /// Get metadata information about all verbs registered with this source.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IVerbInfo> GetAll();

    /// <summary>
    /// Get metadata information about a single verb. If the verb is not registered
    /// with this source, null is returned.
    /// </summary>
    /// <param name="verb"></param>
    /// <returns></returns>
    Maybe<IVerbInfo> GetByName(Verb verb);
}
