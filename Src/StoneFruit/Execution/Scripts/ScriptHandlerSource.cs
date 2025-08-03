using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParserObjects;
using StoneFruit.Execution.Trie;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Scripts;

/// <summary>
/// Handler source which maintains a list of scripts and wraps each script in a handler.
/// </summary>
public class ScriptHandlerSource : IHandlerSource
{
    private readonly VerbTrie<Script> _scripts;

    public ScriptHandlerSource()
    {
        _scripts = new VerbTrie<Script>();
    }

    public Maybe<IHandlerBase> GetInstance(HandlerContext context)
        => _scripts.Get(context.Arguments)
            .Map(script => (IHandlerBase)new ScriptHandler(script));

    public IEnumerable<IVerbInfo> GetAll() => _scripts.GetAll().Select(kvp => kvp.Value);

    public Maybe<IVerbInfo> GetByName(Verb verb) => _scripts.Get(verb).Map(i => (IVerbInfo)i);

    public void AddScript(Verb verb, IEnumerable<string> lines, string description, string usage, string group)
    {
        var scriptLines = lines.OrEmptyIfNull().ToList();
        if (scriptLines.Count == 0)
            return;
        description = GetDescription(verb, description);
        usage = GetUsage(verb, usage, scriptLines);
        group ??= string.Empty;
        var script = new Script(verb, scriptLines, description, usage, group);
        _scripts.Insert(verb, script);
    }

    public int Count => _scripts.Count;

    private static string GetDescription(Verb verb, string description)
            => string.IsNullOrEmpty(description) ? verb.ToString() : description;

    private static string GetUsage(Verb verb, string usage, IReadOnlyList<string> lines)
    {
        if (!string.IsNullOrEmpty(usage))
            return usage;
        var sb = new StringBuilder();
        sb.AppendLine($"{verb} ...");
        foreach (var line in lines)
            sb.AppendLine($"\t{line}");
        return sb.ToString();
    }

    // This is going to stay a private child class because it has ugly mutable state
    // and it's only used here.
    private sealed record Script(Verb Verb, IReadOnlyList<string> Lines, string Description, string Usage, string Group) : IVerbInfo
    {
        public bool ShouldShowInHelp => true;
    }

    // We need to keep ScriptHandler private child class, so it doesn't get scooped up
    // during DI container scan
    private sealed record ScriptHandler(Script Script) : IHandler
    {
        public void Execute(IArguments arguments, HandlerContext context)
        {
            var lines = context.Parser
                .ParseScript(Script.Lines, context.Arguments)
                .ThrowIfContainsErrors();
            context.Arguments.Reset();
            context.State.Commands.Append(lines);
        }
    }
}
