using System;
using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.BuiltInVerbs
{
    [CommandDetails("help", "Get help for available commands")]
    public class HelpCommand : ICommandVerb
    {
        private readonly ITerminalOutput _output;
        private readonly ICommandSource _commands;

        public HelpCommand(ITerminalOutput output, ICommandSource commands)
        {
            _output = output;
            _commands = commands;
        }

        public void Execute()
        {
            var commandsList = _commands.GetAll().ToList();
            int maxCommandLength = commandsList.Select(c => c.Name.Length).Max();
            int descStartColumn = maxCommandLength + 2;
            var blankPrefix = new string(' ', descStartColumn);
            int maxDescLineLength = _output.ConsoleWidth - descStartColumn;

            foreach (var command in _commands.GetAll())
            {
                _output.Write(ConsoleColor.Green, command.Name);
                var buffer = new string(' ', descStartColumn - command.Name.Length);
                _output.Write(buffer);
                var lines = GetDescriptionLines(command.Description, maxDescLineLength);
                if (lines.Count == 0)
                {
                    _output.WriteLine();
                    continue;
                }

                _output.WriteLine(lines[0]);
                foreach (var line in lines.Skip(1))
                    _output.WriteLine($"{blankPrefix}{line}");
            }
        }

        private List<string> GetDescriptionLines(string desc, int max)
        {
            if (max <= 0 || string.IsNullOrEmpty(desc))
                return new List<string>();
            var list = new List<string>();
            int start = 0;
            while (start < desc.Length)
            {
                var available = Math.Min(start + max, desc.Length);
                var line = desc.Substring(start, available);
                start += max;
                list.Add(line);
            }

            return list;
        }
    }
}