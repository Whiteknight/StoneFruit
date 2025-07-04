namespace StoneFruit.Execution.CommandSources;

/// <summary>
/// prompts the user to input commands and exposes them to the runloop.
/// </summary>
public class PromptCommandSource : ICommandSource
{
    private readonly IOutput _output;
    private readonly IEnvironmentCollection _environments;
    private readonly EngineStateMetadataCache _metadata;

    public PromptCommandSource(IOutput output, IEnvironmentCollection environments, EngineStateMetadataCache metadata)
    {
        _output = output;
        _environments = environments;
        _metadata = metadata;
    }

    public Maybe<ArgumentsOrString> GetNextCommand()
    {
        var env = _environments.GetCurrentName().GetValueOrDefault("");
        var str = _output.Prompt(env)
            .Bind(s => string.IsNullOrEmpty(s) ? default : new Maybe<string>(s));
        _metadata.Add(Constants.Metadata.CurrentCommandIsUserInput, true.ToString());
        return str.Map(s => new ArgumentsOrString(s));
    }
}
