using System;

namespace StoneFruit.Execution
{
    public class ConsoleTerminalOutput : ITerminalOutput
    {
        public void WriteLine()
        {
            Console.WriteLine();
        }

        public void WriteLine(ConsoleColor color, string fmt, params object[] args)
        {
            WithColor(color, () => WriteLine(fmt, args));
        }

        public void WriteLine(string fmt, params object[] args)
        {
            var str = string.Format(fmt, args);
            Console.WriteLine(str);
        }

        public void Write(ConsoleColor color, string fmt, params object[] args)
        {
            WithColor(color, () => Write(fmt, args));
        }

        public void Write(string fmt, params object[] args)
        {
            var str = string.Format(fmt, args);
            Console.Write(str);
        }

        public int ConsoleWidth => System.Console.WindowWidth;
        public int ConsoleHeight => System.Console.WindowHeight;

        private static void WithColor(ConsoleColor color, Action act)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            try
            {
                act();
            }
            finally
            {
                Console.ForegroundColor = currentColor;
            }
        }
    }
}