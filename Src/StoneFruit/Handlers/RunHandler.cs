using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Handlers
{
    public class RunHandler : IHandler
    {
        private readonly CommandArguments _args;
        private readonly EngineState _state;

        public RunHandler(CommandArguments args, EngineState state)
        {
            _args = args;
            _state = state;
        }

        public static string Description => "Run a single command or a list of commands";

        public static string Usage => @"run <command>+

Allows passing a list of commands to be executed sequentially. Use quotes to group commands and their
arguments. For example:

    run help 'change-env MyEnv' ...
";

        public void Execute()
        {
            while (true)
            {
                var arg = _args.Shift();
                if (!arg.Exists())
                    return;
                _state.AddCommand(arg.Value);
            }
        }
    }
}