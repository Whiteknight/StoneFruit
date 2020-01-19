using System.Collections.Generic;

namespace StoneFruit.Execution
{
    /// <summary>
    /// The runtime state of the engine.
    /// </summary>
    public class EngineState
    {
        private readonly Queue<string> _additionalCommands;

        public EngineState(bool headless, EngineEventCatalog eventCatalog)
        {
            Headless = headless;
            EventCatalog = eventCatalog;
            ShouldExit = false;
            _additionalCommands = new Queue<string>();
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

        public void EngineStartHeadless() => EventCatalog.EngineStartHeadless.EnqueueScript(this);
        public void EngineStartInteractive() => EventCatalog.EngineStartInteractive.EnqueueScript(this);
        public void EngineStopInteractive() => EventCatalog.EngineStopInteractive.EnqueueScript(this);
        public void EngineStopHeadless() => EventCatalog.EngineStopHeadless.EnqueueScript(this);
        public void EnvironmentChanged() => EventCatalog.EnvironmentChanged.EnqueueScript(this);
        public void HeadlessNoArgs() => EventCatalog.HeadlessNoArgs.EnqueueScript(this);
        public void VerbNotFound() => EventCatalog.VerbNotFound.EnqueueScript(this);
    }
}