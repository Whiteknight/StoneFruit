using System.Collections.Generic;

namespace StoneFruit.Execution.CommandSources;

/// <summary>
/// Makes a literal list of commands available to the runloop.
/// </summary>
public class QueueCommandSource : ICommandSource
{
    private readonly Queue<string> _commands;

    public QueueCommandSource(params string[] commands)
    {
        _commands = new Queue<string>(commands);
    }

    public Maybe<ArgumentsOrString> GetNextCommand()
    {
        if (_commands.Count == 0)
            return default;
        var value = _commands.Dequeue();
        return new ArgumentsOrString(value);
    }
}
