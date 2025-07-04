using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution;

/// <summary>
/// Discriminated union for unparsed command strings or pre-parsed Command objects.
/// </summary>
public class ArgumentsOrString
{
    public ArgumentsOrString(string asString)
    {
        Arguments = null;
        String = NotNull(asString);
    }

    public ArgumentsOrString(IArguments asArguments)
    {
        Arguments = NotNull(asArguments);
        String = null;
    }

    public IArguments? Arguments { get; }

    public string? String { get; }

    public bool IsValid => !string.IsNullOrEmpty(String) || Arguments != null;

    public static implicit operator ArgumentsOrString(string s) => new ArgumentsOrString(s);

    public IArguments GetArguments(ICommandParser parser)
        => Arguments ?? NotNull(parser).ParseCommand(String!);
}
