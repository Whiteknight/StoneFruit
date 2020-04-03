using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Handlers
{
    [Verb(Name)]
    [Verb("quit", showInHelp: false)]
    public class ExitHandler : IHandler
    {
        public const string Name = "exit";

        private readonly EngineState _state;
        private readonly IArguments _args;

        public ExitHandler(EngineState state, IArguments args)
        {
            _state = state;
            _args = args;
        }

        public static string Description => "Exits the application";

        public void Execute()
        {
            var exitCode = _args.Shift().AsInt(Constants.ExitCodeOk);
            _state.Exit(exitCode);
        }
    }
}