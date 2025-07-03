using System;
using System.Threading;
using StoneFruit.Execution.Arguments;
using StoneFruit.Utility;

namespace StoneFruit.Execution;

/// <summary>
/// The runtime state of the engine. Controls the execution of the engine and contains
/// data which persists between command executions.
/// </summary>
public class EngineState
{
    private readonly IEnvironmentCollection _environments;

    private IArguments? _arguments;

    public EngineState(EngineEventCatalog eventCatalog, EngineSettings settings, IEnvironmentCollection environments, ICommandParser parser)
    {
        Assert.NotNull(eventCatalog, nameof(eventCatalog));
        Assert.NotNull(settings, nameof(settings));

        EventCatalog = eventCatalog;
        ShouldExit = false;

        Settings = settings;
        _environments = environments;
        Commands = new EngineStateCommandQueue(parser);
        Metadata = new EngineStateMetadataCache();
        RunMode = EngineRunMode.Idle;
        CommandCounter = new NullCommandCounter();
    }

    public EngineEventCatalog EventCatalog { get; }
    public EngineStateCommandQueue Commands { get; }
    public EngineStateMetadataCache Metadata { get; }
    public EngineSettings Settings { get; }

    public bool ShouldExit { get; private set; }
    public int ExitCode { get; private set; }
    public EngineRunMode RunMode { get; private set; }
    public IEngineStateCommandCounter CommandCounter { get; private set; }

    public IArguments CurrentArguments => _arguments ?? throw new InvalidOperationException("Attempt to access IArguments when there are no current arguments set");

    public void SetRunMode(EngineRunMode runMode)
    {
        RunMode = runMode;
        CommandCounter = runMode switch
        {
            EngineRunMode.Headless => new HeadlessEngineStateCommandCounter(Commands, EventCatalog, Settings),
            EngineRunMode.Interactive => new InteractiveEngineStateCommandCounter(Commands, Settings),
            _ => new NullCommandCounter()
        };
    }

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
    public void SignalExit(int exitCode = Constants.ExitCode.Ok)
    {
        ShouldExit = true;
        ExitCode = exitCode;
    }

    /// <summary>
    /// Gets a CancellationTokenSource configured with settings values, to use for
    /// dispatching commands.
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

    /// <summary>
    /// Call when the current environment has been changed. Executes the EnvironmentChanged
    /// script.
    /// </summary>
    public void OnEnvironmentChanged()
    {
        var currentEnvName = _environments.GetCurrentName().GetValueOrDefault("");
        var args = SyntheticArguments.From(("environment", currentEnvName));
        Commands.Prepend(EventCatalog.EnvironmentChanged, args);
    }

    public void OnHeadlessHelp()
    {
        var args = SyntheticArguments.From(("exitcode", Constants.ExitCode.HeadlessHelp.ToString()));
        Commands.Prepend(EventCatalog.HeadlessHelp, args);
    }

    public void OnHeadlessNoArgs()
    {
        var args = SyntheticArguments.From(("exitcode", Constants.ExitCode.HeadlessNoVerb.ToString()));
        Commands.Prepend(EventCatalog.HeadlessNoArgs, args);
    }
}
