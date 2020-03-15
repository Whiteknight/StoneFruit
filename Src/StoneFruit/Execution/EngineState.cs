using System.Collections.Generic;
using System.Linq;

namespace StoneFruit.Execution
{
    /// <summary>
    /// The runtime state of the engine. Controls the execution of the
    /// engine and contains data which persists between command executions
    /// </summary>
    public class EngineState
    {
        private readonly LinkedList<string> _additionalCommands;
        private readonly Dictionary<string, object> _metadata;

        public EngineState(bool headless, EngineEventCatalog eventCatalog)
        {
            Headless = headless;
            EventCatalog = eventCatalog;
            ShouldExit = false;
            _additionalCommands = new LinkedList<string>();
            _metadata = new Dictionary<string, object>();
        }

        public bool ShouldExit { get; private set; }
        public int ExitCode { get; private set; }
        public bool Headless { get; }
        public EngineEventCatalog EventCatalog { get; }

        public void Exit(int exitCode = 0)
        {
            ShouldExit = true;
            ExitCode = exitCode;
        }

        // TODO: break this out into objects. state.Commands.Add|Remove|Prepend(), state.Metadata.Add|Get() etc

        public void AddCommand(string command)
        {
            _additionalCommands.AddLast(command);
        }

        public void AddCommands(IEnumerable<string> commands)
        {
            foreach (var command in commands)
                AddCommand(command);
        }

        public void PrependCommands(IEnumerable<string> commands)
        {
            var list = commands.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
                _additionalCommands.AddFirst(list[i]);
        }

        public void PrependCommand(string command)
        {
            _additionalCommands.AddFirst(command);
        }

        public string GetNextCommand()
        {
            if (_additionalCommands.Count == 0)
                return null;
            var next = _additionalCommands.First.Value;
            _additionalCommands.RemoveFirst();
            return next;
        }

        public void ClearAdditionalCommands() => _additionalCommands.Clear();

        public void AddMetadata(string name, object value, bool allowOverwrite = true)
        {
            if (_metadata.ContainsKey(name))
            {
                if (!allowOverwrite)
                    return;
                _metadata.Remove(name);
            }

            _metadata.Add(name, value);
        }

        public object GetMetadata(string name)
        {
            if (!_metadata.ContainsKey(name))
                return null;
            return _metadata[name];
        }

        public void RemoveMetadata(string name)
        {
            if (_metadata.ContainsKey(name))
                _metadata.Remove(name);
        }

        // TODO: Configurable loop limit so we don't keep adding commands to the queue in an endless loop
    }
}
