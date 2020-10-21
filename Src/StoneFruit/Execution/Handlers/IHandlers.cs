using System.Collections.Generic;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Handlers
{
    public interface IHandlers
    {
        IHandlerBase GetInstance(IArguments arguments, CommandDispatcher dispatcher);
        IEnumerable<IVerbInfo> GetAll();
        IVerbInfo GetByName(Verb verb);
    }
}
