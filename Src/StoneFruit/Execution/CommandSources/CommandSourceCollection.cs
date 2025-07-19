using System.Collections.Generic;
using StoneFruit.Execution.Scripts;

namespace StoneFruit.Execution.CommandSources;

/// <summary>
/// Ordered collection of ICommandSources. The first source is drained and then removed from
/// the list.
/// </summary>
public class CommandSourceCollection
{
    private readonly LinkedList<ICommandSource> _sources;

    public CommandSourceCollection()
    {
        _sources = new LinkedList<ICommandSource>();
    }

    public void AddToEnd(ICommandSource source)
    {
        if (source == null)
            return;
        _sources.AddLast(source);
    }

    public void AddToEnd(params string[] commands) => AddToEnd(new QueueCommandSource(commands));

    public void AddToEnd(EventScript script, ICommandParser parser, IArgumentCollection arguments)
        => AddToEnd(new ScriptCommandSource(script, parser, arguments));

    public Maybe<ArgumentsOrString> GetNextCommand()
    {
        while (true)
        {
            if (_sources.Count == 0)
                return default;

            var firstSource = _sources!.First!.Value;

            var next = firstSource.GetNextCommand();
            if (next.IsSuccess)
                return next;

            _sources.RemoveFirst();
        }
    }
}
