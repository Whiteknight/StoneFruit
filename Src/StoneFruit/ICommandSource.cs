using System.Collections.Generic;
using StoneFruit.Execution;
using StoneFruit.Execution.Commands;

namespace StoneFruit
{
    public interface ICommandSource
    {
        ICommandVerb GetCommandInstance(CompleteCommand command, IEnvironmentCollection environments, EngineState state, ITerminalOutput output);
        IEnumerable<CommandDescription> GetAll();
    }
}