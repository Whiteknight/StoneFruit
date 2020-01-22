using System.Collections.Generic;
using StoneFruit.Execution;

namespace StoneFruit
{
    /// <summary>
    /// Manages the list of available verbs
    /// </summary>
    public interface ICommandVerbSource
    {
        ICommandVerb GetInstance(CompleteCommand completeCommand, CommandDispatcher dispatcher);

        ICommandVerb GetInstance<TCommand>(CompleteCommand completeCommand, CommandDispatcher dispatcher)
            where TCommand : class, ICommandVerb;

        IEnumerable<IVerbInfo> GetAll();

        IVerbInfo GetByName(string name);
    }
}