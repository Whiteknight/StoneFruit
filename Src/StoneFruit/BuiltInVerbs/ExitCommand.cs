using StoneFruit.Execution;

namespace StoneFruit.BuiltInVerbs
{
    // TODO: Do we need both?
    [CommandName(Name)]
    [CommandName("quit")]
    public class ExitCommand : ICommandVerb
    {
        public const string Name = "exit";

        private readonly EngineState _state;

        public ExitCommand(EngineState state)
        {
            _state = state;
        }

        public static string Description => "Exits the application";

        public void Execute()
        {
            _state.ShouldExit = true;
        }
    }
}