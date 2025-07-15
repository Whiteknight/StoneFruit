using StoneFruit.Execution.Exceptions;

namespace StoneFruit.Execution.Arguments;

/// <summary>
/// Exception thrown in response to errors encountered during argument access.
/// </summary>
public class ArgumentParseException : InternalException
{
    public ArgumentParseException(string message)
        : base(message)
    {
    }
}
