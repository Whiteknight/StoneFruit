namespace StoneFruit.Execution
{
    public static class Constants
    {
        public const int ExitCodeOk = 0;
        public const int ExitCodeHeadlessHelp = 0;
        public const int ExitCodeHeadlessNoVerb = 1;
        public const int ExitCodeCascadeError = 2;
        public const int ExitCodeMaximumCommands = 3;

        public const string MetadataError = "__CURRENT_EXCEPTION";

        public const string EnvironmentNameDefault = "";
    }
}