using System;
using System.Collections.Generic;
using StoneFruit.Execution;

namespace StoneFruit
{
    /// <summary>
    /// Manages the list of available verbs
    /// </summary>
    public interface ICommandSource
    {
        ICommandVerb GetCommandInstance(CompleteCommand completeCommand, CommandDispatcher dispatcher);

        ICommandVerb GetCommandInstance<TCommand>(CompleteCommand completeCommand, CommandDispatcher dispatcher)
            where TCommand : class, ICommandVerb;
        IReadOnlyDictionary<string, Type> GetAll();
        Type GetCommandTypeByName(string name);
    }
}