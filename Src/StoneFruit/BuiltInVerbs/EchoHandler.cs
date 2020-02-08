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

        public void Execute()
        {
            // TODO: Argument to specify color
            // TODO: Argument to specify whether to write a newline or not
            foreach (var arg in _args.GetAllPositionals())
                _output.WriteLine(arg.Value);
        }
    }
}
