using System;

namespace StoneFruit.Execution.CommandSources
{
    public class PromptCommandSource : ICommandSource
    {
        private readonly ITerminalOutput _output;
        private readonly IEnvironmentCollection _environments;

        public PromptCommandSource(ITerminalOutput output, IEnvironmentCollection environments)
        {
            _output = output;
            _environments = environments;
        }

        public void Start()
        {
            _output
                .Write("Enter command ")
                .Color(ConsoleColor.DarkGray).Write("('help' for help, 'exit' to quit)")
                .Color(ConsoleColor.Gray).WriteLine(":");
        }

        public string GetNextCommand()
        {
            return _output.Prompt($"{_environments.CurrentName}");
        }
    }
}