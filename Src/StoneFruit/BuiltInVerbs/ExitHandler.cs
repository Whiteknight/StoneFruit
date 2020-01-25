using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.BuiltInVerbs
{
    [CommandName(Name)]
    [CommandName("quit", showInHelp: false)]
    public class ExitHandler : ICommandHandler
    {
        public const string Name = "exit";

        private readonly EngineState _state;
        private readonly CommandArguments _args;

        public ExitHandler(EngineState state, CommandArguments args)
        {
            _state = state;
            _args = args;
        }

        public static string Description => "Exits the application";

        public void Execute()
        {
            var exitCode = _args.Shift().AsInt();
            _state.Exit(exitCode);
        }
    }
}