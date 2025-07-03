namespace StoneFruit;

/// <summary>
/// Information about a registered verb.
/// </summary>
public interface IVerbInfo
{
    /// <summary>
    /// Gets the verb used to invoke the handler. All verbs are case-insensitive.
    /// </summary>
    Verb Verb { get; }

    /// <summary>
    /// Gets a short description of the verb.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets detailed usage information about the verb.
    /// </summary>
    string Usage { get; }

    /// <summary>
    /// Gets the grouping that the verb belongs in.
    /// </summary>
    string Group { get; }

    /// <summary>
    /// Gets a value indicating whether whether or not to show the verb in the list when executing "help".
    /// </summary>
    bool ShouldShowInHelp { get; }
}
