using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution
{
    /// <summary>
    /// Keeps track of how many commands have been executed and causes the headless runloop
    /// to terminate if the limit is exceeded
    /// </summary>
    public class HeadlessEngineStateCommandCounter : IEngineStateCommandCounter
    {
        private readonly EngineStateCommandQueue _commands;
        private readonly EngineEventCatalog _events;
        private readonly EngineSettings _settings;

        private int _consecutiveCommands;
        private bool _hasReachedLimit;

        public HeadlessEngineStateCommandCounter(EngineStateCommandQueue commands, EngineEventCatalog events, EngineSettings settings)
        {
            _commands = commands;
            _events = events;
            _settings = settings;
            _consecutiveCommands = 0;
            _hasReachedLimit = false;
        }

        public void ReceiveUserInput()
        {
            // This will probably never be needed, but just in case...
            _consecutiveCommands = 0;
            _hasReachedLimit = false;
        }

        public bool VerifyCanExecuteNextCommand(ICommandParser parser, IOutput output)
        {
            var limit = _settings.MaxInputlessCommands;
            if (_consecutiveCommands <= limit)
            {
                _consecutiveCommands++;
                return true;
            }

            // If we've already reached the limit, we don't execute the script again. We just
            // bail out
            _commands.Clear();
            if (_hasReachedLimit)
                throw new ExecutionException("The MaximumHeadlessCommands script is too long and has been terminated");

            // Clear the counter so we can execute the exit script. Set the limit flag so we don't
            // recurse here again until the next user input has been received.
            _hasReachedLimit = true;
            _consecutiveCommands = 0;
            _commands.Clear();

            var args = SyntheticArguments.From(
                ("limit", limit.ToString()),
                ("exitcode", Constants.ExitCodeMaximumCommands.ToString())
            );
            _commands.Prepend(_events.MaximumHeadlessCommands.GetCommands(parser, args));

            return false;
        }
    }
}
