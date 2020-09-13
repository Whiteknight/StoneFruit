namespace StoneFruit
{
    /// <summary>
    ///  Information about a registered verb.
    /// </summary>
    public interface IVerbInfo
    {
        /// <summary>
        /// The verb used to invoke the handler. All verbs are case-insensitive
        /// </summary>
        string Verb { get; }

        /// <summary>
        /// A short description of the verb.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Detailed usage information about the verb
        /// </summary>
        string Usage { get; }

        /// <summary>
        /// The grouping that the verb belongs in.
        /// </summary>
        string Group { get; }

        /// <summary>
        /// Whether or not to show the verb in the list when executing "help"
        /// </summary>
        bool ShouldShowInHelp { get; }
    }
}