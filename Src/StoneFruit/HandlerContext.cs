using StoneFruit.Execution;

namespace StoneFruit;

public sealed record HandlerContext(
    IArguments Arguments,
    IOutput Output,
    CommandDispatcher Dispatcher,
    IEnvironments Environments,
    ICommandParser Parser,
    EngineState State);
