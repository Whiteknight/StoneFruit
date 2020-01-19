using StoneFruit.Execution.Arguments;

namespace StoneFruit.BuiltInVerbs
{
    [CommandName(Name)]
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

        public void Execute()
        {
            foreach (var arg in _args.GetAllPositionals())
                _output.WriteLine(arg.Value);
        }
    }
}
