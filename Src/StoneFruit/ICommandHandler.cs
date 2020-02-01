using System.Threading;
using System.Threading.Tasks;

namespace StoneFruit
{
    public interface ICommandHandlerBase
    {
    }

    /// <summary>
    /// Implements a verb
    /// </summary>
    public interface ICommandHandler : ICommandHandlerBase
    {
        void Execute();
    }

    public interface ICommandHandlerAsync : ICommandHandlerBase
    {
        Task ExecuteAsync(CancellationToken cancellation);
    }
}