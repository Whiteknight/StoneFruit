using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution;

namespace StoneFruit.Handlers
{
    [Verb(Name)]
    public class HelpHandler : IHandler
    {
        public const string Name = "help";
        public const string FlagShowAll = "showall";
        public const string FlagStartsWith = "startswith";
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

        public static string Usage => $@"{Name} [ -{FlagShowAll} ]
    Get overview information for all available verbs. The verb Command class should implement

        public static string {nameof(Description)} {{ get; }}

    The help command by default hides some internal commands which are not necessary for normal
    user interaction. To see all commands, use the -{FlagShowAll} flag.

{Name} <verb>
    Get detailed help information for the given verb. The verb Command class should implement

        public static string {nameof(Usage)} {{ get; }}

{Name} -{FlagStartsWith} <word>
    Get overview information for all handlers whose verbs start with the given word. The verb
    Command class should implement the {nameof(Description)} property, described above.
";

        public void Execute()
        {
            var verb = _args.GetAllPositionals().Select(p => p.AsString()).ToArray();
            var showAll = _args.HasFlag(FlagShowAll);
            if (verb.Length == 0)
            {
                GetOverview(showAll);
                return;
            }

            if (_args.HasFlag(FlagStartsWith))
            {
                GetOverviewStartingWith(verb[0], showAll);
                return;
            }

            GetDetail(verb);
        }

        private void GetDetail(Verb verb)
        {
            var info = _commands.GetByName(verb);
            if (!info.HasValue)
                throw new ExecutionException($"Cannot find command named '{verb}'");
            _output.WriteLine(info.Value.Usage);
        }

        private void GetOverview(bool showAll)
        {
            var infoList = _commands.GetAll()
                .Where(i => showAll || i.ShouldShowInHelp);
            ShowOverview(infoList);
        }

        private void GetOverviewStartingWith(string word, bool showAll)
        {
            var infoList = _commands.GetAll()
                .Where(i => (showAll || i.ShouldShowInHelp)
                    && i.Verb[0].StartsWith(word, StringComparison.OrdinalIgnoreCase)
                );
            if (infoList.Any())
                ShowOverview(infoList);
        }

        private void ShowOverview(IEnumerable<IVerbInfo> infoList)
        {
            int maxCommandLength = infoList.Max(c => c.Verb.ToString().Length);

            var infoGroups = infoList.GroupBy(v => v.Group ?? "").ToDictionary(g => g.Key, g => g.ToList());

            int descStartColumn = maxCommandLength + 4;
            var blankPrefix = new string(' ', descStartColumn);
            int maxDescLineLength = GetConsoleWidth() - descStartColumn;
            OutputOverviewGroups(infoGroups, descStartColumn, blankPrefix, maxDescLineLength);

            _output.WriteLine($"Type '{Name} <verb>' to get more information, if available.");
        }

        private void OutputOverviewGroups(Dictionary<string, List<IVerbInfo>> infoGroups, int descStartColumn, string blankPrefix, int maxDescLineLength)
        {
            if (infoGroups.ContainsKey(""))
                OutputVerbList("", infoGroups[""], descStartColumn, maxDescLineLength, blankPrefix);

            foreach (var infoGroup in infoGroups.Where(g => g.Key != BuiltinsGroup && g.Key != ""))
                OutputVerbList(infoGroup.Key, infoGroup.Value, descStartColumn, maxDescLineLength, blankPrefix);

            if (infoGroups.ContainsKey(BuiltinsGroup) && infoGroups[BuiltinsGroup].Any())
                OutputVerbList("Built-In Verbs", infoGroups[BuiltinsGroup], descStartColumn, maxDescLineLength, blankPrefix);
        }

        private static int GetConsoleWidth()
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
            // Write the group name (if we have one)
            string padLeft = OutputGroupNameAndSetupPadding(groupName);

            // Foreach item in the group output the padding, the verb, then the description
            foreach (var info in verbInfos)
            {
                _output
                    .Color(ConsoleColor.Green)
                    .Write(padLeft)
                    .Write(info.Verb)
                    .Write(new string(' ', descStartColumn - (info.Verb.ToString() + padLeft).Length));
                OutputDescriptionLines(maxDescLineLength, blankPrefix, info.Description);
            }
        }

        private void OutputDescriptionLines(int maxDescLineLength, string blankPrefix, string desc)
        {
            var lines = GetDescriptionLines(desc, maxDescLineLength);
            if (lines.Count == 0)
            {
                _output.WriteLine();
                return;
            }

            // For the first line we are already indented because we have output the padding and
            // verb already. So just write the line. For subsequent lines we need to output the
            // blankPrefix first to line everything up
            _output.WriteLine(lines[0]);
            foreach (var line in lines.Skip(1))
                _output.WriteLine($"{blankPrefix}{line}");
        }

        private string OutputGroupNameAndSetupPadding(string groupName)
        {
            // If there is no group name do nothing and return nothing
            if (string.IsNullOrEmpty(groupName))
                return string.Empty;

            // Otherwise, output the group name and return a padding value that we
            // use to indent all items in this group
            _output
                .Color(ConsoleColor.Blue)
                .WriteLine(groupName);
            return "  ";
        }

        private static IReadOnlyList<string> GetDescriptionLines(string desc, int max)
        {
            if (max <= 0 || string.IsNullOrEmpty(desc))
                return Array.Empty<string>();

            if (desc.Length < max)
                return new[] { desc };

            var list = new List<string>();
            int start = 0;
            while (start < desc.Length)
            {
                if (desc.Length - start < max)
                {
                    list.Add(desc.Substring(start, desc.Length - start));
                    break;
                }

                var line = desc.Substring(start, max);
                start += max;
                list.Add(line);
            }

            return list;
        }
    }
}
