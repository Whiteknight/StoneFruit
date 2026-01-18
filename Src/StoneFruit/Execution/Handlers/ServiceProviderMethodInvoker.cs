using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Handlers;

#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

/// <summary>
/// Default implementation of IHandlerMethodInvoker, attempts to invoke the given method by
/// mapping parameters from built-in values and argument values.
/// </summary>
public class ServiceProviderMethodInvoker : IHandlerMethodInvoker
{
    private static readonly MethodInfo _getTaskMethod = typeof(ServiceProviderMethodInvoker)
        .GetMethod(nameof(GetObjectTask), BindingFlags.NonPublic | BindingFlags.Static)!;

    private readonly IServiceProvider _provider;
    private readonly ArgumentValueMapper _mapper;

    public ServiceProviderMethodInvoker(IServiceProvider provider, ArgumentValueMapper mapper)
    {
        _provider = provider;
        _mapper = mapper;
    }

    public object? Invoke(object instance, MethodInfo method, HandlerContext context)
    {
        var result = InvokeDynamic(instance, method, context, CancellationToken.None);
        return ResolveResultToObject(result);
    }

    public object? Invoke(Delegate func, HandlerContext context)
    {
        var result = InvokeDynamic(func.Target, func.Method, context, CancellationToken.None);
        return ResolveResultToObject(result);
    }

    public Task<object?> InvokeAsync(object instance, MethodInfo method, HandlerContext context, CancellationToken token)
    {
        var result = InvokeDynamic(instance, method, context, token);
        return ResolveResultToTaskOfObject(result, token);
    }

    public Task<object?> InvokeAsync(Delegate func, HandlerContext context, CancellationToken token)
    {
        var result = InvokeDynamic(func.Target, func.Method, context, token);
        return ResolveResultToTaskOfObject(result, token);
    }

    private object? InvokeDynamic(object? instance, MethodInfo method, HandlerContext context, CancellationToken token)
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
        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            argumentValues[i] = GetArgumentValue(parameter, context, token);
        }

        return method.Invoke(instance, argumentValues);
    }

    private static Task<object?> ResolveResultToTaskOfObject(object? result, CancellationToken token)
    {
        if (result == null)
            return Task.FromResult<object?>(null);
        var resultType = result.GetType();
        if (resultType.Assembly == typeof(ServiceProviderMethodInvoker).Assembly)
            return Task.FromResult<object?>(null);

        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            Debug.Assert(_getTaskMethod != null);
            var valueMethod = _getTaskMethod.MakeGenericMethod(resultType.GetGenericArguments()[0]);
            return valueMethod.Invoke(null, [result]) as Task<object?> ?? Task.FromResult<object?>(null);
        }

        if (result is Task task)
            return task.ContinueWith(v => (object?)null, token);
        return Task.FromResult<object?>(null);
    }

    private static object? ResolveResultToObject(object? result)
    {
        if (result == null)
            return null;
        var resultType = result.GetType();
        if (resultType.Assembly == typeof(ServiceProviderMethodInvoker).Assembly)
            return null;
        return result;
    }

    private static Task<object?> GetObjectTask<T>(Task<T> task)
    {
        return task.ContinueWith<object?>(t =>
        {
            if (typeof(T).Assembly == typeof(ServiceProviderMethodInvoker).Assembly)
                return null;
            return t.Result;
        });
    }

    private object? GetArgumentValue(ParameterInfo parameter, HandlerContext context, CancellationToken token)
    {
        Debug.Assert(parameter.Name != null, "Parameter name should not be null");

        if (parameter.ParameterType == typeof(CancellationToken))
            return token;

        var isResolvableType = (parameter.ParameterType.IsClass || parameter.ParameterType.IsInterface) && parameter.ParameterType != typeof(string);
        if (isResolvableType)
        {
            if (parameter.ParameterType == typeof(HandlerContext))
                return context;

            var service = _provider.GetService(parameter.ParameterType);
            if (service != null)
                return service;
        }

        var args = context.Arguments;

        if (parameter.ParameterType == typeof(bool))
            return args.HasFlag(parameter.Name!);

        IValuedArgument arg = args.Get(parameter.Name!);
        if (!arg.Exists())
            arg = args.Shift();

        // If we don't have a matching named or positional, just return nothing.
        if (!arg.Exists())
            return null;

        if (parameter.ParameterType == typeof(string))
            return arg.AsString();

        if (parameter.ParameterType == typeof(int) || parameter.ParameterType == typeof(int?))
            return arg.AsInt();

        var maybeResult = _mapper.TryParseValue(parameter.ParameterType, arg);
        var mappedValue = maybeResult.GetValueOrDefault(null!);
        if (mappedValue is not null)
        {
            Debug.Assert(mappedValue.GetType().IsAssignableTo(parameter.ParameterType));
            return mappedValue;
        }

        return default;
    }
}
