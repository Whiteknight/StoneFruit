using System;
using System.Linq;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Handlers
{
    [Verb(Name, Hide = true)]
    public class EchoHandler : IHandler
    {
        public const string Name = "echo";

        private readonly EngineState _state;
        private readonly IOutput _output;
        private readonly IArguments _args;

        public EchoHandler(EngineState state, IOutput output, IArguments args)
        {
            _state = state;
            _output = output;
            _args = args;
        }

        public static string Group => HelpHandler.BuiltinsGroup;
        public static string Description => "Writes a string of output to the console";

        public static string Usage => @"echo [color=<color>] [-nonewline] [-noheadless] ...

Writes all positional arguments to the output. If color is specified, use that color.
Appends a new-line to the end unless -nonewline is specified
-noheadless causes the handler to not output any text in headless mode
";

        public void Execute()
        {
            // Some messages, especially internal ones, don't want to display in headless
            if (_args.HasFlag("noheadless") && _state.Headless)
                return;

            var output = _output;
            var colorName = _args.Get("color").AsString();
            if (!string.IsNullOrEmpty(colorName))
            {
                var color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colorName);
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
