using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Arguments;
using StoneFruit.Utility;

namespace StoneFruit.BuiltInVerbs
{
    public class HelpCommand : ICommandVerb
    {
        private readonly ITerminalOutput _output;
        private readonly ICommandSource _commands;
        private readonly CommandArguments _args;

        public HelpCommand(ITerminalOutput output, ICommandSource commands, CommandArguments args)
        {
            _output = output;
            _commands = commands;
            _args = args;
        }

        public static string Description => "List all commands or get detailed information for a single command";

        public static string Help => @"help 
Get overview information for all available verbs. The verb Command class must implement

    public static string Description {{ get; }} 

help <command-name>
Get detailed help information for the given verb. The verb Command class must implement

    public static string Help {{ get; }}
";

        public void Execute()
        {
            // TODO: Mode where we take the name of a command and try to offer detailed information
            // Will probably require some kind of extension to the command object to provide that info
            var arg = _args.Shift();
            if (arg.Exists())
            {
                GetDetail(arg.Value);
                return;
            }

            GetOverview();
        }

        private void GetDetail(string name)
        {
            var commandType = _commands.GetCommandTypeByName(name);
            if (commandType == null)
                throw new Exception($"Cannot find command named '{name}'");
            var help = commandType.GetHelp();
            _output.WriteLine(help);
        }

        private void GetOverview()
        {
            var commandsList = _commands.GetAll().ToList();
            int maxCommandLength = commandsList.Select(c => c.Key.Length).Max();
            int descStartColumn = maxCommandLength + 2;
            var blankPrefix = new string(' ', descStartColumn);
            int maxDescLineLength = _output.ConsoleWidth - descStartColumn;

            foreach (var type in _commands.GetAll())
            {
                _output.Write(ConsoleColor.Green, type.Key);
                var buffer = new string(' ', descStartColumn - type.Key.Length);
                _output.Write(buffer);
                var desc = type.Value.GetDescription();
                var lines = GetDescriptionLines(desc, maxDescLineLength);
                if (lines.Count == 0)
                {
                    _output.WriteLine();
                    continue;
                }

                _output.WriteLine(lines[0]);
                foreach (var line in lines.Skip(1))
                    _output.WriteLine($"{blankPrefix}{line}");
            }

            _output.WriteLine("Type 'help <command-name>' to get more information, if available.");
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