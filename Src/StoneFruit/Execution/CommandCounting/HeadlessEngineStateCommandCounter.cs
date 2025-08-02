using StoneFruit.Execution.Exceptions;
using StoneFruit.Execution.Metadata;

namespace StoneFruit.Execution.CommandCounting;

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

    public bool VerifyCanExecuteNextCommand()
    {
        // There's no obvious way for this to be true, but just to be safe for future scenarios...
        if (_metadata.IsCurrentCommandFromUserInput())
        {
            _metadata.ResetCountsOnUserInput();
            return true;
        }

        var consecutiveCommands = _metadata.GetConsecutiveCommandCountWithoutUserInput();
        var limit = _settings.MaxInputlessCommands;
        if (consecutiveCommands <= limit)
        {
            _metadata.IncrementConsecutiveCommandCount();
            return true;
        }

        // If we've already reached the limit, we don't execute the script again. We just
        // bail out
        _commands.Clear();
        if (_metadata.IsConsecutiveCommandLimitReached())
            throw new ExecutionException("The MaximumHeadlessCommands script is too long and has been terminated");

        // Clear the counter so we can execute the exit script. Set the limit flag so we don't
        // recurse here again until the next user input has been received.
        _metadata.SetupForCommandLimitErrorScript();
        _commands.Clear();
        _state.OnMaximumHeadlessCommands();
        return false;
    }
}
