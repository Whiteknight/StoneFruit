using System;

namespace StoneFruit.Execution
{
    /// <summary>
    /// An ITerminalOutput adaptor for System.Console and ReadLine
    /// </summary>
    public class ConsoleTerminalOutput : ITerminalOutput
    {
        public void WriteLine()
        {
            Console.WriteLine();
        }

        public void WriteLine(ConsoleColor color, string line)
        {
            WithColor(color, () => WriteLine(line));
        }

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public void Write(ConsoleColor color, string str)
        {
            WithColor(color, () => Write(str));
        }

        public void Write(string str)
        {
            Console.Write(str);
        }

        public string Prompt(string prompt, bool mustProvide = true, bool keepHistory = true)
        {
            while (true)
            {
                var value = ReadLine.Read(prompt + "> ");
                if (string.IsNullOrEmpty(value) && mustProvide)
                    continue;
                if (keepHistory)
                    ReadLine.AddHistory(value);
                return value;
            }
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