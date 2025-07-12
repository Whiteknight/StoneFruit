namespace StoneFruit.Execution.Exceptions;

/// <summary>
/// Exception we throw during Engine execution, including inside built-in handlers.
/// </summary>
public class ExecutionException : InternalException
{
    public ExecutionException(string message)
        : base(message)
    {
    }
}
