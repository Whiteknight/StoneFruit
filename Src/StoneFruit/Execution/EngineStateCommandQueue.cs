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

    public void Append(ArgumentsOrString argsOrString)
    {
        _additionalCommands.AddLast(argsOrString);
    }

    public void Append(IEnumerable<ArgumentsOrString> argsOrStrings)
    {
        foreach (var argsOrString in argsOrStrings)
            Append(argsOrString);
    }

    public void Append(EventScript script, IArguments args)
    {
        Append(script.GetCommands(_parser, args));
    }

    public void Prepend(IEnumerable<ArgumentsOrString> argsOrStrings)
    {
        var list = argsOrStrings.ToList();
        for (int i = list.Count - 1; i >= 0; i--)
            _additionalCommands.AddFirst(list[i]);
    }

    public void Prepend(ArgumentsOrString argsOrString)
    {
        _additionalCommands.AddFirst(argsOrString);
    }

    public void Prepend(EventScript script, IArguments args)
    {
        Prepend(script.GetCommands(_parser, args));
    }

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
