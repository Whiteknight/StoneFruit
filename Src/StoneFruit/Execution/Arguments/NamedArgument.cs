namespace StoneFruit.Execution.Arguments;

/// <summary>
/// An accessor for a named argument which has a name and a value
/// </summary>
public class NamedArgument : INamedArgument
{
    public NamedArgument(string name, string value)
    {
        Name = name.ToLowerInvariant();
        Value = value;
    }

    public string Name { get; }

    public string Value { get; }

    public bool Consumed { get; set; }

    public string AsString(string defaultValue = "") => Value;

    public bool AsBool(bool defaultValue = false) => this.As(bool.Parse, defaultValue);

    public int AsInt(int defaultValue = 0) => this.As(int.Parse, defaultValue);

    public long AsLong(long defaultValue = 0) => this.As(long.Parse, defaultValue);
}
