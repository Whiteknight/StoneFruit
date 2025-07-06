using System;

namespace StoneFruit.Execution;

/// <summary>
/// Exception we throw during Engine execution, including inside built-in handlers.
/// </summary>
public class ExecutionException : Exception
{
    public ExecutionException(string message)
        : base(message)
    {
    }
}
