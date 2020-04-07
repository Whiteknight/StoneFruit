using StoneFruit.Utility;

namespace StoneFruit.Execution
{
    /// <summary>
    /// The runtime state of the engine. Controls the execution of the
    /// engine and contains data which persists between command executions
    /// </summary>
    public class EngineState
    {
        public EngineState(bool headless, EngineEventCatalog eventCatalog, EngineSettings settings)
        {
            Assert.ArgumentNotNull(eventCatalog, nameof(eventCatalog));
            Assert.ArgumentNotNull(settings, nameof(settings));

            Headless = headless;
            EventCatalog = eventCatalog;
            ShouldExit = false;

            Settings = settings;
            Commands = new EngineStateCommandQueue();
            Metadata = new EngineStateMetadataCache();
            CommandCounter = Headless ? new HeadlessEngineStateCommandCounter(Commands, eventCatalog, Settings) : (IEngineStateCommandCounter)new InteractiveEngineStateCommandCounter(Commands, Settings);
        }

        public bool ShouldExit { get; private set; }
        public int ExitCode { get; private set; }
        public bool Headless { get; }
        public EngineEventCatalog EventCatalog { get; }
        public EngineStateCommandQueue Commands { get; }
        public EngineStateMetadataCache Metadata { get; }
        public IEngineStateCommandCounter CommandCounter { get; set; }
        public EngineSettings Settings { get; set; }

        public void Exit(int exitCode = 0)
        {
            ShouldExit = true;
            ExitCode = exitCode;
        }
    }
}
