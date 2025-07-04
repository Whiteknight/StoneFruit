namespace StoneFruit.Execution;

public static class Constants
{
    public static class ExitCode
    {
        /// <summary>
        /// Exit code when everything is going well.
        /// </summary>
        public const int Ok = 0;

        /// <summary>
        /// Exit code for when successfully returning from headless help.
        /// </summary>
        public const int HeadlessHelp = 0;

        /// <summary>
        /// Exit code when we don't have a verb in headless mode.
        /// </summary>
        public const int HeadlessNoVerb = 1;

        /// <summary>
        /// Exit code for when a cascade error forces a premature exit.
        /// </summary>
        public const int CascadeError = 2;

        /// <summary>
        /// Exit code for when the maximum number of commands are executed in headless
        /// mode.
        /// </summary>
        public const int MaximumCommands = 3;
    }

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
}
