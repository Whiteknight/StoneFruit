using System;

namespace StoneFruit.BuiltInVerbs
{
    [CommandDetails(EnvironmentListCommand.Name)]
    public class EnvironmentListCommand : ICommandVerb
    {
        public const string Name = "env-list";

        private readonly IEnvironmentCollection _environments;
        private readonly ITerminalOutput _output;

        public EnvironmentListCommand(IEnvironmentCollection environments, ITerminalOutput output)
        {
            _environments = environments;
            _output = output;
        }

        public static string Description => "Lists all available environments";

        public void Execute()
        {
            foreach (var env in _environments.GetNames())
            {
                _output.Write(ConsoleColor.White, env.Key.ToString());
                _output.Write(ConsoleColor.DarkGray, ") ");
                _output.Write(ConsoleColor.Cyan, env.Value);
                if (env.Value == _environments.CurrentName)
                    _output.Write(ConsoleColor.White, "*");
                _output.WriteLine();
            }
        }
    }
}