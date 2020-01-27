using System;
using StoneFruit.Execution;

namespace StoneFruit.BuiltInVerbs
{
    [CommandName(Name, false)]
    public class ShowExceptionHandler : ICommandHandler
    {
        public const string Name = "showerror";

        private readonly EngineState _state;
        private readonly ITerminalOutput _output;

        public ShowExceptionHandler(EngineState state, ITerminalOutput output)
        {
            _state = state;
            _output = output;
        }

        public void Execute()
        {
            // TODO: Arguments to control whether we show the message, the stacktrace, etc
            var e = _state.GetMetadata(Engine.MetadataError) as Exception;
            if (e == null)
                return;
            _output
                .Color(ConsoleColor.Red)
                .WriteLine(e.Message)
                .WriteLine(e.StackTrace);
        }
    }
}
