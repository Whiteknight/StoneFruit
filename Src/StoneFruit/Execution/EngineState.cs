using System.Collections;
using System.Collections.Generic;

namespace StoneFruit.Execution
{
    public class EngineState
    {
        private readonly Queue<string> _additionalCommands;

        public EngineState(bool headless)
        {
            Headless = headless;
            ShouldExit = false;
            _additionalCommands = new Queue<string>();
        }

        public bool ShouldExit { get; set; }
        public bool Headless { get; }

        public void AddCommand(string command)
        {
            _additionalCommands.Enqueue(command);
        }

        public string GetNextCommand()
        {
            if (_additionalCommands.Count == 0)
                return null;
            return _additionalCommands.Dequeue();
        }

        public void ClearAdditionalCommands()
        {
            _additionalCommands.Clear();
        }

        // TODO: Configurable loop limit so we don't keep adding commands to the queue in an endless loop

        // TODO: Some kind of metadata mechanism so verbs can store metadata here in the state and retrieve it later
        // TODO: Exit code so we can force the application to exit with a specific code for shell scripting purposes

    }
}