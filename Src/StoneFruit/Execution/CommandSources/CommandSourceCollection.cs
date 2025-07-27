using System.Collections.Generic;
using StoneFruit.Execution.Scripts;

namespace StoneFruit.Execution.CommandSources;

/// <summary>
/// Ordered collection of ICommandSources. The first source is drained and then removed from
/// the list.
/// </summary>
public readonly record struct CommandSourceCollection(LinkedList<ICommandSource> Sources)
{
    public Maybe<ArgumentsOrString> GetNextCommand()
    {
        while (true)
        {
            if (Sources.Count == 0)
                return default;

            var firstSource = Sources!.First!.Value;

            var next = firstSource.GetNextCommand();
            if (next.IsSuccess)
                return next;

            Sources.RemoveFirst();
        }
    }
}

public class CommandSourcesBuilder
{
    private readonly LinkedList<ICommandSource> _sources;
    private readonly ICommandParser _parser;
    private readonly IInput _input;
    private readonly IEnvironments _environments;
    private readonly EngineState _state;

    public CommandSourcesBuilder(ICommandParser parser, IInput input, IEnvironments environments, EngineState state)
    {
        _sources = new LinkedList<ICommandSource>();
        _parser = parser;
        _input = input;
        _environments = environments;
        _state = state;
    }

    public CommandSourceCollection Build()
        => new CommandSourceCollection(_sources);

    public CommandSourcesBuilder AddToEnd(ICommandSource source)
    {
        if (source != null)
            _sources.AddLast(source);
        return this;
    }

    public CommandSourcesBuilder AddToEnd(params string[] commands)
        => AddToEnd(new QueueCommandSource(commands));

    public CommandSourcesBuilder AddToEnd(EventScript script, IArgumentCollection arguments)
        => AddToEnd(new ScriptCommandSource(script, _parser, arguments));

    public CommandSourcesBuilder AddPromptToEnd()
        => AddToEnd(new PromptCommandSource(_input, _environments, _state.Metadata));
}
