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

    public void Invoke(object instance, MethodInfo method, IArguments arguments, CommandDispatcher dispatcher, CancellationToken token)
    {
        InvokeDynamic(instance, method, arguments, token);
    }

    public Task InvokeAsync(object instance, MethodInfo method, IArguments arguments, CommandDispatcher dispatcher, CancellationToken token)
    {
        return InvokeDynamic(instance, method, arguments, token) as Task ?? Task.CompletedTask;
    }

    private object? InvokeDynamic(object instance, MethodInfo method, IArguments args, CancellationToken token)
    {
        var parameters = method.GetParameters();
        var argumentValues = new object?[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            argumentValues[i] = AssignArgumentValue(parameter, args, i, token);
        }

        return method.Invoke(instance, argumentValues);
    }

    private object? AssignArgumentValue(ParameterInfo parameter, IArguments args, int i, CancellationToken token)
    {
        // TODO: Separate the i which is the current parameter from j which would be the current non-DI index
        // This way we can put positional args after of injected args, or where ever is appropriate.
        Debug.Assert(parameter.Name != null, "Parameter name should not be null");
        if ((parameter.ParameterType.IsClass || parameter.ParameterType.IsInterface) && parameter.ParameterType != typeof(string))
            return _provider.GetRequiredService(parameter.ParameterType);

        if (parameter.ParameterType == typeof(CancellationToken))
            return token;

        if (parameter.ParameterType == typeof(bool))
            return args.HasFlag(parameter.Name!);

        IValuedArgument arg = args.Get(parameter.Name!);
        if (!arg.Exists())
            arg = args.Get(i);

        if (parameter.ParameterType == typeof(string))
            return arg.Exists() ? arg.AsString() : null;

        if (parameter.ParameterType == typeof(int))
            return arg.Exists() ? arg.AsInt() : 0;

        return null;
    }
}
