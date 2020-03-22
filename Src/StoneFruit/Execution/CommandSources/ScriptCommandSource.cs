using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.CommandSources
{
    public class ScriptCommandSource : ICommandSource
    {
        private readonly CommandObjectOrString[] _script;
        private int _index;

        public ScriptCommandSource(EventScript script, CommandParser parser, params IArgument[] args)
            :this (script, parser, new CommandArguments(args))
        {
            
        }

        public ScriptCommandSource(EventScript script, CommandParser parser, CommandArguments args)
        {
            _script = script.GetCommands(parser, args).ToArray();
        }

        public void Start()
        {
            _index = 0;
        }

        public CommandObjectOrString GetNextCommand()
        {
            if (_index >= _script.Length)
                return null;
            return _script[_index++];
        }
    }
}