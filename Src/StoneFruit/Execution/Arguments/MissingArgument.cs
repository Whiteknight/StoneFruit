namespace StoneFruit.Execution.Arguments;

/// <summary>
/// A Null Object implementation of IArgument and friends. Has no value and allows
/// throwing an exception if a value is required.
/// </summary>
public class MissingArgument : IPositionalArgument, INamedArgument, IFlagArgument
{
    public string Message { get; }

    public MissingArgument(string message)
    {
        Message = message;
    }

    public static MissingArgument NoPositionals()
        => new MissingArgument("Cannot get next positional argument, there are none left. Either you did not pass enough or you consumed them all already.");

    public static MissingArgument PositionalConsumed(int index)
        => new MissingArgument($"Cannot get argument at position {index}. The value has already been consumed.");

    public static MissingArgument NoneNamed(string name)
        => new MissingArgument($"Cannot get argument named '{name}'. You either don't have this argument or you already consumed it");

    public static MissingArgument FlagConsumed(string name)
        => new MissingArgument($"Cannot get flag named '{name}'. You have already consumed it.");

    public static MissingArgument FlagMissing(string name)
        => new MissingArgument($"Cannot get flag named '{name}'");

    public string Value => string.Empty;

    public string Name => string.Empty;

    public bool Consumed
    {
        get => true;
        set
        {
            // Do nothing here, there's nothing to consume
        }
    }

    public IArgument MarkConsumed(bool _ = true) => this;

    public string AsString(string defaultValue = "") => defaultValue;

    public bool AsBool(bool defaultValue = false) => defaultValue;

    public int AsInt(int defaultValue = 0) => defaultValue;

    public long AsLong(long defaultValue = 0) => defaultValue;

    public void Throw(string errorMessage) => throw new ArgumentParseException(string.IsNullOrEmpty(errorMessage) ? Message : errorMessage);
}
