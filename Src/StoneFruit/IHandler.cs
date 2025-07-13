using System.Threading;
using System.Threading.Tasks;

namespace StoneFruit;

/// <summary>
/// Base type for handler objects.
/// </summary>
public interface IHandlerBase
{
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
    void Execute();
}

public interface IHandlerWithContext : IHandlerBase
{
    void Execute(HandlerContext context);
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
    /// <param name="cancellation"></param>
    /// <returns></returns>
    Task ExecuteAsync(CancellationToken cancellation);
}

public interface IAsyncHandlerWithContext : IHandlerBase
{
    Task ExecuteAsync(HandlerContext context, CancellationToken cancellation);
}
