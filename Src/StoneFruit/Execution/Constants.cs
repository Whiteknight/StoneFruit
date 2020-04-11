namespace StoneFruit.Execution
{
    public static class Constants
    {
        /// <summary>
        /// Exit code when everything is going well
        /// </summary>
        public const int ExitCodeOk = 0;

        /// <summary>
        /// Exit code for when successfully returning from headless help
        /// </summary>
        public const int ExitCodeHeadlessHelp = 0;

        /// <summary>
        /// Exit code when we don't have a verb in headless mode
        /// </summary>
        public const int ExitCodeHeadlessNoVerb = 1;

        /// <summary>
        /// Exit code for when a cascade error forces a premature exit
        /// </summary>
        public const int ExitCodeCascadeError = 2;

        /// <summary>
        /// Exit code for when the maximum number of commands are executed in headless
        /// mode
        /// </summary>
        public const int ExitCodeMaximumCommands = 3;

        /// <summary>
        /// Metadata key for the current exception object, to help keep track of error
        /// loops
        /// </summary>
        public const string MetadataError = "__CURRENT_EXCEPTION";

        /// <summary>
        /// The name of the default environment to use if there are no environments
        /// configured.
        /// </summary>
        public const string EnvironmentNameDefault = "";
    }
}