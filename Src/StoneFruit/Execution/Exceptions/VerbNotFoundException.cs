namespace StoneFruit.Execution.Exceptions;

/// <summary>
/// Exception thrown by the engine when a verb cannot be found.
/// </summary>
public class VerbNotFoundException : InternalException
{
    public string Verb { get; private set; }

    public VerbNotFoundException(string message)
        : base(message)
    {
        Verb = string.Empty;
    }

    public static VerbNotFoundException FromArguments(IArguments arguments)
    {
        var firstPositional = arguments.Shift();
        return firstPositional.Exists()
            ? new VerbNotFoundException($"Could not find a handler for verb {firstPositional.AsString()}") { Verb = firstPositional.AsString() }
            : new VerbNotFoundException("No verb provided. You must provide at least one verb");
    }
}
