using StoneFruit.Execution;

namespace StoneFruit;

// TODO: Is there a way to get access to IServiceProvider here so handler delegates can get a rich
// set of objects?
public sealed record HandlerContext(
    IArguments Arguments,
    IOutput Output,
    CommandDispatcher Dispatcher,
    IEnvironments Environments,
    ICommandParser Parser,
    EngineState State);
