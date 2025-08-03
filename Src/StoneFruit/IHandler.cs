using System.Threading;
using System.Threading.Tasks;

namespace StoneFruit;

/// <summary>
/// Base type for handler objects.
/// </summary>
public interface IHandlerBase
{
    static virtual string Description { get; } = string.Empty;
    static virtual string Usage { get; } = string.Empty;
    static virtual string Group { get; } = string.Empty;
}

/// <summary>
/// A handler. Handlers are invoked in response to specific verbs depending on
/// registrations and mapping rules. Any dependencies of the handler are expected to
/// be injected in the handler during creation.
/// </summary>
public interface IHandler : IHandlerBase
{
    /// <summary>
    /// Execute the handler and perform any work.
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="context"></param>
    void Execute(IArguments arguments, HandlerContext context);
}

/// <summary>
/// An asynchronous handler. Handlers are invoked in response to specific verbs
/// depending on registrations and mapping rules. Any dependencies of the handler are
/// expected to be injected in the handler during creation.
/// </summary>
public interface IAsyncHandler : IHandlerBase
{
    /// <summary>
    /// Execute the handler asynchronously and perform any work.
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="context"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    Task ExecuteAsync(IArguments arguments, HandlerContext context, CancellationToken cancellation);
}
