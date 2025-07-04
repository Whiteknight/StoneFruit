namespace StoneFruit.Execution;

/// <summary>
/// Keep track of how many commands have been executed without user input and show
/// a prompt to the user if the limit has been exceeded. Depending on user input the
/// execution can continue or the command queue can be cleared.
/// </summary>
public class InteractiveEngineStateCommandCounter : IEngineStateCommandCounter
{
    private readonly EngineStateCommandQueue _commands;
    private readonly EngineStateMetadataCache _metadata;
    private readonly EngineSettings _settings;

    public InteractiveEngineStateCommandCounter(EngineStateCommandQueue commands, EngineStateMetadataCache metadata, EngineSettings settings)
    {
        _commands = commands;
        _metadata = metadata;
        _settings = settings;
    }

    public bool VerifyCanExecuteNextCommand(ICommandParser parser, IOutput output)
    {
        var isFromUser = _metadata.Get(Constants.Metadata.CurrentCommandIsUserInput)
            .Map(o => bool.TryParse(o.ToString(), out var val) && val)
            .GetValueOrDefault(false);
        if (isFromUser)
        {
            _metadata.Add(Constants.Metadata.ConsecutiveCommandsWithoutUserInput, 0);
            _metadata.Remove(Constants.Metadata.CurrentCommandIsUserInput);
            return true;
        }

        var consecutiveCommands = _metadata.Get(Constants.Metadata.ConsecutiveCommandsWithoutUserInput)
            .Map(o => int.TryParse(o.ToString(), out var val) ? val : 0)
            .GetValueOrDefault(0);
        var limit = _settings.MaxInputlessCommands;
        if (consecutiveCommands < limit)
        {
            _metadata.Add(Constants.Metadata.ConsecutiveCommandsWithoutUserInput, consecutiveCommands + 1);
            return true;
        }

        var cont = output.Prompt("Maximum command count reached, continue? (y/n)");
        if (cont.GetValueOrDefault("n").Equals("y", System.StringComparison.InvariantCultureIgnoreCase))
        {
            _metadata.Add(Constants.Metadata.ConsecutiveCommandsWithoutUserInput, 0);
            return true;
        }

        // Clear the commands in state, so we can get back to the prompt
        _commands.Clear();
        return false;
    }
}