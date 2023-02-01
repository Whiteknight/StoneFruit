using System;

namespace StoneFruit
{
    public interface ICommandLine
    {
        string GetRawArguments();
    }

    public class EnvironmentCommandLine : ICommandLine
    {
        // Attempt to get the raw commandline arguments as they were passed to the
        // application. Main(string[] args) is transformed by the shell with quotes
        // stripped. Environment.CommandLine is unmodified but we have to pull the exe name
        // off the front.
        public string GetRawArguments()
        {
            // Environment.CommandLine includes the name of the exe invoked, so strip that
            // off the front. Luckily it seems like quotes are stripped for us.
            var exeName = Environment.GetCommandLineArgs()[0];
            return Environment.CommandLine.Substring(exeName.Length).Trim();
        }
    }

    public class StringCommandLine : ICommandLine
    {
        private readonly string _commandLine;

        public StringCommandLine(string commandLine)
        {
            _commandLine = commandLine;
        }

        public string GetRawArguments() => _commandLine;
    }
}
