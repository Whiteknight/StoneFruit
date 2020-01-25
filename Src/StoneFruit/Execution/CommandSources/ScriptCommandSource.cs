using System.Linq;

namespace StoneFruit.Execution.CommandSources
{
    public class ScriptCommandSource : ICommandSource
    {
        private readonly string[] _script;
        private int _index;

        public ScriptCommandSource(EventScript script)
        {
            _script = script.GetCommands().ToArray();
            
        }

        public void Start()
        {
            _index = 0;
        }

        public string GetNextCommand()
        {
            if (_index >= _script.Length)
                return null;
            return _script[_index++];
        }
    }
}