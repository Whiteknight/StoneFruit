using StoneFruit.Execution;

namespace StoneFruit.BuiltInVerbs
{
    
    [CommandName(Name)]
    [CommandName("quit", showInHelp: false)]
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
            // TODO: Exit Code argument
            _state.ShouldExit = true;
        }
    }
}