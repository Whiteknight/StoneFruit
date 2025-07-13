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
            .Map(script => (IHandlerBase)new ScriptHandler(context, script));

    public IEnumerable<IVerbInfo> GetAll() => _scripts.GetAll().Select(kvp => kvp.Value);

    public Maybe<IVerbInfo> GetByName(Verb verb) => _scripts.Get(verb).Map(i => (IVerbInfo)i);

    public void AddScript(Verb verb, IEnumerable<string> lines, string description, string usage, string group)
    {
        var scriptLines = lines.OrEmptyIfNull().ToList();
        if (scriptLines.Count == 0)
            return;
        var script = new Script(verb, scriptLines, description, usage, group);
        _scripts.Insert(verb, script);
    }

    public int Count => _scripts.Count;

    // This is going to stay a private child class because it has ugly mutable state
    // and it's only used here.
    private sealed class Script : IVerbInfo
    {
        private readonly IReadOnlyList<string> _lines;
        private IReadOnlyList<CommandFormat>? _formats;

        public Script(Verb verb, IReadOnlyList<string> lines, string description, string usage, string group)
        {
            _lines = lines;
            Verb = verb;
            Description = GetDescription(Verb, description);
            Usage = GetUsage(Verb, usage, lines);
            Group = group ?? string.Empty;
        }

        public Verb Verb { get; }
        public string Description { get; }
        public string Usage { get; }
        public string Group { get; }
        public bool ShouldShowInHelp => true;

        public IEnumerable<CommandFormat> GetFormats(ICommandParser parser)
        {
            // Parse the script lines and cache the results so we aren't re-parsing
            if (_formats != null)
                return _formats;
            _formats = _lines
                .Select(parser.ParseScript)
                .ToList();
            return _formats;
        }

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
    }

    // We need to keep ScriptHandler private child class, so it doesn't get scooped up
    // during DI container scan
    private sealed record ScriptHandler(HandlerContext Context, Script Script) : IHandler
    {
        public void Execute()
        {
            // Get the format objects, parsing them if necessary
            var formats = Script.GetFormats(Context.Parser);
            foreach (var lineFormat in formats)
            {
                // Fill in arguments to the formats to create the command
                var formattedArguments = lineFormat.Format(Context.Arguments);
                Context.Arguments.Reset();

                // Add the command to the EngineState for execution after this
                Context.State.Commands.Append(new ArgumentsOrString(formattedArguments));
            }
        }
    }
}
