using System;

namespace StoneFruit.Execution;

#pragma warning disable S3218 // Inner class members should not shadow outer class "static" or type members

public readonly record struct ExitCode(int Value)
{
    public static class Constants
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

        public const int Unknown = 100;
    }

    public static ExitCode Ok => new ExitCode(Constants.Ok);
    public static ExitCode HeadlessHelp => new ExitCode(Constants.HeadlessHelp);
    public static ExitCode HeadlessNoVerb => new ExitCode(Constants.HeadlessNoVerb);
    public static ExitCode CascadeError => new ExitCode(Constants.CascadeError);
    public static ExitCode MaximumCommands => new ExitCode(Constants.MaximumCommands);

    public static ExitCode Parse(string input)
    {
        var value = int.TryParse(input, out var v1) && v1 >= 0
            ? v1
            : Constants.Unknown;
        return new ExitCode(value);
    }

    public void Set()
    {
        Environment.ExitCode = Value;
    }

    public override string ToString() => Value.ToString();
}
