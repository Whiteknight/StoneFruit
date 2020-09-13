using System;
using StoneFruit.Execution.Output;

namespace StoneFruit.Handlers
{
    [Verb(Name)]
    public class EnvironmentListHandler : IHandler
    {
        public const string Name = "env-list";

        private readonly IEnvironmentCollection _environments;
        private readonly IOutput _output;

        public EnvironmentListHandler(IEnvironmentCollection environments, IOutput output)
        {
            _environments = environments;
            _output = output;
        }

        public static string Group => HelpHandler.BuiltinsGroup;
        public static string Description => "Lists all available environments";

        public void Execute()
        {
            var highlight = new Brush(ConsoleColor.Black, ConsoleColor.Cyan);
            var envList = _environments.GetNames();
            for (int i = 0; i < envList.Count; i++)
            {
                var index = i + 1;
                var env = envList[i];

                _output
                    .Color(ConsoleColor.White).Write(index.ToString())
                    .Color(ConsoleColor.DarkGray).Write(") ")
                    .Color(env == _environments.CurrentName ? highlight : ConsoleColor.Cyan).Write(env)
                    .WriteLine();
            }
        }
    }
}