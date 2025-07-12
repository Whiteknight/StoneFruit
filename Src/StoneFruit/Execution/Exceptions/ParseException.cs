namespace StoneFruit.Execution.Exceptions;

/// <summary>
/// Exception thrown by the argument parser or script parser when the input is not in a valid
/// format.
/// </summary>
public class ParseException : InternalException
{
    public ParseException(string message) : base(message)
    {
    }
}
