using StoneFruit.Execution.Arguments;

namespace StoneFruit.BuiltInVerbs
{
    [CommandName(Name, showInHelp: false)]
    public class EchoVerb : ICommandVerb
    {
        public const string Name = "echo";

        private readonly ITerminalOutput _output;
        private readonly CommandArguments _args;

        public EchoVerb(ITerminalOutput output, CommandArguments args)
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
