namespace StoneFruit.Execution.Arguments;

/// <summary>
/// Accessor for a flag argument which contains a name but no value.
/// </summary>
public class FlagArgument : IFlagArgument
{
    public FlagArgument(string name)
    {
        Name = name.ToLowerInvariant();
    }

    public string Name { get; }

    public bool Consumed { get; set; }
}
