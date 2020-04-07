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

        public HeadlessEngineStateCommandCounter(EngineStateCommandQueue commands, EngineEventCatalog events, EngineSettings settings)
        {
            _commands = commands;
            _events = events;
            _settings = settings;
            _consecutiveCommands = 0;
        }

        public void ReceiveUserInput()
        {
            // This will probably never be needed, but just in case...
            _consecutiveCommands = 0;
        }

        public bool VerifyCanExecuteNextCommand(ICommandParser parser, IOutput output)
        {
            var limit = _settings.MaxInputlessCommands;
            if (_consecutiveCommands <= limit)
            {
                _consecutiveCommands++;
                return true;
            }

            // Clear the counter so we can execute the exit script
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