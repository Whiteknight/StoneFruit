namespace StoneFruit.Execution;

/// <summary>
/// Empty CommandCounter implementation for situations where counting is not necessary.
/// </summary>
public class NullCommandCounter : IEngineStateCommandCounter
{
    public void ReceiveUserInput()
    {
        // This counter does not count, so we don't need to do anything here
    }

    public bool VerifyCanExecuteNextCommand(ICommandParser parser, IOutput output) => true;
}
