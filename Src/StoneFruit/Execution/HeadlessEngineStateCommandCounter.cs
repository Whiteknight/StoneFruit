using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution;

/// <summary>
/// Keeps track of how many commands have been executed and causes the headless runloop
/// to terminate if the limit is exceeded.
/// </summary>
public class HeadlessEngineStateCommandCounter : IEngineStateCommandCounter
{
    private const string _numberOfCommandsKey = "_numberOfCommands";
    private const string _hasReachedLimitKey = "_hasReachedLimit";
    private readonly EngineStateCommandQueue _commands;
    private readonly EngineStateMetadataCache _metadata;
    private readonly EngineEventCatalog _events;
    private readonly EngineSettings _settings;

    public HeadlessEngineStateCommandCounter(EngineStateCommandQueue commands, EngineStateMetadataCache metadata, EngineEventCatalog events, EngineSettings settings)
    {
        _commands = commands;
        _metadata = metadata;
        _events = events;
        _settings = settings;
    }

    public void ReceiveUserInput()
    {
        // This will probably never be needed, but just in case...
        _metadata.Remove(_numberOfCommandsKey);
        _metadata.Remove(_hasReachedLimitKey);
    }

    public bool VerifyCanExecuteNextCommand(ICommandParser parser, IOutput output)
    {
        var consecutiveCommands = _metadata.Get(_numberOfCommandsKey)
            .Map(o => int.TryParse(o.ToString(), out var val) ? val : 0)
            .GetValueOrDefault(0);
        var limit = _settings.MaxInputlessCommands;
        if (consecutiveCommands <= limit)
        {
            _metadata.Add(_numberOfCommandsKey, consecutiveCommands + 1);
            return true;
        }

        // If we've already reached the limit, we don't execute the script again. We just
        // bail out
        _commands.Clear();
        var hasReachedLimit = _metadata.Get(_hasReachedLimitKey)
            .Map(o => bool.TryParse(o.ToString(), out var val) && val)
            .GetValueOrDefault(false);
        if (hasReachedLimit)
            throw new ExecutionException("The MaximumHeadlessCommands script is too long and has been terminated");

        // Clear the counter so we can execute the exit script. Set the limit flag so we don't
        // recurse here again until the next user input has been received.
        _metadata.Add(_hasReachedLimitKey, true.ToString());
        _metadata.Add(_numberOfCommandsKey, 0);
        _commands.Clear();

        var args = SyntheticArguments.From(
            ("limit", limit.ToString()),
            ("exitcode", Constants.ExitCode.MaximumCommands.ToString())
        );
        _commands.Prepend(_events.MaximumHeadlessCommands.GetCommands(parser, args));

        return false;
    }
}
