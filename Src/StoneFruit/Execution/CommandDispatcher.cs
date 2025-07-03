using System;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Execution.Arguments;
using StoneFruit.Utility;

namespace StoneFruit.Execution;

/// <summary>
/// Dispatches control to the appropriate handler for a given verb. Used by the engine
/// to dispatch commands and can be used from within a command to delegate to another
/// command without returning to the engine runloop first.
/// </summary>
public class CommandDispatcher
{
    public CommandDispatcher(ICommandParser parser, IHandlers handlers, IEnvironmentCollection environments, EngineState state, IOutput output)
    {
        Assert.NotNull(parser, nameof(parser));
        Assert.NotNull(handlers, nameof(handlers));
        Assert.NotNull(environments, nameof(environments));
        Assert.NotNull(output, nameof(output));
        Assert.NotNull(state, nameof(state));

        Parser = parser;
        Handlers = handlers;
        Environments = environments;
        Output = output;
        State = state;
    }

    /// <summary>
    /// Gets the parser to turn strings into Commands.
    /// </summary>
    public ICommandParser Parser { get; }

    /// <summary>
    /// Gets the source of handlers.
    /// </summary>
    public IHandlers Handlers { get; }

    /// <summary>
    /// Gets the current environment and collection of all possible environments.
    /// </summary>
    public IEnvironmentCollection Environments { get; }

    /// <summary>
    /// Gets the execution state of the engine.
    /// </summary>
    public EngineState State { get; }

    /// <summary>
    /// Gets the output.
    /// </summary>
    public IOutput Output { get; }

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
        Assert.NotNull(argsOrString, nameof(argsOrString));
        var args = argsOrString.GetArguments(Parser);
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
        Assert.NotNullOrEmpty(commandString, nameof(commandString));
        var command = Parser.ParseCommand(commandString);
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
        Assert.NotNull(args, nameof(args));
        var newArgs = new PrependedVerbArguments(verb, args);
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
        Assert.NotNull(arguments, nameof(arguments));
        State.SetCurrentArguments(arguments);
        var handler = Handlers.GetInstance(arguments, this)
            .OnFailure(() => throw VerbNotFoundException.FromArguments(arguments))
            .GetValueOrThrow();
        if (handler is IHandler syncHandler)
        {
            syncHandler.Execute();
            State.ClearCurrentArguments();
            return;
        }

        if (handler is IAsyncHandler asyncHandler)
        {
            Task.Run(async () => await asyncHandler.ExecuteAsync(token), token)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            State.ClearCurrentArguments();
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
        Assert.NotNull(argsOrString, nameof(argsOrString));
        var args = argsOrString.GetArguments(Parser);
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
        Assert.NotNullOrEmpty(commandString, nameof(commandString));
        var command = Parser.ParseCommand(commandString);
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
        Assert.NotNull(args, nameof(args));
        var newArgs = new PrependedVerbArguments(verb, args);
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
        Assert.NotNull(arguments, nameof(arguments));

        // Set the current arguments in the State, so they can be resolved from the container
        State.SetCurrentArguments(arguments);

        // Get the handler. Throw if a matching one is not found
        var handler = Handlers.GetInstance(arguments, this)
            .OnFailure(() => throw VerbNotFoundException.FromArguments(arguments))
            .GetValueOrThrow();

        // Invoke the handler, async or otherwise.
        await ExecuteHandler(handler, token);
        State.ClearCurrentArguments();
    }

    private static async Task ExecuteHandler(IHandlerBase handler, CancellationToken token)
    {
        if (handler is IHandler syncHandler)
        {
            syncHandler.Execute();
            return;
        }

        if (handler is IAsyncHandler asyncHandler)
        {
            await asyncHandler.ExecuteAsync(token);
            return;
        }

        throw new InvalidOperationException($"Unknown handler type ${handler.GetType().Name}");
    }
}
