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
            // TODO: Need to figure out the correct error result here
            .Map(env => _output.Prompt(env).GetValueOrDefault(string.Empty))
            .OnSuccess(_ => _metadata.Add(Constants.Metadata.CurrentCommandIsUserInput, true.ToString()))
            .Match(
                s => new Maybe<ArgumentsOrString>(new ArgumentsOrString(s)),
                _ => default);
}
