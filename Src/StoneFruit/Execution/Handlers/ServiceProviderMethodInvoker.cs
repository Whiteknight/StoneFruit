using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Default implementation of IHandlerMethodInvoker, attempts to invoke the given method by
/// mapping parameters from built-in values and argument values.
/// </summary>
public class ServiceProviderMethodInvoker : IHandlerMethodInvoker
{
    private readonly IServiceProvider _provider;

    public ServiceProviderMethodInvoker(IServiceProvider provider)
    {
        _provider = provider;
    }

    public void Invoke(object instance, MethodInfo method, HandlerContext context)
    {
        InvokeDynamic(instance, method, context.Arguments, CancellationToken.None);
    }

    public Task InvokeAsync(object instance, MethodInfo method, HandlerContext context, CancellationToken token)
        => InvokeDynamic(instance, method, context.Arguments, token) as Task ?? Task.CompletedTask;

    private object? InvokeDynamic(object instance, MethodInfo method, IArguments args, CancellationToken token)
    {
        // Iterate over the list of parameters. For each parameter, depending on it's position and
        // type, try to get a value.
        // First we look in the IServiceProvider if the parameter is a non-string reference type.
        // Next we see if we can fill in some standard values (the CancellationToken)
        // If it's a bool we see if we can find a flag in IArguments with the same name
        // Otherwise we look for either a named argument with the same name as the parameter,
        // and finally we look for the next positional argument.
        var parameters = method.GetParameters();
        var argumentValues = new object?[parameters.Length];
        int j = 0;
        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            (argumentValues[i], var shouldIncrement) = AssignArgumentValue(parameter, args, j, token);

            // j is the current positional argument index. Only increment it if we have pulled a
            // positional argument.
            if (shouldIncrement)
                j++;
        }

        return method.Invoke(instance, argumentValues);
    }

    private (object? Value, bool IncrementPosition) AssignArgumentValue(ParameterInfo parameter, IArguments args, int i, CancellationToken token)
    {
        Debug.Assert(parameter.Name != null, "Parameter name should not be null");
        if ((parameter.ParameterType.IsClass || parameter.ParameterType.IsInterface) && parameter.ParameterType != typeof(string))
            return (_provider.GetRequiredService(parameter.ParameterType), false);

        if (parameter.ParameterType == typeof(CancellationToken))
            return (token, false);

        if (parameter.ParameterType == typeof(bool))
            return (args.HasFlag(parameter.Name!), false);

        bool increment = false;
        IValuedArgument arg = args.Get(parameter.Name!);
        if (!arg.Exists())
        {
            arg = args.Get(i);
            increment = true;
        }

        // If we don't have a matching named or positional, just return nothing.
        if (!arg.Exists())
            return (null, true);

        if (parameter.ParameterType == typeof(string))
            return (arg.AsString(), increment);

        if (parameter.ParameterType == typeof(int))
            return (arg.AsInt(), increment);

        return default;
    }
}
