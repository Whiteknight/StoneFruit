using System.Collections.Generic;
using StoneFruit.Execution;

namespace StoneFruit
{
    /// <summary>
    /// Manages the list of available verbs
    /// </summary>
    public interface ICommandHandlerSource
    {
        ICommandHandlerBase GetInstance(CompleteCommand completeCommand, CommandDispatcher dispatcher);

        ICommandHandlerBase GetInstance<TCommand>(CompleteCommand completeCommand, CommandDispatcher dispatcher)
            where TCommand : class, ICommandHandlerBase;

        IEnumerable<IVerbInfo> GetAll();

        IVerbInfo GetByName(string name);
    }

}