using StoneFruit.Utility;

namespace StoneFruit.Execution;

/// <summary>
/// Discriminated union for unparsed command strings or pre-parsed Command objects.
/// </summary>
public class ArgumentsOrString
{
    public ArgumentsOrString(string asString)
    {
        Assert.NotNullOrEmpty(asString, nameof(asString));
        Arguments = null;
        String = asString;
    }

    public ArgumentsOrString(IArguments asArguments)
    {
        Assert.NotNull(asArguments, nameof(asArguments));
        Arguments = asArguments;
        String = null;
    }

    public IArguments? Arguments { get; }

    public string? String { get; }

    public bool IsValid => !string.IsNullOrEmpty(String) || Arguments != null;

    public static implicit operator ArgumentsOrString(string s) => new ArgumentsOrString(s);

    public IArguments GetArguments(ICommandParser parser)
    {
        Assert.NotNull(parser, nameof(parser));
        return Arguments ?? parser.ParseCommand(String!);
    }
}
