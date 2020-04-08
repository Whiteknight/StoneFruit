using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution
{
    public class EngineStateCommandQueue
    {
        private readonly LinkedList<CommandOrString> _additionalCommands;

        public EngineStateCommandQueue()
        {
            _additionalCommands = new LinkedList<CommandOrString>();
        }

        public void Append(string command)
        {
            _additionalCommands.AddLast(command);
        }

        public void Append(Command command)
        {
            _additionalCommands.AddLast(command);
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
                _additionalCommands.AddFirst(list[i]);
        }

        public void Prepend(IEnumerable<CommandOrString> commands)
        {
            var list = commands.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
                _additionalCommands.AddFirst(list[i]);
        }

        public void Prepend(string command)
        {
            _additionalCommands.AddFirst(command);
        }

        public void Prepend(Command command)
        {
            _additionalCommands.AddFirst(command);
        }

        public CommandOrString GetNext()
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