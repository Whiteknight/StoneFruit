using System;

namespace StoneFruit.Execution.Exceptions;

public class WrappedException : Exception
{
    public WrappedException(Exception inner) : base("Received exception", inner)
    {
    }
}

