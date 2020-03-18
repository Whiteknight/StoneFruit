using System;
using StoneFruit.Execution;

namespace StoneFruit.Handlers
{
    [Verb(NameHeadlessPreamble, showInHelp: false)]
    public class BuiltinMessageHandler : IHandler
    {
        public const string NameHeadlessPreamble = "builtin-headlesspreamble";

        private readonly IOutput _output;
        private readonly Command _command;

        public BuiltinMessageHandler(IOutput output, Command command)
        {
            _output = output;
            _command = command;
        }

        public static string Description => "Shows some built-in messages";

        public void Execute()
        {
            if (_command.Verb == NameHeadlessPreamble)
            {
                _output
                    .Write("Enter command ")
                    .Color(ConsoleColor.DarkGray).Write("('help' for help, 'exit' to quit)")
                    .Color(ConsoleColor.Gray).WriteLine(":");
            }
        }
    }
}
