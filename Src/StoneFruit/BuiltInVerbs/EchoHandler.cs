using System;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.BuiltInVerbs
{
    [Verb(Name, showInHelp: false)]
    public class EchoHandler : IHandler
    {
        public const string Name = "echo";

        private readonly IOutput _output;
        private readonly CommandArguments _args;

        public EchoHandler(IOutput output, CommandArguments args)
        {
            _output = output;
            _args = args;
        }

        public static string Description => "Writes a string of output to the console";

        public static string Usage => @"echo [color=<color>] ...

Writes each argument to the output as a new line. If color is specified, use that color
";

        public void Execute()
        {
            var output = _output;
            var colorName = _args.Get("color").AsString();
            if (!string.IsNullOrEmpty(colorName))
            {
                var color = (ConsoleColor) Enum.Parse(typeof(ConsoleColor), colorName);
                output = output.Color(color);
            }

            foreach (var arg in _args.GetAllPositionals())
                output.WriteLine(arg.Value);
        }
    }
}
