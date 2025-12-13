using System;
using System.Threading;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.CommandCounting;
using StoneFruit.Execution.Metadata;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution;

/// <summary>
/// The runtime state of the engine. Controls the execution of the engine and contains
/// data which persists between command executions.
/// </summary>
public class EngineState
{
    private readonly IInput _input;
    private IArguments? _arguments;
    private HandlerContext? _handlerContext;

    public EngineState(
        EngineEventCatalog eventCatalog,
        EngineSettings settings,
        ICommandParser parser,
        IInput input)
    {
        EventCatalog = NotNull(eventCatalog);
        ShouldExit = false;

        Settings = NotNull(settings);
        _input = input;
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

    public ExitCode ExitCode { get; private set; }

    public EngineRunMode RunMode { get; private set; }

    public IEngineStateCommandCounter CommandCounter { get; private set; }

    public IArguments CurrentArguments
        => _arguments
            ?? throw new InvalidOperationException("Cannot access current arguments, because one has not been set.");

    public HandlerContext HandlerContext
        => _handlerContext ?? throw new InvalidOperationException("Cannot access handler context, because one has not been set.");

    public void SetRunMode(EngineRunMode runMode)
    {
        RunMode = runMode;
        CommandCounter = runMode switch
        {
            EngineRunMode.Headless => new HeadlessEngineStateCommandCounter(Commands, Metadata, Settings, this),
            EngineRunMode.Interactive => new InteractiveEngineStateCommandCounter(Commands, Metadata, Settings, _input),
            _ => new NullCommandCounter()
        };
    }

    public void SetHandlerExecutionContext(IArguments arguments, HandlerContext context)
    {
        _arguments = arguments;
        _handlerContext = context;
    }

    public void ClearHandlerExecutionContext()
    {
        _arguments = null;
        _handlerContext = null;
    }

    /// <summary>
    /// Signal the runloop that it should exit immediately and stop executing commands.
    /// </summary>
    /// <param name="exitCode"></param>
    public void SignalExit(ExitCode exitCode = default)
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
    public void OnEnvironmentChanged(string env)
    {
        var args = SyntheticArguments.From(("environment", env));
        Commands.Prepend(EventCatalog.EnvironmentChanged, args);
    }

    public void OnHeadlessHelp()
    {
        var args = SyntheticArguments.From(("exitcode", ExitCode.HeadlessHelp.ToString()));
        Commands.Prepend(EventCatalog.HeadlessHelp, args);
    }

    public void OnHeadlessNoArgs()
    {
        var args = SyntheticArguments.From(("exitcode", ExitCode.HeadlessNoVerb.ToString()));
        Commands.Prepend(EventCatalog.HeadlessNoArgs, args);
    }

    public void OnMaximumHeadlessCommands()
    {
        var args = SyntheticArguments.From(
           ("limit", Settings.MaxInputlessCommands.ToString()),
           ("exitcode", ExitCode.MaximumCommands.ToString())
        );
        Commands.Prepend(EventCatalog.MaximumHeadlessCommands, args);
    }
}
