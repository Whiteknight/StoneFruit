using System;
using System.Threading.Tasks;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.CommandSources;
using StoneFruit.Execution.Environments;
using StoneFruit.Execution.Exceptions;
using StoneFruit.Handlers;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

/// <summary>
/// The execution core. Provides a run loop to receive user commands and execute them.
/// </summary>
public class Engine
{
    private readonly ICommandParser _parser;
    private readonly ICommandLine _cmdLineArgs;
    private readonly IInput _input;

    public Engine(
        IHandlers handlers,
        IEnvironments environments,
        ICommandParser parser,
        IOutput output,
        IInput input,
        EngineEventCatalog eventCatalog,
        EngineSettings settings,
        ICommandLine cmdLineArgs
    )
    {
        Environments = NotNull(environments);
        _parser = NotNull(parser);
        Output = NotNull(output);
        _input = NotNull(input);
        _cmdLineArgs = NotNull(cmdLineArgs);
        State = new EngineState(eventCatalog, settings, Environments, _parser, _input);
        Dispatcher = new CommandDispatcher(_parser, handlers, Environments, State, Output);
    }

    public EngineState State { get; }

    /// <summary>
    /// Gets the set of configured environments.
    /// </summary>
    public IEnvironments Environments { get; }

    /// <summary>
    /// Gets the configured output channel.
    /// </summary>
    public IOutput Output { get; }

    public CommandDispatcher Dispatcher { get; }

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
        if (Environments.IsValid(commandLine))
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
        State.SetRunMode(EngineRunMode.Headless);
        var sources = new CommandSourceCollection();

        // If we have a single argument "help", run the help script and exit. We don't
        // require a valid environment to run help
        if (commandLine == "help")
        {
            State.OnHeadlessHelp();
            return await RunLoop(sources);
        }

        // Now see if the first argument is the name of an environment. If so, switch
        // to that environment and continue
        (var startingEnvironment, commandLine) = GetStartingEnvironment(commandLine);

        // If there is no commandline left, run the HeadlessNoArgs script
        if (string.IsNullOrWhiteSpace(commandLine))
        {
            State.OnHeadlessNoArgs();
            return await RunLoop(sources);
        }

        // Setup the Headless start script, an environment change command if any, the
        // user command, and the headless stop script
        sources.AddToEnd(State.EventCatalog.EngineStartHeadless, _parser);
        if (startingEnvironment.Is(env => !string.IsNullOrEmpty(env)))
            sources.AddToEnd($"{EnvironmentHandler.Name} '{startingEnvironment.GetValueOrThrow()}'");
        sources.AddToEnd(commandLine);
        sources.AddToEnd(State.EventCatalog.EngineStopHeadless, _parser);

        return await RunLoop(sources);
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
        State.SetRunMode(EngineRunMode.Interactive);
        var source = new CommandSourceCollection();

        // Change the environment if necessary. Otherwise the EngineStartInteractive
        // script will probably prompt the user to do so.
        if (!string.IsNullOrEmpty(environment))
            source.AddToEnd($"{EnvironmentHandler.Name} '{environment}'");

        source.AddToEnd(State.EventCatalog.EngineStartInteractive, _parser);
        source.AddToEnd(new PromptCommandSource(_input, Environments, State.Metadata));

        return await RunLoop(source);
    }

    // See if the given commandLine starts with a valid environment name. If so,
    // extract the environment name from the front and return the remainder of the
    // commandline.
    private (Maybe<string> StartingEnvironment, string CommandLine) GetStartingEnvironment(string commandLine)
    {
        var validEnvironments = Environments.GetNames();
        if (validEnvironments.Count <= 1)
            return (default, commandLine);

        var parts = commandLine.Split(Constants.SeparatedBySpace, 2);
        var env = parts[0];
        return Environments.IsValid(env)
            ? (new Maybe<string>(env), parts[1])
            : (default, commandLine);
    }

    // Pulls commands from the command source until the source is empty or an exit
    // signal is received. Each command is added to the command queue and the queue
    // is drained.
    private async Task<int> RunLoop(CommandSourceCollection sources)
    {
        if (State.RunMode == EngineRunMode.Idle)
            throw new ExecutionException("Cannot run the engine in Idle mode. Must enter Headless or Interactive mode first.");
        try
        {
            Environment.ExitCode = await RunLoopInternal(sources);
            return Environment.ExitCode;
        }
        finally
        {
            State.SetRunMode(EngineRunMode.Idle);
        }
    }

    private async Task<int> RunLoopInternal(CommandSourceCollection sources)
    {
        while (true)
        {
            // Get a command. If we have one in the state use that. Otherwise try to
            // get one from the sources. If null, we're all done so exit
            var command = State.Commands.GetNext()
                .Or(() => sources.GetNextCommand())
                .GetValueOrDefault(ArgumentsOrString.Invalid);
            if (!command.IsValid)
                return Constants.ExitCode.Ok;

            // Check the counter to make sure that we are not in a runaway loop
            // If we are in a loop, the counter will setup the command queue to handle
            // it, so we just continue.
            var canExecute = State.CommandCounter.VerifyCanExecuteNextCommand(_parser);
            if (!canExecute)
                continue;

            await RunOneCommand(command);

            // If exit is signaled, return.
            if (State.ShouldExit)
                return State.ExitCode;
        }
    }

    private async Task RunOneCommand(ArgumentsOrString command)
    {
        try
        {
            // Get a cancellation token source, configured according to state Command
            // settings, and use that to dispatch the command
            using var tokenSource = State.GetConfiguredCancellationSource();

            await Dispatcher.ExecuteAsync(command, tokenSource.Token);
        }
        catch (VerbNotFoundException vnf)
        {
            // The verb was not found. Execute the VerbNotFound script
            var args = SyntheticArguments.From(("verb", vnf.Verb));
            HandleError(vnf, State.EventCatalog.VerbNotFound, args);
        }
        catch (InternalException ie)
        {
            // It is an internal exception type. We want to show the message but we don't need
            // to burden the user with the full stacktrace.
            var args = SyntheticArguments.From(
                ("message", ie.Message),
                ("stacktrace", "")
            );
            HandleError(ie, State.EventCatalog.EngineError, args);
        }
        catch (Exception e)
        {
            // We've received some other error. Execute the EngineError script
            // and hope for the best
            var args = SyntheticArguments.From(
                ("message", e.Message),
                ("stacktrace", e.StackTrace ?? "")
            );
            HandleError(e, State.EventCatalog.EngineError, args);
        }
    }

    // Handle an error from the dispatcher.
    private void HandleError(Exception currentException, EventScript script, IArguments args)
    {
        // When we handle an error, we set the Current Error in the State. When we are done
        // handling it, we clear the error from the State. If we get into HandleError() and
        // there is already a Current Error in the state it means we have gotten into a loop,
        // throwing errors from the error-handling code. Instead of spiralling down forever,
        // we bail out with a very stern message.

        // Check if we already have a current error
        var maybePrevious = State.Metadata.Get(Constants.Metadata.Error);
        if (maybePrevious.IsSuccess && maybePrevious.GetValueOrThrow() is Exception previousException)
        {
            // An exception was thrown while attempting to handle a previous error.
            // This isn't scripted because it's critical error-handling code and we cannot
            // allow yet another exception to be thrown at this point.
            Output
                .Color(ConsoleColor.Red)
                .WriteLine("Received an exception while attempting to handle a previous exception")
                .WriteLine("This is a fatal condition and the engine will exit")
                .WriteLine("Make sure you clear the current exception when you are done handling it to avoid these situations")
                .WriteLine("Current Exception:")
                .WriteLine(currentException.Message)
                .WriteLine(currentException.StackTrace ?? "")
                .WriteLine("Previous Exception:")
                .WriteLine(previousException.Message)
                .WriteLine(previousException.StackTrace ?? "")
                ;
            State.SignalExit(Constants.ExitCode.CascadeError);
            return;
        }

        // Add the current exception to state metadata so we can keep track of loops,
        // then prepend the error-handling script and a command to remove the exception
        // from metadata (prepends happen in reverse order from how they're executed)
        // We can't remove metadata in the script, because users might change the script
        // and inadvertantly break loop detection.
        State.Metadata.Add(Constants.Metadata.Error, currentException, false);
        State.Commands.Prepend($"{MetadataHandler.Name} remove {Constants.Metadata.Error}");
        State.Commands.Prepend(script.GetCommands(_parser, args));
        // Current command queue:
        // 1. Error-handling script
        // 2. "metadata remove __CURRENT_EXCEPTION"
        // 3. previous contents of command queue, if any
    }
}
