using System.Threading;
using System.Threading.Tasks;

namespace StoneFruit
{
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
        void Execute();
    }

    /// <summary>
    /// An asynchronous handler. Handlers are invoked in response to specific verbs
    /// depending on registrations and mapping rules. Any dependencies of the handler are
    /// expected to be injected in the handler during creation
    /// </summary>
    public interface IAsyncHandler : IHandlerBase
    {
        Task ExecuteAsync(CancellationToken cancellation);
    }
}