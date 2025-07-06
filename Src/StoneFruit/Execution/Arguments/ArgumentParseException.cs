using System;

namespace StoneFruit.Execution.Arguments;

/// <summary>
/// Exception thrown in response to errors encountered during argument access.
/// </summary>
public class ArgumentParseException : Exception
{
    public ArgumentParseException(string message)
        : base(message)
    {
    }

    public static ArgumentParseException MissingRequiredArgument(int index)
        => new ArgumentParseException($"Required argument at position {index} was not provided");

    public static ArgumentParseException MissingRequiredArgument(string argument)
        => new ArgumentParseException($"Required argument named '{argument}' was not provided");
}
