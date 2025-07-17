using System;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Exceptions;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution;

/// <summary>
/// Dispatches control to the appropriate handler for a given verb. Used by the engine
/// to dispatch commands and can be used from within a command to delegate to another
/// command without returning to the engine runloop first.
/// </summary>
public class CommandDispatcher
{
    private readonly ICommandParser _parser;
    private readonly IHandlers _handlers;
    private readonly IEnvironments _environments;
    private readonly EngineState _state;
    private readonly IOutput _output;

    public CommandDispatcher(ICommandParser parser, IHandlers handlers, IEnvironments environments, EngineState state, IOutput output)
    {
        _parser = NotNull(parser);
        _handlers = NotNull(handlers);
        _environments = NotNull(environments);
        _output = NotNull(output);
        _state = NotNull(state);
    }

    /// <summary>
    /// Find and execute the appropriate handler for the given arguments object or
    /// unparsed command string. This overload is mostly intended for internal use. If you have
    /// a raw unparsed command string or a parsed IArguments object, you should use one of
    /// those overloads instead.
    /// </summary>
    /// <param name="argsOrString"></param>
    /// <param name="token"></param>
    public void Execute(ArgumentsOrString argsOrString, CancellationToken token = default)
    {
        var args = NotNull(argsOrString).GetArguments(_parser);
        Execute(args, token);
    }

    /// <summary>
    /// Find and execute the appropriate handler for the given unparsed command string. Use
    /// this overload if you have the raw text of a command to execute and do not want to
    /// parse it out yourself.
    /// </summary>
    /// <param name="commandString"></param>
    /// <param name="token"></param>
    public void Execute(string commandString, CancellationToken token = default)
    {
        var command = _parser.ParseCommand(NotNullOrEmpty(commandString));
        Execute(command, token);
    }

    /// <summary>
    /// Find and execute the appropriate handler for the given verb and arguments. Use this
    /// overload if you want to explicitly separate the verb from the rest of the arguments.
    /// </summary>
    /// <param name="verb"></param>
    /// <param name="args"></param>
    /// <param name="token"></param>
    public void Execute(Verb verb, IArguments args, CancellationToken token = default)
    {
        var newArgs = new PrependedVerbArguments(verb, NotNull(args));
        Execute(newArgs, token);
    }

    /// <summary>
    /// Find and execute the appropriate handler for the given arguments. Use this variant if
    /// you have a parsed IArguments which contains the verb and args for the command.
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="token"></param>
    public void Execute(IArguments arguments, CancellationToken token = default)
    {
        NotNull(arguments);
        var context = CreateHandlerContext(arguments);
        _state.SetHandlerExecutionContext(arguments, context);

        try
        {
            var handler = _handlers.GetInstance(context)
                .OnFailure(() => throw VerbNotFoundException.FromArguments(arguments))
                .GetValueOrThrow();
            if (handler is IHandler syncHandler)
            {
                syncHandler.Execute();

                return;
            }

            if (handler is IAsyncHandler asyncHandler)
            {
                Task.Run(async () => await asyncHandler.ExecuteAsync(token), token)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
        }
        finally
        {
            _state.ClearHandlerExecutionContext();
        }
    }

    /// <summary>
    /// Find and execute the appropriate handler for the given arguments object or
    /// unparsed command string. This overload is mostly intended for internal use. If you have
    /// a raw unparsed command string or a parsed IArguments object, you should call one of
    /// those overloads instead.
    /// </summary>
    /// <param name="argsOrString"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task ExecuteAsync(ArgumentsOrString argsOrString, CancellationToken token = default)
    {
        var args = NotNull(argsOrString).GetArguments(_parser);
        return ExecuteAsync(args, token);
    }

    /// <summary>
    /// Parse the command string, find and dispatch the appropriate handler.
    /// </summary>
    /// <param name="commandString"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task ExecuteAsync(string commandString, CancellationToken token = default)
    {
        var command = _parser.ParseCommand(NotNullOrEmpty(commandString));
        return ExecuteAsync(command, token);
    }

    /// <summary>
    /// Find and execute the appropriate handler for the given verb and arguments. Use this
    /// variant if you have already separated the verb from the rest of the arguments and you
    /// want to specify each explicitly.
    /// </summary>
    /// <param name="verb"></param>
    /// <param name="args"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task ExecuteAsync(Verb verb, IArguments args, CancellationToken token = default)
    {
        var newArgs = new PrependedVerbArguments(verb, NotNull(args));
        return ExecuteAsync(newArgs, token);
    }

    /// <summary>
    /// Find and execute the appropriate handler for the given Command object. Use this
    /// overload if you have a parsed IArguments object which contains the verb and args for
    /// the command.
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task ExecuteAsync(IArguments arguments, CancellationToken token = default)
    {
        // Set the current arguments in the State, so they can be resolved from the container
        NotNull(arguments);
        var context = CreateHandlerContext(arguments);
        _state.SetHandlerExecutionContext(arguments, context);

        try
        {
            // Get the handler. Throw if a matching one is not found
            var handler = _handlers.GetInstance(context)
                .OnFailure(() => throw VerbNotFoundException.FromArguments(arguments))
                .GetValueOrThrow();

            // Invoke the handler, async or otherwise.
            await ExecuteHandler(handler, context, token);
        }
        finally
        {
            _state.ClearHandlerExecutionContext();
        }
    }

    private static async Task ExecuteHandler(IHandlerBase handler, HandlerContext context, CancellationToken token)
    {
        if (handler is IHandler syncHandler)
        {
            syncHandler.Execute();
            return;
        }

        if (handler is IHandlerWithContext syncWithContext)
        {
            syncWithContext.Execute(context);
            return;
        }

        if (handler is IAsyncHandler asyncHandler)
        {
            await asyncHandler.ExecuteAsync(token);
            return;
        }

        if (handler is IAsyncHandlerWithContext asyncWithContext)
        {
            await asyncWithContext.ExecuteAsync(context, token);
            return;
        }

        throw new InvalidOperationException($"Unknown handler type ${handler.GetType().Name}");
    }

    private HandlerContext CreateHandlerContext(IArguments args)
        => new HandlerContext(args, _output, this, _environments, _parser, _state);
}
