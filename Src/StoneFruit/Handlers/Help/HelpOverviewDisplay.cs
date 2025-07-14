using System;
using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.IO;

namespace StoneFruit.Handlers.Help;

public class HelpOverviewDisplay
{
    private readonly IOutput _output;
    private readonly IHandlers _commands;

    public HelpOverviewDisplay(IOutput output, IHandlers commands)
    {
        _output = output;
        _commands = commands;
    }

    public void DisplayOverview(bool showAll)
    {
        var verbInfos = _commands.GetAll()
            .Where(i => showAll || i.ShouldShowInHelp);
        ShowOverviewFor(verbInfos);
    }

    public void DisplayOverviewStartingWith(string word, bool showAll)
    {
        var verbInfos = _commands.GetAll()
            .Where(i =>
                (showAll || i.ShouldShowInHelp)
                && i.Verb[0].StartsWith(word, StringComparison.OrdinalIgnoreCase)
            );
        if (verbInfos.Any())
            ShowOverviewFor(verbInfos);
    }

    private void ShowOverviewFor(IEnumerable<IVerbInfo> infoList)
    {
        int maxCommandLength = infoList.Max(c => c.Verb.ToString().Length);

        var infoGroups = infoList.GroupBy(v => v.Group ?? "").ToDictionary(g => g.Key, g => g.ToList());

        int descStartColumn = maxCommandLength + 4;
        var blankPrefix = new string(' ', descStartColumn);
        int maxDescLineLength = GetConsoleWidth() - descStartColumn;
        OutputOverviewGroups(infoGroups, descStartColumn, blankPrefix, maxDescLineLength);

        _output.WriteLine($"Type '{HelpHandler.Name} <verb>' to get more information, if available.");
    }

    private void OutputOverviewGroups(Dictionary<string, List<IVerbInfo>> verbInfoGroups, int descStartColumn, string blankPrefix, int maxDescLineLength)
    {
        // Show items without a group first
        // Items with a user-defined group second
        // Items in the Built-Ins group last
        if (verbInfoGroups.TryGetValue("", out var value))
            OutputVerbList("", value, descStartColumn, maxDescLineLength, blankPrefix);

        foreach (var infoGroup in verbInfoGroups.Where(g => g.Key != HelpHandler.BuiltinsGroup && g.Key != ""))
            OutputVerbList(infoGroup.Key, infoGroup.Value, descStartColumn, maxDescLineLength, blankPrefix);

        if (verbInfoGroups.TryGetValue(HelpHandler.BuiltinsGroup, out var builtins) && builtins.Count != 0)
            OutputVerbList("Built-In Verbs", builtins, descStartColumn, maxDescLineLength, blankPrefix);
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

    private static List<string> GetDescriptionLines(string desc, int max)
    {
        if (max <= 0 || string.IsNullOrEmpty(desc))
            return [];

        if (desc.Length < max)
            return [desc];

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

    private string OutputGroupNameAndSetupPadding(string groupName)
    {
        // If there is no group name do nothing and return nothing
        if (string.IsNullOrEmpty(groupName))
            return string.Empty;

        // Otherwise, output the group name and return a padding value that we
        // use to indent all items in this group
        _output
            .Write("== ")
            .Color(ConsoleColor.Blue)
            .Write(groupName)
            .Color(Brush.Default)
            .WriteLine(" ==");
        return "  ";
    }
}
