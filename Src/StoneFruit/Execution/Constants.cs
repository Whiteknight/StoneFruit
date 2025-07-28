namespace StoneFruit.Execution;

public static class Constants
{
    public static class Metadata
    {
        /// <summary>
        /// Metadata key for the current exception object, to help keep track oferror
        /// loops.
        /// </summary>
        public const string Error = "__CURRENT_EXCEPTION";
        public const string ConsecutiveCommandsWithoutUserInput = "__CONSECUTIVE_COMMANDS";
        public const string ConsecutiveCommandsReachedLimit = "__CONSECUTIVE_COMMANDS_LIMIT_REACHED";
        public const string CurrentCommandIsUserInput = "__CURRENT_COMMAND_FROM_USER";
    }

    /// <summary>
    /// The name of the default environment to use if there are no environments
    /// configured.
    /// </summary>
    public const string EnvironmentNameDefault = "";

    public static readonly char[] SeparatedBySpace = [' '];
}
