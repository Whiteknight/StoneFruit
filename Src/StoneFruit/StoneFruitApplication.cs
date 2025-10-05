using System.Threading.Tasks;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.CommandSources;
using StoneFruit.Execution.Environments;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

/// <summary>
/// The execution core. Provides a run loop to receive user commands and execute them.
/// </summary>
public class StoneFruitApplication
{
    private readonly ICommandLine _cmdLineArgs;
    private readonly CommandSourcesBuilder _sources;
    private readonly EngineState _state;
    private readonly IEnvironments _environments;
    private readonly Engine _engine;

    public StoneFruitApplication(
        IEnvironments environments,
        EngineState state,
        ICommandLine cmdLineArgs,
        CommandSourcesBuilder sources,
        Engine engine
    )
    {
        _environments = NotNull(environments);
        _cmdLineArgs = NotNull(cmdLineArgs);
        _state = NotNull(state);
        _sources = NotNull(sources);
        _engine = engine;
    }

    /// <summary>
    /// Selects the appropriate run mode and executes it based on the raw command line
    /// arguments passed to the application. If command line arguments are provided,
    /// they are executed in headless mode and the application will exit. If no
    /// arguments are provided the application will run in interactive mode.
    /// </summary>
    /// <returns></returns>
    public Task<int> RunWithCommandLineArgumentsAsync()
    {
        var commandLine = _cmdLineArgs.GetRawArguments();
        return RunAsync(commandLine);
    }

    public int RunWithCommandLineArguments()
    {
        return Task.Run(async () =>
        {
            var commandLine = _cmdLineArgs.GetRawArguments();
            return await RunAsync(commandLine);
        }).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Selects the appropriate run mode and executes it. If a command is provided, the
    /// application is inferred to be in headless mode. If no arguments are provided
    /// the application is inferred to be in interactive mode.
    /// </summary>
    /// <param name="commandLine"></param>
    public Task<int> RunAsync(string commandLine)
    {
        // If there are no arguments, enter interactive mode
        if (string.IsNullOrEmpty(commandLine))
            return RunInteractivelyAsync();

        // if there is exactly one argument and it's the name of a valid environment,
        // start interactive mode setting that environment first.
        if (_environments.IsValid(commandLine))
            return RunInteractivelyAsync(commandLine);

        // Otherwise run in headless mode and figure it out from there.
        return RunHeadlessAsync(commandLine);
    }

    public int Run(string commandLine)
        => Task.Run(async () => await RunAsync(commandLine))
            .GetAwaiter()
            .GetResult();

    /// <summary>
    /// Run the application in headless mode with the raw commandline arguments then
    /// exits the application.
    /// </summary>
    /// <returns></returns>
    public Task<int> RunHeadlessWithCommandLineArgsAsync()
    {
        var commandLine = _cmdLineArgs.GetRawArguments();
        return RunHeadlessAsync(commandLine);
    }

    public int RunHeadlessWithCommandLineArgs()
    {
        return Task.Run(async () =>
        {
            var commandLine = _cmdLineArgs.GetRawArguments();
            return await RunHeadlessAsync(commandLine);
        }).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Run the application in headless mode with the provided commandline string.
    /// </summary>
    /// <param name="commandLine"></param>
    /// <returns></returns>
    public async Task<int> RunHeadlessAsync(string commandLine)
    {
        _state.SetRunMode(EngineRunMode.Headless);

        // If we have a single argument "help", run the help script and exit. We don't
        // require a valid environment to run help
        if (commandLine == "help")
        {
            _state.OnHeadlessHelp();
            return await _engine.RunLoop(_sources.Build());
        }

        // Now see if the first argument is the name of an environment. If so, switch
        // to that environment and continue
        (var startingEnvironment, commandLine) = GetStartingEnvironment(commandLine);

        // If there is no commandline left, run the HeadlessNoArgs script
        if (string.IsNullOrWhiteSpace(commandLine))
        {
            _state.OnHeadlessNoArgs();
            return await _engine.RunLoop(_sources.Build());
        }

        // Setup the Headless start script, an environment change command if any, the
        // user command, and the headless stop script
        _sources.AddToEnd(_state.EventCatalog.EngineStartHeadless, SyntheticArguments.Empty);
        if (startingEnvironment.Is(env => !string.IsNullOrEmpty(env)))
            _sources.AddToEnd($"{EnvironmentHandler.Name} '{startingEnvironment.GetValueOrThrow()}'");
        _sources
            .AddToEnd(commandLine)
            .AddToEnd(_state.EventCatalog.EngineStopHeadless, SyntheticArguments.Empty);

        return await _engine.RunLoop(_sources.Build());
    }

    public int RunHeadless(string commandLine)
        => Task.Run(async () => await RunHeadlessAsync(commandLine))
            .GetAwaiter()
            .GetResult();

    public int RunInteractively()
        => Task.Run(async () => await RunInteractivelyAsync(null))
            .GetAwaiter()
            .GetResult();

    public int RunInteractively(string? environment)
        => Task.Run(async () => await RunInteractivelyAsync(environment))
            .GetAwaiter()
            .GetResult();

    /// <summary>
    /// Runs interactively, prompting the user for input and executing each command in
    /// turn. If an environment is not set, the user is prompted to select one before
    /// any commands are executed. Returns when the user has entered the 'exit' or
    /// 'quit' commands, or when some other verb has set the exit condition.
    /// </summary>
    public Task<int> RunInteractivelyAsync() => RunInteractivelyAsync(null);

    /// <summary>
    /// Runs interactively, setting the environment to the value given and then
    /// prompting the user for commands to execute. Returns when the user has entered
    /// the 'exit' or 'quit' commands, or when some other verb has set the exit
    /// condition.
    /// </summary>
    /// <param name="environment"></param>
    public async Task<int> RunInteractivelyAsync(string? environment)
    {
        _state.SetRunMode(EngineRunMode.Interactive);

        // Change the environment if necessary. Otherwise the EngineStartInteractive
        // script will probably prompt the user to do so.
        if (!string.IsNullOrEmpty(environment))
            _sources.AddToEnd($"{EnvironmentHandler.Name} '{environment}'");

        _sources
            .AddToEnd(_state.EventCatalog.EngineStartInteractive, SyntheticArguments.Empty)
            .AddPromptToEnd();

        return await _engine.RunLoop(_sources.Build());
    }

    // See if the given commandLine starts with a valid environment name. If so,
    // extract the environment name from the front and return the remainder of the
    // commandline.
    private (Maybe<string> StartingEnvironment, string CommandLine) GetStartingEnvironment(string commandLine)
    {
        var validEnvironments = _environments.GetNames();
        if (validEnvironments.Count <= 1)
            return (default, commandLine);

        var parts = commandLine.Split(Constants.SeparatedBySpace, 2);
        var env = parts[0];
        return _environments.IsValid(env)
            ? (new Maybe<string>(env), parts[1])
            : (default, commandLine);
    }
}
