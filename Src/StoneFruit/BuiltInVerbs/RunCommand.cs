using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.BuiltInVerbs
{
    [CommandDetails("run", "run a list of commands")]
    public class RunCommand : ICommandVerb
    {
        private readonly CommandArguments _args;
        private readonly EngineState _state;

        public RunCommand(CommandArguments args, EngineState state)
        {
            _args = args;
            _state = state;
        }

        public void Execute()
        {
            while (true)
            {
                var arg = _args.ShiftNextPositional();
                if (arg == null || arg is MissingArgument)
                    return;
                _state.AddCommand(arg.Value);
            }
        }
    }
}