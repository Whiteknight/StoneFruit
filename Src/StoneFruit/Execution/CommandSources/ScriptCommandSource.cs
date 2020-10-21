using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.CommandSources
{
    /// <summary>
    /// An adaptor for EventScript to ICommandSource. Makes commands from a script
    /// available to the runloop.
    /// </summary>
    public class ScriptCommandSource : ICommandSource
    {
        private readonly ArgumentsOrString[] _script;
        private int _index;

        public ScriptCommandSource(EventScript script, ICommandParser parser, params IArgument[] args)
            : this (script, parser, new SyntheticArguments(args))
        {
        }

        public ScriptCommandSource(EventScript script, ICommandParser parser, IArguments args)
        {
            _script = script.GetCommands(parser, args).ToArray();
            _index = 0;
        }

        public ArgumentsOrString GetNextCommand()
        {
            if (_index >= _script.Length)
                return null;
            return _script[_index++];
        }
    }
}