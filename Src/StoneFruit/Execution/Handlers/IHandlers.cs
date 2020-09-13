using System.Collections.Generic;

namespace StoneFruit.Execution.Handlers
{
    public interface IHandlers
    {
        IHandlerBase GetInstance(Command command, CommandDispatcher dispatcher);
        IEnumerable<IVerbInfo> GetAll();
        IVerbInfo GetByName(string name);
    }
}
