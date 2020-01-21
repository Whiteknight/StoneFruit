using StoneFruit.Utility;

namespace StoneFruit
{
    /// <summary>
    /// Abstraction for output
    /// </summary>
    public interface ITerminalOutput
    {
        ITerminalOutput Color(Brush brush);

        ITerminalOutput WriteLine();
        ITerminalOutput WriteLine(string line);

        ITerminalOutput Write(string str);

        string Prompt(string prompt, bool mustProvide = true, bool keepHistory = true);
    }

    public static class TerminalOutputExtensions
    {
        public static ITerminalOutput WriteLineFormat(this ITerminalOutput output, string fmt, params object[] args)
        {
            Assert.ArgumentNotNull(output, nameof(output));
            Assert.ArgumentNotNullOrEmpty(fmt, nameof(fmt));
            var line = string.Format(fmt, args);
            return output.WriteLine(line);
        }

        public static ITerminalOutput WriteFormat(this ITerminalOutput output, string fmt, params object[] args)
        {
            Assert.ArgumentNotNull(output, nameof(output));
            Assert.ArgumentNotNullOrEmpty(fmt, nameof(fmt));
            var line = string.Format(fmt, args);
            return output.Write(line);
        }
    }
}