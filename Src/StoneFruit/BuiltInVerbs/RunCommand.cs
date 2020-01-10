using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.BuiltInVerbs
{
    public class RunCommand : ICommandVerb
    {
        private readonly CommandArguments _args;
        private readonly EngineState _state;

        public RunCommand(CommandArguments args, EngineState state)
        {
            _args = args;
            _state = state;
        }

        public static string Description => "Run a single command or a list of commands";

        public static string Help => @"run ...
Allows passing a list of commands to be executed sequentially. Use quotes to group commands and their
arguments. For example:

    run help 'change-env MyEnv' ...
";

        public void Execute()
        {
            while (true)
            {
                var arg = _args.Shift();
                if (arg == null || arg is MissingArgument)
                    return;
                _state.AddCommand(arg.Value);
            }
        }
    }
}