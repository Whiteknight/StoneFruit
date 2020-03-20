using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Handlers
{
    [Verb("argument-display", false)]
    public class ArgumentDisplayHandler : IHandler
    {
        private readonly CommandArguments _args;
        private readonly IOutput _output;

        public ArgumentDisplayHandler(CommandArguments args, IOutput output)
        {
            _args = args;
            _output = output;
        }

        public static string Description => "Diagnostic handler to display arguments passed";
        public static string Usage => "argument-display ...";

        public void Execute()
        {
            int index = 0;
            foreach (var p in _args.GetAllPositionals())
            {
                _output.WriteLine($"{index}: {p.Value}");
                index++;
            }

            foreach (var n in _args.GetAllNamed().Cast<NamedArgument>())
                _output.WriteLine($"'{n.Name}': {n.Value}");

            foreach (var f in _args.GetAllFlags().Cast<FlagArgument>())
                _output.WriteLine($"flag: {f.Name}");
        }
    }
}
