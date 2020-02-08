using System.Threading;
using System.Threading.Tasks;

namespace StoneFruit
{
    public interface IHandlerBase
    {
    }

    /// <summary>
    /// Implements a verb
    /// </summary>
    public interface IHandler : IHandlerBase
    {
        void Execute();
    }

    public interface IAsyncHandler : IHandlerBase
    {
        Task ExecuteAsync(CancellationToken cancellation);
    }
}