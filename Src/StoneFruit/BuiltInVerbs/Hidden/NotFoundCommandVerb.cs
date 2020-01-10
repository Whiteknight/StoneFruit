using System;

namespace StoneFruit.BuiltInVerbs.Hidden
{
    /// <summary>
    /// Internal Null Object implementation of ICommandVerb. Not intended for normal use
    /// </summary>
    public class NotFoundCommandVerb : ICommandVerb
    {
        private readonly string _verb;
        private readonly ITerminalOutput _output;

        public NotFoundCommandVerb(string verb, ITerminalOutput output)
        {
            _verb = verb;
            _output = output;
        }

        public void Execute()
        {
            _output.WriteLine(ConsoleColor.Red, $"Command '{_verb}' not found. Please check your spelling or help output and try again");
        }
    }
}