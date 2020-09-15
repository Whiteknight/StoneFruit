using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Handlers;

namespace StoneFruit.Handlers
{
    [Verb(Name)]
    public class HelpHandler : IHandler
    {
        public const string Name = "help";
        public const string BuiltinsGroup = "__STONEFRUIT_BUILTIN";

        private readonly IOutput _output;
        private readonly IHandlers _commands;
        private readonly IArguments _args;

        public HelpHandler(IOutput output, IHandlers commands, IArguments args)
        {
            _output = output;
            _commands = commands;
            _args = args;
        }

        public static string Group => BuiltinsGroup;

        public static string Description => "List all commands or get detailed information for a single command";

        public static string Usage => @"help [-showall]
Get overview information for all available verbs. The verb Command class must implement

    public static string Description {{ get; }} 

help <command-name>
Get detailed help information for the given verb. The verb Command class must implement

    public static string Usage {{ get; }}

The help command by default hides some internal commands which are not necessary for normal user interaction.
To see all commands, use the -showall flag
";

        public void Execute()
        {
            var arg = _args.Shift();
            if (arg.Exists())
            {
                GetDetail(arg.Value);
                return;
            }

            var showAll = _args.HasFlag("showall");
            GetOverview(showAll);
        }

        private void GetDetail(string name)
        {
            var info = _commands.GetByName(name);
            if (info == null)
                throw new ExecutionException($"Cannot find command named '{name}'");
            _output.WriteLine(info.Usage);
        }

        private void GetOverview(bool showAll)
        {
            var infoList = _commands.GetAll();
            if (!showAll)
                infoList = infoList.Where(i => i.ShouldShowInHelp);
            var infoGroups = infoList.GroupBy(v => v.Group ?? "").ToDictionary(g => g.Key, g => g.ToList());

            int maxCommandLength = infoList.Select(c => c.Verb.Length).Max();
            int descStartColumn = maxCommandLength + 4;
            var blankPrefix = new string(' ', descStartColumn);
            int maxDescLineLength = GetConsoleWidth() - descStartColumn;

            if (infoGroups.ContainsKey(""))
                OutputVerbList("", infoGroups[""], descStartColumn, maxDescLineLength, blankPrefix);

            foreach (var infoGroup in infoGroups.Where(g => g.Key != BuiltinsGroup && g.Key != ""))
                OutputVerbList(infoGroup.Key, infoGroup.Value, descStartColumn, maxDescLineLength, blankPrefix);

            if (infoGroups.ContainsKey(BuiltinsGroup) && infoGroups[BuiltinsGroup].Any())
                OutputVerbList("Built-In Verbs", infoGroups[BuiltinsGroup], descStartColumn, maxDescLineLength, blankPrefix);

            _output.WriteLine("Type 'help <command-name>' to get more information, if available.");
        }

        private int GetConsoleWidth()
        {
            try
            {
                return Console.WindowWidth;
            }
            catch
            {
                return int.MaxValue;
            }
        }

        private void OutputVerbList(string groupName, IEnumerable<IVerbInfo> verbInfos, int descStartColumn, int maxDescLineLength, string blankPrefix)
        {
            string padLeft = "";
            if (!string.IsNullOrEmpty(groupName))
            {
                padLeft = "  ";
                _output
                    .Color(ConsoleColor.Blue)
                    .WriteLine(groupName);
            }

            foreach (var info in verbInfos)
            {
                _output
                    .Color(ConsoleColor.Green)
                    .Write(padLeft)
                    .Write(info.Verb)
                    .Write(new string(' ', descStartColumn - info.Verb.Length));
                var desc = info.Description;
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
        }

        private static List<string> GetDescriptionLines(string desc, int max)
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