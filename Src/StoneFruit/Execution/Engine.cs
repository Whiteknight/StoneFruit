using System;
using System.Threading.Tasks;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.CommandSources;
using StoneFruit.Execution.Exceptions;
using StoneFruit.Execution.Metadata;
using StoneFruit.Execution.Scripts;

namespace StoneFruit.Execution;

public sealed class Engine
{
    private readonly EngineState _state;
    private readonly IOutput _output;
    private readonly CommandDispatcher _dispatcher;

    public Engine(EngineState state, IOutput output, CommandDispatcher dispatcher)
    {
        _state = state;
        _output = output;
        _dispatcher = dispatcher;
    }

    // Pulls commands from the command source until the source is empty or an exit
    // signal is received. Each command is added to the command queue and the queue
    // is drained.
    public async Task<int> RunLoop(CommandSourceCollection sources)
    {
        if (_state.RunMode == EngineRunMode.Idle)
            throw new ExecutionException("Cannot run the engine in Idle mode. Must enter Headless or Interactive mode first.");
        try
        {
            var exitCode = await RunLoopInternal(sources);
            exitCode.Set();
            return exitCode.Value;
        }
        finally
        {
            _state.SetRunMode(EngineRunMode.Idle);
        }
    }

    private async Task<ExitCode> RunLoopInternal(CommandSourceCollection sources)
    {
        while (true)
        {
            // Get a command. If we have one in the state use that. Otherwise try to
            // get one from the sources. If null, we're all done so exit
            var command = _state.Commands.GetNext()
                .Or(sources.GetNextCommand)
                .GetValueOrDefault(ArgumentsOrString.Invalid);
            if (!command.IsValid)
                return ExitCode.Ok;

            // Check the counter to make sure that we are not in a runaway loop
            // If we are in a loop, the counter will setup the command queue to handle
            // it, so we just continue.
            var canExecute = _state.CommandCounter.VerifyCanExecuteNextCommand();
            if (!canExecute)
                continue;

            await RunOneCommand(command);

            // If exit is signaled, return.
            if (_state.ShouldExit)
                return _state.ExitCode;
        }
    }

    private async Task RunOneCommand(ArgumentsOrString command)
    {
        try
        {
            // Get a cancellation token source, configured according to state Command
            // settings, and use that to dispatch the command
            using var tokenSource = _state.GetConfiguredCancellationSource();

            await _dispatcher.ExecuteAsync(command, tokenSource.Token);
        }
        catch (VerbNotFoundException vnf)
        {
            // The verb was not found. Execute the VerbNotFound script
            var args = SyntheticArguments.From(("verb", vnf.Verb));
            HandleError(vnf, _state.EventCatalog.VerbNotFound, args);
        }
        catch (InternalException ie)
        {
            // It is an internal exception type. We want to show the message but we don't need
            // to burden the user with the full stacktrace.
            var args = SyntheticArguments.From(
                ("message", ie.Message),
                ("stacktrace", "")
            );
            HandleError(ie, _state.EventCatalog.EngineError, args);
        }
        catch (WrappedException we)
        {
            var e = we.InnerException!;
            // We've received some other error. Execute the EngineError script
            // and hope for the best
            var args = SyntheticArguments.From(
                ("message", e.Message),
                ("stacktrace", e.StackTrace ?? ""),
                ("exitcode", ExitCode.Constants.UnhandledException.ToString())
            );
            HandleError(e, _state.EventCatalog.EngineError, args);
        }
        catch (Exception e)
        {
            // We've received some other error. Execute the EngineError script
            // and hope for the best
            var args = SyntheticArguments.From(
                ("message", e.Message),
                ("stacktrace", e.StackTrace ?? ""),
                ("exitcode", ExitCode.Constants.UnhandledException.ToString())
            );
            HandleError(e, _state.EventCatalog.EngineError, args);
        }
    }

    // Handle an error from the dispatcher.
    private void HandleError(Exception currentException, EventScript script, IArgumentCollection args)
    {
        // When we handle an error, we set the Current Error in the State. When we are done
        // handling it, we clear the error from the State. If we get into HandleError() and
        // there is already a Current Error in the state it means we have gotten into a loop,
        // throwing errors from the error-handling code. Instead of spiralling down forever,
        // we bail out with a very stern message.

        // Check if we already have a current error
        var maybePrevious = _state.Metadata.Get(Constants.Metadata.Error);
        if (maybePrevious.IsSuccess && maybePrevious.GetValueOrThrow() is Exception previousException)
        {
            // An exception was thrown while attempting to handle a previous error.
            // This isn't scripted because it's critical error-handling code and we cannot
            // allow yet another exception to be thrown at this point.
            _output.WriteError($"""
                Received an exception while attempting to handle a previous exception
                This is a fatal condition and the engine will exit
                Make sure you clear the current exception when you are done handling it to avoid these situations
                Current Exception:
                {currentException.Message}
                {currentException.StackTrace ?? ""}
                Previous Exception:
                {previousException.Message}
                {previousException.StackTrace ?? ""}
                """);
            _state.SignalExit(ExitCode.CascadeError);
            return;
        }

        // Add the current exception to state metadata so we can keep track of loops,
        // then prepend the error-handling script and a command to remove the exception
        // from metadata (prepends happen in reverse order from how they're executed)
        // We can't remove metadata in the script, because users might change the script
        // and inadvertantly break loop detection.
        _state.Metadata.Add(Constants.Metadata.Error, currentException, false);
        if (_state.RunMode == EngineRunMode.Headless)
            _state.Commands.Prepend($"{ExitHandler.Name} {ExitCode.Constants.UnhandledException}");
        _state.Commands
            .Prepend($"{MetadataHandler.Name} remove {Constants.Metadata.Error}")
            .Prepend(script, args);

        // Current command queue:
        // 1. Error-handling script
        // 2. "metadata remove __CURRENT_EXCEPTION"
        // 3. "exit <exitcode>" (headless mode only)
        // 4. previous contents of command queue, if any
    }
}
