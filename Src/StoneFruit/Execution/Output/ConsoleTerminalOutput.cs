using System;

namespace StoneFruit.Execution.Output
{
    /// <summary>
    /// An ITerminalOutput adaptor for System.Console and ReadLine
    /// </summary>
    public class ConsoleTerminalOutput : ITerminalOutput
    {
        public ITerminalOutput Color(Brush brush) => new ColoredTerminalOutputWrapper(brush, this);

        public ITerminalOutput WriteLine()
        {
            Console.WriteLine();
            return this;
        }

        public ITerminalOutput WriteLine(string line)
        {
            Console.WriteLine(line);
            return this;
        }

        public ITerminalOutput Write(string str)
        {
            Console.Write(str);
            return this;
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
    }
}