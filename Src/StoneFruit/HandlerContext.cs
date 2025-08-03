using StoneFruit.Execution;

namespace StoneFruit;

public sealed record HandlerContext(
    IArguments Arguments,
    IOutput Output,
    IInput Input,
    CommandDispatcher Dispatcher,
    IEnvironments Environments,
    ICommandParser Parser,
    EngineState State)
{
    public IEnvironment CurrentEnvironment => Environments.GetCurrent().GetValueOrThrow();
}