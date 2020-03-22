using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution
{
    public class EngineStateCommandQueue
    {
        private readonly LinkedList<CommandObjectOrString> _additionalCommands;

        public EngineStateCommandQueue()
        {
            _additionalCommands = new LinkedList<CommandObjectOrString>();
        }

        public void Append(string command)
        {
            _additionalCommands.AddLast(CommandObjectOrString.FromString(command));
        }

        public void Append(Command command)
        {
            _additionalCommands.AddLast(CommandObjectOrString.FromObject(command));
        }

        public void Append(IEnumerable<string> commands)
        {
            foreach (var command in commands)
                Append(command);
        }

        public void Prepend(IEnumerable<string> commands)
        {
            var list = commands.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
                _additionalCommands.AddFirst(CommandObjectOrString.FromString(list[i]));
        }

        public void Prepend(string command)
        {
            _additionalCommands.AddFirst(CommandObjectOrString.FromString(command));
        }

        public void Prepend(Command command)
        {
            _additionalCommands.AddFirst(CommandObjectOrString.FromObject(command));
        }

        public CommandObjectOrString GetNext()
        {
            if (_additionalCommands.Count == 0)
                return null;
            var next = _additionalCommands.First.Value;
            _additionalCommands.RemoveFirst();
            return next;
        }

        public void Clear() => _additionalCommands.Clear();
    }
}