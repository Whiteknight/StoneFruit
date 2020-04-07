using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution
{
    /// <summary>
    /// Keep track of how many commands have been executed without user input and show
    /// a prompt to the user if the limit has been exceeded. Depending on user input the
    /// execution can continue or the command queue can be cleared
    /// </summary>
    public class InteractiveEngineStateCommandCounter : IEngineStateCommandCounter
    {
        private readonly EngineStateCommandQueue _commands;
        private readonly EngineSettings _settings;
        private int _consecutiveCommands;

        public InteractiveEngineStateCommandCounter(EngineStateCommandQueue commands, EngineSettings settings)
        {
            _commands = commands;
            _settings = settings;
            _consecutiveCommands = 0;
        }

        public void ReceiveUserInput()
        {
            // Set to -1 so when we execute the current command that the user just input,
            // we are back to 0 commands without user input.
            _consecutiveCommands = -1;
        }

        public bool VerifyCanExecuteNextCommand(ICommandParser parser, IOutput output)
        {
            var limit = _settings.MaxInputlessCommands;
            if (_consecutiveCommands < limit)
            {
                _consecutiveCommands++;
                return true;
            }

            var cont = output.Prompt("Maximum command count reached, continue? (y/n)");
            if (cont?.ToLowerInvariant() == "y")
            {
                ReceiveUserInput();
                return true;
            }

            // Clear the commands in state, so we can get back to the prompt
            _commands.Clear();
            return false;
        }
    }
}