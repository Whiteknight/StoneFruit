namespace StoneFruit.Execution.CommandSources;

/// <summary>
/// prompts the user to input commands and exposes them to the runloop.
/// </summary>
public class PromptCommandSource : ICommandSource
{
    private readonly IOutput _output;
    private readonly IEnvironmentCollection _environments;
    private readonly EngineState _state;

    public PromptCommandSource(IOutput output, IEnvironmentCollection environments, EngineState state)
    {
        _output = output;
        _environments = environments;
        _state = state;
    }

    public Maybe<ArgumentsOrString> GetNextCommand()
    {
        var env = _environments.GetCurrentName().GetValueOrDefault("");
        var str = _output.Prompt(env)
            .Bind(s => string.IsNullOrEmpty(s) ? default : new Maybe<string>(s));
        _state.CommandCounter.ReceiveUserInput();
        return str.Map(s => new ArgumentsOrString(s));
    }
}
