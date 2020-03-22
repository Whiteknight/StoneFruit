using System;
using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Handlers
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

        public static string Usage => @"echo [color=<color>] [-nonewline] ...

Writes all positional arguments to the output. If color is specified, use that color.
Appends a new-line to the end unless -nonewline is specified
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

            var strings = _args.GetAllPositionals().Where(p => p.Exists()).Select(p => p.AsString());
            var line = string.Join(" ", strings);

            if (_args.HasFlag("nonewline"))
                output.Write(line);
            else
                output.WriteLine(line);
        }
    }
}
