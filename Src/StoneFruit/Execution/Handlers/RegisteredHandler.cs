using System;

namespace StoneFruit.Execution.Handlers;

// Type to help with DI registrations.
public sealed record RegisteredHandler(Type HandlerType)
{
    public static RegisteredHandler Create<T>()
        where T : class, IHandlerBase
        => new RegisteredHandler(typeof(T));
}
