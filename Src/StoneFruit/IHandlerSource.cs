using System.Collections.Generic;
using StoneFruit.Execution;

namespace StoneFruit
{
    /// <summary>
    /// Manages the list of available verbs
    /// </summary>
    public interface IHandlerSource
    {
        IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher);

        IHandlerBase GetInstance<TCommand>(Command command, CommandDispatcher dispatcher)
            where TCommand : class, IHandlerBase;

        IEnumerable<IVerbInfo> GetAll();

        IVerbInfo GetByName(string name);
    }
}