﻿using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Invokes a handler method on an object, including mapping arguments where necessary.
/// </summary>
public interface IHandlerMethodInvoker
{
    void Invoke(object instance, MethodInfo method, HandlerContext context);

    Task InvokeAsync(object instance, MethodInfo method, HandlerContext context, CancellationToken token);
}
