namespace StoneFruit.Execution;

/// <summary>
/// Counter for commands executed since last user input. Serves as protection against
/// runaway recursion and infinite loops in the engine.
/// </summary>
public interface IEngineStateCommandCounter
{
    /// <summary>
    /// Check if another command can be executed, and increments the internal count if it can
    /// be executed.
    /// </summary>
    /// <returns></returns>
    bool VerifyCanExecuteNextCommand();
}
