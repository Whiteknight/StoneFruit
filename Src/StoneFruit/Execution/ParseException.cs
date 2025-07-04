using System;

namespace StoneFruit.Execution;

/// <summary>
/// Exception thrown by the argument parser or script parser when the input is not in a valid
/// format.
/// </summary>
[Serializable]
public class ParseException : Exception
{
    public ParseException(string message) : base(message)
    {
    }
}
