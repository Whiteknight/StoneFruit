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
        /// Whether or not to show the verb in the list when executing "help"
        /// </summary>
        bool ShouldShowInHelp { get; }

        // TODO: Should we show more info here, such as the nature of the handler or the type of the source?
    }
}