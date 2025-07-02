namespace StoneFruit.Execution.Arguments;

/// <summary>
/// A parsed argument which may represent a named argument or a combination of flag and
/// positional. This is a raw parsed argument, it won't be used in this form
/// </summary>
public class ParsedFlagPositionalOrNamedArgument : IParsedArgument
{
    public ParsedFlagPositionalOrNamedArgument(string name, string value)
    {
        Name = name.ToLowerInvariant();
        Value = value;
    }

    public string Name { get; }
    public string Value { get; }
}
