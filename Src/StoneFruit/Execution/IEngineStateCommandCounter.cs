namespace StoneFruit.Execution
{
    /// <summary>
    /// Counter for commands executed since last user input. Serves as protection against
    /// runaway recursion and infinite loops in the engine.
    /// </summary>
    public interface IEngineStateCommandCounter
    {
        /// <summary>
        /// Reset the counter in response to user input.
        /// </summary>
        void ReceiveUserInput();

        /// <summary>
        /// Check if another command can be executed, and increments the internal count if it can
        /// be executed.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        bool VerifyCanExecuteNextCommand(ICommandParser parser, IOutput output);
    }
}
