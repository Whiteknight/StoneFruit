using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Invokes a handler method on an object, including mapping arguments where necessary.
/// </summary>
public interface IHandlerMethodInvoker
{
    object? Invoke(object instance, MethodInfo method, HandlerContext context);

    object? Invoke(Delegate func, HandlerContext context);

    Task<object?> InvokeAsync(object instance, MethodInfo method, HandlerContext context, CancellationToken token);

    Task<object?> InvokeAsync(Delegate func, HandlerContext context, CancellationToken token);
}
