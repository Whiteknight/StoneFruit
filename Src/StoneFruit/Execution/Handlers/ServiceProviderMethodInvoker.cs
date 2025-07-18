using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Handlers;

/// <summary>
/// Default implementation of IHandlerMethodInvoker, attempts to invoke the given method by
/// mapping parameters from built-in values and argument values.
/// </summary>
public class ServiceProviderMethodInvoker : IHandlerMethodInvoker
{
    private readonly IServiceProvider _provider;
    private readonly IValueTypeParser[] _typeParsers;

    public ServiceProviderMethodInvoker(IServiceProvider provider, IEnumerable<IValueTypeParser> typeParsers)
    {
        _provider = provider;
        _typeParsers = typeParsers.OrEmptyIfNull().ToArray();
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
        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            argumentValues[i] = GetArgumentValue(parameter, args, token);
        }

        return method.Invoke(instance, argumentValues);
    }

    private object? GetArgumentValue(ParameterInfo parameter, IArguments args, CancellationToken token)
    {
        Debug.Assert(parameter.Name != null, "Parameter name should not be null");

        var isResolvableType = (parameter.ParameterType.IsClass || parameter.ParameterType.IsInterface) && parameter.ParameterType != typeof(string);
        if (isResolvableType)
        {
            var service = _provider.GetService(parameter.ParameterType);
            if (service != null)
                return service;
        }

        if (parameter.ParameterType == typeof(CancellationToken))
            return token;

        if (parameter.ParameterType == typeof(bool))
            return args.HasFlag(parameter.Name!);

        // TODO: If we have an array or collection type, we should be able to fill that in with
        // the list of all matching named arguments with that name, or the value of all remaining
        // positionals, etc.

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

        if (isResolvableType)
        {
            var rawMethod = GetType().GetMethod(nameof(TryParseValue), BindingFlags.NonPublic | BindingFlags.Static)!;
            var tryParseMethod = rawMethod.MakeGenericMethod(parameter.ParameterType);

            return tryParseMethod.Invoke(null, [arg, _typeParsers]);
        }

        return default;
    }

    private static T? TryParseValue<T>(IValuedArgument arg, IValueTypeParser[] parsers)
        where T : class
    {
        return parsers
            .OfType<IValueTypeParser<T>>().FirstOrDefault()
            ?.TryParse(arg);
    }
}
