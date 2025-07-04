namespace StoneFruit.Execution;

/// <summary>
/// Keep track of how many commands have been executed without user input and show
/// a prompt to the user if the limit has been exceeded. Depending on user input the
/// execution can continue or the command queue can be cleared.
/// </summary>
public class InteractiveEngineStateCommandCounter : IEngineStateCommandCounter
{
    private const string _numberOfCommandsKey = "_numberOfCommands";
    private readonly EngineStateCommandQueue _commands;
    private readonly EngineStateMetadataCache _metadata;
    private readonly EngineSettings _settings;

    public InteractiveEngineStateCommandCounter(EngineStateCommandQueue commands, EngineStateMetadataCache metadata, EngineSettings settings)
    {
        _commands = commands;
        _metadata = metadata;
        _settings = settings;
    }

    public void ReceiveUserInput()
    {
        // Set to -1 so when we execute the current command that the user just input,
        // we are back to 0 commands without user input.
        _metadata.Add(_numberOfCommandsKey, -1);
    }

    public bool VerifyCanExecuteNextCommand(ICommandParser parser, IOutput output)
    {
        var consecutiveCommands = _metadata.Get(_numberOfCommandsKey)
            .Map(o => int.TryParse(o.ToString(), out var val) ? val : 0)
            .GetValueOrDefault(0);
        var limit = _settings.MaxInputlessCommands;
        if (consecutiveCommands < limit)
        {
            _metadata.Add(_numberOfCommandsKey, consecutiveCommands + 1);
            return true;
        }

        var cont = output.Prompt("Maximum command count reached, continue? (y/n)");
        if (cont.GetValueOrDefault("n").Equals("y", System.StringComparison.InvariantCultureIgnoreCase))
        {
            ReceiveUserInput();
            return true;
        }

        // Clear the commands in state, so we can get back to the prompt
        _commands.Clear();
        return false;
    }
}