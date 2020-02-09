using System;

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

        public static string Description => "Lists all available environments";

        public void Execute()
        {
            var highlight = new Brush(ConsoleColor.Black, ConsoleColor.Cyan);
            foreach (var env in _environments.GetNames())
            {
                _output
                    .Color(ConsoleColor.White).Write(env.Key.ToString())
                    .Color(ConsoleColor.DarkGray).Write(") ");
                if (env.Value == _environments.CurrentName)
                    _output.Color(highlight).Write(env.Value);
                else
                    _output.Color(ConsoleColor.Cyan).Write(env.Value);

                _output.WriteLine();
            }
        }
    }
}