using StoneFruit.Execution.Exceptions;

namespace StoneFruit.Execution;

/// <summary>
/// Keeps track of how many commands have been executed and causes the headless runloop
/// to terminate if the limit is exceeded.
/// </summary>
public class HeadlessEngineStateCommandCounter : IEngineStateCommandCounter
{
    private readonly EngineStateCommandQueue _commands;
    private readonly EngineStateMetadataCache _metadata;
    private readonly EngineSettings _settings;
    private readonly EngineState _state;

    public HeadlessEngineStateCommandCounter(EngineStateCommandQueue commands, EngineStateMetadataCache metadata, EngineSettings settings, EngineState state)
    {
        _commands = commands;
        _metadata = metadata;
        _settings = settings;
        _state = state;
    }

    public bool VerifyCanExecuteNextCommand(ICommandParser parser)
    {
        // There's no obvious way for this to be true, but just to be safe for future scenarios...
        var isFromUser = _metadata.Get(Constants.Metadata.CurrentCommandIsUserInput)
            .Map(o => bool.TryParse(o.ToString(), out var val) && val)
            .GetValueOrDefault(false);
        if (isFromUser)
        {
            _metadata.Add(Constants.Metadata.ConsecutiveCommandsWithoutUserInput, 0);
            _metadata.Remove(Constants.Metadata.CurrentCommandIsUserInput);
            _metadata.Remove(Constants.Metadata.ConsecutiveCommandsReachedLimit);
            return true;
        }

        var consecutiveCommands = _metadata.Get(Constants.Metadata.ConsecutiveCommandsWithoutUserInput)
            .Map(o => int.TryParse(o.ToString(), out var val) ? val : 0)
            .GetValueOrDefault(0);
        var limit = _settings.MaxInputlessCommands;
        if (consecutiveCommands <= limit)
        {
            _metadata.Add(Constants.Metadata.ConsecutiveCommandsWithoutUserInput, consecutiveCommands + 1);
            return true;
        }

        // If we've already reached the limit, we don't execute the script again. We just
        // bail out
        _commands.Clear();
        var hasReachedLimit = _metadata.Get(Constants.Metadata.ConsecutiveCommandsReachedLimit)
            .Map(o => bool.TryParse(o.ToString(), out var val) && val)
            .GetValueOrDefault(false);
        if (hasReachedLimit)
            throw new ExecutionException("The MaximumHeadlessCommands script is too long and has been terminated");

        // Clear the counter so we can execute the exit script. Set the limit flag so we don't
        // recurse here again until the next user input has been received.
        _metadata.Add(Constants.Metadata.ConsecutiveCommandsReachedLimit, true.ToString());
        _metadata.Add(Constants.Metadata.ConsecutiveCommandsWithoutUserInput, 0);
        _commands.Clear();

        _state.OnMaximumHeadlessCommands();

        return false;
    }
}
