namespace StoneFruit.Execution.CommandSources;

/// <summary>
/// prompts the user to input commands and exposes them to the runloop.
/// </summary>
public class PromptCommandSource : ICommandSource
{
    private readonly IOutput _output;
    private readonly IEnvironments _environments;
    private readonly EngineStateMetadataCache _metadata;

    public PromptCommandSource(IOutput output, IEnvironments environments, EngineStateMetadataCache metadata)
    {
        _output = output;
        _environments = environments;
        _metadata = metadata;
    }

    public Maybe<ArgumentsOrString> GetNextCommand()
        => _environments.GetCurrentName().Or(() => Constants.EnvironmentNameDefault)
            .And(env => _output.Prompt(env))
            .OnSuccess(_ => _metadata.Add(Constants.Metadata.CurrentCommandIsUserInput, true.ToString()))
            .Map(s => new ArgumentsOrString(s));
}
