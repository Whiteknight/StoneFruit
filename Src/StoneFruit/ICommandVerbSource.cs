using System.Collections.Generic;
using StoneFruit.Execution;

namespace StoneFruit
{
    /// <summary>
    /// Manages the list of available verbs
    /// </summary>
    public interface ICommandVerbSource
    {
        ICommandVerbBase GetInstance(CompleteCommand completeCommand, CommandDispatcher dispatcher);

        ICommandVerbBase GetInstance<TCommand>(CompleteCommand completeCommand, CommandDispatcher dispatcher)
            where TCommand : class, ICommandVerbBase;

        IEnumerable<IVerbInfo> GetAll();

        IVerbInfo GetByName(string name);
    }

}