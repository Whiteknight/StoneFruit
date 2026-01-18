using System;

namespace StoneFruit.Execution.Exceptions;

public class InternalException : Exception
{
    public InternalException(string message) : base(message)
    {
    }
}
