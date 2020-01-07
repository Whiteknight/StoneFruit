using System;

namespace StoneFruit
{
    public interface ITerminalOutput
    {
        void WriteLine();
        void WriteLine(ConsoleColor color, string fmt, params object[] args);
        void WriteLine(string fmt, params object[] args);

        void Write(ConsoleColor color, string fmt, params object[] args);
        void Write(string fmt, params object[] args);

        int ConsoleWidth { get; }
        int ConsoleHeight { get; }
    }

    public static class TerminalOutputExtensions
    {
        public static void RedLine(this ITerminalOutput output, string fmt, params object[] args)
        {
            output?.WriteLine(ConsoleColor.Red, fmt, args);
        }

        public static void GreenLine(this ITerminalOutput output, string fmt, params object[] args)
        {
            output?.WriteLine(ConsoleColor.Green, fmt, args);
        }

        public static void WhiteLine(this ITerminalOutput output, string fmt, params object[] args)
        {
            output?.WriteLine(ConsoleColor.White, fmt, args);
        }
    }
}