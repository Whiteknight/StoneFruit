using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Scripts;

namespace StoneFruit.Execution;

/// <summary>
/// Queue of prepared commands for the engine to execute. Various events and actions in the
/// system may add additional commands to the front or end of the list.
/// </summary>
public class EngineStateCommandQueue
{
    private readonly LinkedList<ArgumentsOrString> _additionalCommands;
    private readonly ICommandParser _parser;

    public EngineStateCommandQueue(ICommandParser parser)
    {
        _additionalCommands = new LinkedList<ArgumentsOrString>();
        _parser = parser;
    }

    public EngineStateCommandQueue Append(ArgumentsOrString argsOrString)
    {
        _additionalCommands.AddLast(argsOrString);
        return this;
    }

    public EngineStateCommandQueue Append(IEnumerable<ArgumentsOrString> argsOrStrings)
    {
        foreach (var argsOrString in argsOrStrings)
            Append(argsOrString);
        return this;
    }

    public EngineStateCommandQueue Append(EventScript script, IArgumentCollection args)
        => Append(script.GetCommands(_parser, args));

    public EngineStateCommandQueue Prepend(IEnumerable<ArgumentsOrString> argsOrStrings)
    {
        var list = argsOrStrings.ToList();
        for (int i = list.Count - 1; i >= 0; i--)
            _additionalCommands.AddFirst(list[i]);
        return this;
    }

    public EngineStateCommandQueue Prepend(ArgumentsOrString argsOrString)
    {
        _additionalCommands.AddFirst(argsOrString);
        return this;
    }

    public EngineStateCommandQueue Prepend(EventScript script, IArgumentCollection args)
        => Prepend(script.GetCommands(_parser, args));

    public Maybe<ArgumentsOrString> GetNext()
    {
        if (_additionalCommands.Count == 0)
            return default;
        ArgumentsOrString next = _additionalCommands!.First!.Value;
        _additionalCommands.RemoveFirst();
        return next;
    }

    public void Clear() => _additionalCommands.Clear();
}
