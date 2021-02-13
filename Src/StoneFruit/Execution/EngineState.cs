using System;
using System.Threading;
using StoneFruit.Utility;

namespace StoneFruit.Execution
{
    /// <summary>
    /// The runtime state of the engine. Controls the execution of the engine and contains
    /// data which persists between command executions.
    /// </summary>
    public class EngineState
    {
        private IArguments? _arguments;

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
            CommandCounter = Headless ?
                new HeadlessEngineStateCommandCounter(Commands, eventCatalog, Settings) :
                (IEngineStateCommandCounter)new InteractiveEngineStateCommandCounter(Commands, Settings);
        }

        public bool ShouldExit { get; private set; }
        public int ExitCode { get; private set; }
        public bool Headless { get; }
        public EngineEventCatalog EventCatalog { get; }
        public EngineStateCommandQueue Commands { get; }
        public EngineStateMetadataCache Metadata { get; }
        public IEngineStateCommandCounter CommandCounter { get; set; }
        public EngineSettings Settings { get; set; }
        public IArguments CurrentArguments => _arguments ?? throw new InvalidOperationException("Attempt to access IArguments when there are no current arguments set");

        public void SetCurrentArguments(IArguments arguments)
        {
            _arguments = arguments;
        }

        public void ClearCurrentArguments()
        {
            _arguments = null;
        }

        /// <summary>
        /// Signal the runloop that it should exit immediately and stop executing commands.
        /// </summary>
        /// <param name="exitCode"></param>
        public void Exit(int exitCode = Constants.ExitCodeOk)
        {
            ShouldExit = true;
            ExitCode = exitCode;
        }

        /// <summary>
        /// Gets a CancellationTokenSource configured with settings values, to use for
        /// dispatching commands
        /// </summary>
        /// <returns></returns>
        public CancellationTokenSource GetConfiguredCancellationSource()
        {
            var tokenSource = new CancellationTokenSource();
            var timeout = Settings.MaxExecuteTimeout;
            if (timeout < TimeSpan.MaxValue)
                tokenSource.CancelAfter(timeout);
            return tokenSource;
        }
    }
}
