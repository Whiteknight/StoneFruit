namespace StoneFruit.Execution
{
    /// <summary>
    /// The runtime state of the engine. Controls the execution of the
    /// engine and contains data which persists between command executions
    /// </summary>
    public class EngineState
    {
        public EngineState(bool headless, EngineEventCatalog eventCatalog)
        {
            Headless = headless;
            EventCatalog = eventCatalog;
            ShouldExit = false;
            
            Commands = new EngineStateCommandQueue();
            Metadata = new EngineStateMetadataCache();
        }

        public bool ShouldExit { get; private set; }
        public int ExitCode { get; private set; }
        public bool Headless { get; }
        public EngineEventCatalog EventCatalog { get; }
        public EngineStateCommandQueue Commands { get; }
        public EngineStateMetadataCache Metadata { get; }

        // TODO: Some kind of global settings mechanism where the user can "set name value" and affect usage

        public void Exit(int exitCode = 0)
        {
            ShouldExit = true;
            ExitCode = exitCode;
        }

        // TODO: Configurable loop limit so we don't keep adding commands to the queue in an endless loop
    }
}
