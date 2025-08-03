using System;

namespace StoneFruit.Execution.Handlers;

// Type to help with DI registrations.
public sealed record RegisteredHandler(Type HandlerType, string? Prefix = null)
{
    public static RegisteredHandler Create<T>(string? prefix = null)
        where T : class, IHandlerBase
        => new RegisteredHandler(typeof(T), prefix);
}
