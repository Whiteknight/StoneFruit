using System.IO;
using System.Linq;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Handlers
{
    [Verb("exec", showInHelp: false)]
    public class ExecuteScriptHandler : IHandler
    {
        private readonly IArguments _args;
        private readonly EngineState _state;

        public ExecuteScriptHandler(IArguments args, EngineState state)
        {
            _args = args;
            _state = state;
        }

        public static string Group => HelpHandler.BuiltinsGroup;
        public static string Description => "Executes a script containing commands, one per line";

        public static string Usage => @"exec <fileName>

Loads the contents of the file and treats each line as a separate command to execute in sequence.";

        public void Execute()
        {
            var scriptName = _args.Shift().Require().AsString();
            if (string.IsNullOrEmpty(scriptName))
                throw new ExecutionException("Must provide a file name to execute");
            if (!File.Exists(scriptName))
                throw new ExecutionException($"File {scriptName} does not exist");

            var contents = File.ReadAllLines(scriptName)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();
            foreach (var line in contents)
                _state.Commands.Append(line);
        }
    }
}
