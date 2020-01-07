using StoneFruit.Execution;

namespace StoneFruit.BuiltInVerbs
{
    // TODO: Do we need both?
    [CommandDetails("exit", "Exit the application")]
    [CommandDetails("quit", "Exit the application")]
    public class ExitCommand : ICommandVerb
    {
        private readonly EngineState _state;

        public ExitCommand(EngineState state)
        {
            _state = state;
        }

        public void Execute()
        {
            _state.ShouldExit = true;
        }
    }
}