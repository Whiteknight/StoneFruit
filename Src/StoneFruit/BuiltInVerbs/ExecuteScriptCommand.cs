using System.IO;
using System.Linq;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.BuiltInVerbs
{
    [CommandName("exec")]
    public class ExecuteScriptCommand : ICommandVerb
    {
        private readonly CommandArguments _args;
        private readonly EngineState _state;

        public ExecuteScriptCommand(CommandArguments args, EngineState state)
        {
            _args = args;
            _state = state;
        }

        public static string Description => "Executes a script containing commands, one per line";

        public static string Help => @"exec <fileName>
Loads the contents of the file and treats each line as a separate command to execute in sequence.";

        public void Execute()
        {
            var scriptName = _args.Shift().Require().Value;
            // TODO: Some kind of comment syntax? Maybe we can cover that in the command parser?
            // TODO: Error-handling if we don't have the file?
            var contents = File.ReadAllLines(scriptName).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            foreach (var line in contents)
                _state.AddCommand(line);
        }
    }
}
