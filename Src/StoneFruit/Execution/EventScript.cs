using System.Collections.Generic;
using StoneFruit.Execution.Arguments;

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

        public IEnumerable<CommandObjectOrString> GetCommands(CommandParser parser, IArguments args)
        {
            var commands = new List<CommandObjectOrString>();
            foreach (var line in _lines)
            {
                var format = parser.ParseScript(line);
                var command = format.Format(args);
                commands.Add(CommandObjectOrString.FromObject(command));
            }

            return commands;
        }

        public override string ToString() => string.Join("\n", _lines);
    }
}