using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution
{
    public interface IEngineStateCommandCounter
    {
        void ReceiveUserInput();
        bool VerifyCanExecuteNextCommand(CommandParser parser, IOutput output);
    }
}