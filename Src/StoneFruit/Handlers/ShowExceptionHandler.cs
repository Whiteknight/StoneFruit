using System;
using StoneFruit.Execution;

namespace StoneFruit.Handlers
{
    [Verb(Name, false)]
    public class ShowExceptionHandler : IHandler
    {
        public const string Name = "showerror";

        private readonly EngineState _state;
        private readonly IOutput _output;

        public ShowExceptionHandler(EngineState state, IOutput output)
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
