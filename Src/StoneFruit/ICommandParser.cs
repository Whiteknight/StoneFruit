using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Scripts.Formatting;

namespace StoneFruit
{
    public interface ICommandParser
    {
        Command ParseCommand(string line);
        IArguments ParseArguments(string args);
        CommandFormat ParseScript(string script);
    }
}