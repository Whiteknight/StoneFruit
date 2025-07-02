namespace StoneFruit.Execution.Arguments;

/// <summary>
/// An argument defined by a name. This is a raw parsed argument, it won't be used in this form
/// </summary>
public class ParsedNamedArgument : IParsedArgument
{
    public ParsedNamedArgument(string name, string value)
    {
        Name = name.ToLowerInvariant();
        Value = value;
    }

    public string Name { get; }

    public string Value { get; }
}