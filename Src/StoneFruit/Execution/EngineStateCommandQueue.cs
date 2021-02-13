using System.Collections.Generic;
using System.Linq;

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

        public void Append(ArgumentsOrString argsOrString)
        {
            _additionalCommands.AddLast(argsOrString);
        }

        public void Append(IEnumerable<ArgumentsOrString> argsOrStrings)
        {
            foreach (var argsOrString in argsOrStrings)
                Append(argsOrString);
        }

        public void Prepend(IEnumerable<ArgumentsOrString> argsOrStrings)
        {
            var list = argsOrStrings.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
                _additionalCommands.AddFirst(list[i]);
        }

        public void Prepend(ArgumentsOrString argsOrString)
        {
            _additionalCommands.AddFirst(argsOrString);
        }

        public IResult<ArgumentsOrString> GetNext()
        {
            if (_additionalCommands.Count == 0)
                return FailureResult<ArgumentsOrString>.Instance;
            ArgumentsOrString next = _additionalCommands!.First!.Value;
            _additionalCommands.RemoveFirst();
            return new SuccessResult<ArgumentsOrString>(next);
        }

        public void Clear() => _additionalCommands.Clear();
    }
}
