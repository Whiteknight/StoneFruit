using StoneFruit.Execution.Metadata;

namespace StoneFruit.Execution.CommandCounting;

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
    private readonly IInput _input;

    public InteractiveEngineStateCommandCounter(EngineStateCommandQueue commands, EngineStateMetadataCache metadata, EngineSettings settings, IInput input)
    {
        _commands = commands;
        _metadata = metadata;
        _settings = settings;
        _input = input;
    }

    public bool VerifyCanExecuteNextCommand()
    {
        if (_metadata.IsCurrentCommandFromUserInput())
        {
            _metadata.ResetCountsOnUserInput();
            return true;
        }

        var consecutiveCommands = _metadata.GetConsecutiveCommandCountWithoutUserInput();
        var limit = _settings.MaxInputlessCommands;
        if (consecutiveCommands < limit)
        {
            _metadata.IncrementConsecutiveCommandCount();
            return true;
        }

        var cont = _input.Prompt("Maximum command count reached, continue? (y/n)");
        _metadata.ResetCountsOnUserInput();
        if (cont.GetValueOrDefault("n").Equals("y", System.StringComparison.InvariantCultureIgnoreCase))
            return true;

        // Clear the commands in state, so we can get back to the prompt
        _commands.Clear();
        return false;
    }
}
