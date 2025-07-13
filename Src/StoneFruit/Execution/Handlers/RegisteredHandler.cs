using System;

namespace StoneFruit.Execution.Handlers;

public sealed record RegisteredHandler(Type HandlerType)
{
    public static RegisteredHandler Create<T>()
        where T : class, IHandlerBase
        => new RegisteredHandler(typeof(T));
}
