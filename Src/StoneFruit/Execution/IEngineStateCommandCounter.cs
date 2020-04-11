namespace StoneFruit.Execution
{
    /// <summary>
    /// Counter for commands executed since last user input. Serves as protection against
    /// runaway recursion and infinite loops in the engine.
    /// </summary>
    public interface IEngineStateCommandCounter
    {
        void ReceiveUserInput();
        bool VerifyCanExecuteNextCommand(ICommandParser parser, IOutput output);
    }
}