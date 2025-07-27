namespace StoneFruit.Execution;

/// <summary>
/// Empty CommandCounter implementation for situations where counting is not necessary.
/// </summary>
public class NullCommandCounter : IEngineStateCommandCounter
{
    public bool VerifyCanExecuteNextCommand() => true;
}
