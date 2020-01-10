using System;

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
        public static void RedLine(this ITerminalOutput output, string line)
        {
            output?.WriteLine(ConsoleColor.Red, line);
        }

        public static void GreenLine(this ITerminalOutput output, string line)
        {
            output?.WriteLine(ConsoleColor.Green, line);
        }

        public static void WhiteLine(this ITerminalOutput output, string line)
        {
            output?.WriteLine(ConsoleColor.White, line);
        }
    }
}