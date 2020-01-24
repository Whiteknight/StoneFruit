using System.Threading.Tasks;

namespace StoneFruit
{
    public interface ICommandVerbBase
    {
    }

    /// <summary>
    /// Implements a verb
    /// </summary>
    public interface ICommandVerb : ICommandVerbBase
    {
        void Execute();
    }

    public interface ICommandVerbAsync : ICommandVerbBase
    {
        Task ExecuteAsync();
    }
}