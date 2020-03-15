using System.Collections.Generic;

namespace StoneFruit.Execution
{
    /// <summary>
    /// A script of commands which will be executed in sequence, in response to different Engine events.
    /// </summary>
    public class EventScript
    {
        private readonly string[] _initialLines;
        private readonly List<string> _lines;

        public EventScript(params string[] initialLines)
        {
            _initialLines = initialLines;
            _lines = new List<string>(initialLines);
        }

        public void Reset()
        {
            _lines.Clear();
            _lines.AddRange(_initialLines);
        }

        public void Clear() => _lines.Clear();

        public void Add(params string[] lines) => _lines.AddRange(lines);

        public IEnumerable<string> GetCommands() => _lines;

        public override string ToString() => string.Join("\n", _lines);
    }
}