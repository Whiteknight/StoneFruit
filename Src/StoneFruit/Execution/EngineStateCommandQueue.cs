using System.Collections.Generic;
using System.Linq;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution
{
    /// <summary>
    /// Queue of prepared commands for the engine to execute
    /// </summary>
    public class EngineStateCommandQueue
    {
        private readonly LinkedList<ArgumentsOrString> _additionalCommands;

        public EngineStateCommandQueue()
        {
            _additionalCommands = new LinkedList<ArgumentsOrString>();
        }

        public void Append(string command)
        {
            _additionalCommands.AddLast(command);
        }

        public void Append(IArguments arguments)
        {
            _additionalCommands.AddLast(new ArgumentsOrString(arguments));
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

        public void Prepend(IEnumerable<ArgumentsOrString> commands)
        {
            var list = commands.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
                _additionalCommands.AddFirst(list[i]);
        }

        public void Prepend(string command)
        {
            _additionalCommands.AddFirst(command);
        }

        public void Prepend(IArguments arguments)
        {
            _additionalCommands.AddFirst(new ArgumentsOrString(arguments));
        }

        public ArgumentsOrString GetNext()
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