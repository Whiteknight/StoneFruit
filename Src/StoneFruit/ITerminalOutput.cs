using System;
using StoneFruit.Utility;

namespace StoneFruit
{
    /// <summary>
    /// Abstraction for output
    /// </summary>
    public interface ITerminalOutput
    {
        void WriteLine();
        void WriteLine(ConsoleColor color, string line);
        void WriteLine(string line);

        void Write(ConsoleColor color, string str);
        void Write(string str);

        string Prompt(string prompt, bool mustProvide = true, bool keepHistory = true);

        int ConsoleWidth { get; }
        int ConsoleHeight { get; }
    }

    public static class TerminalOutputExtensions
    {
        public static void WriteLineFmt(this ITerminalOutput output, ConsoleColor color, string fmt, params object[] args)
        {
            Assert.ArgumentNotNull(output, nameof(output));
            Assert.ArgumentNotNullOrEmpty(fmt, nameof(fmt));
            var line = string.Format(fmt, args);
            output.WriteLine(color, line);
        }

        public static void WriteLineFmt(this ITerminalOutput output, string fmt, params object[] args)
        {
            Assert.ArgumentNotNull(output, nameof(output));
            Assert.ArgumentNotNullOrEmpty(fmt, nameof(fmt));
            var line = string.Format(fmt, args);
            output.WriteLine(line);
        }

        public static void WriteFormat(this ITerminalOutput output, ConsoleColor color, string fmt, params object[] args)
        {
            Assert.ArgumentNotNull(output, nameof(output));
            Assert.ArgumentNotNullOrEmpty(fmt, nameof(fmt));
            var line = string.Format(fmt, args);
            output.Write(color, line);
        }

        public static void WriteFormat(this ITerminalOutput output, string fmt, params object[] args)
        {
            Assert.ArgumentNotNull(output, nameof(output));
            Assert.ArgumentNotNullOrEmpty(fmt, nameof(fmt));
            var line = string.Format(fmt, args);
            output.Write(line);
        }

        public static void RedLine(this ITerminalOutput output, string line)
        {
            Assert.ArgumentNotNull(output, nameof(output));
            output?.WriteLine(ConsoleColor.Red, line);
        }

        public static void GreenLine(this ITerminalOutput output, string line)
        {
            Assert.ArgumentNotNull(output, nameof(output));
            output?.WriteLine(ConsoleColor.Green, line);
        }

        public static void WhiteLine(this ITerminalOutput output, string line)
        {
            Assert.ArgumentNotNull(output, nameof(output));
            output?.WriteLine(ConsoleColor.White, line);
        }
    }
}