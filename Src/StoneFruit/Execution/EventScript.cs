using System.Collections.Generic;
using System.Linq;
using StoneFruit.Utility;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution;

/// <summary>
/// A script of commands which will be executed in sequence, in response to different
/// Engine events.
/// </summary>
public class EventScript
{
    private readonly string[] _initialLines;
    private readonly List<string> _lines;

    public EventScript(params string[] initialLines)
    {
        _initialLines = initialLines;
        _lines = [.. initialLines];
    }

    public void Reset()
    {
        _lines.Clear();
        _lines.AddRange(_initialLines);
    }

    public void Clear() => _lines.Clear();

    public void Add(params string[] lines)
    {
        _lines.AddRange(NotNull(lines));
    }

    public IEnumerable<ArgumentsOrString> GetCommands(ICommandParser parser, IArguments args)
    {
        NotNull(parser);
        NotNull(args);
        return _lines
            .Where(l => !string.IsNullOrEmpty(l))
            .Select(parser.ParseScript)
            .Select(format => format.Format(args))
            .Tap(_ => args.Reset())
            .Select(c => new ArgumentsOrString(c));
    }

    public override string ToString() => string.Join("\n", _lines);
}
