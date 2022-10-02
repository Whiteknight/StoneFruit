using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace StoneFruit.Execution.Handlers
{
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

        private object InvokeDynamic(object instance, MethodInfo method, IArguments args, CancellationToken token)
        {
            var parameters = method.GetParameters();
            var argumentValues = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType == typeof(CancellationToken))
                {
                    argumentValues[i] = token;
                    continue;
                }
                if (parameters[i].ParameterType == typeof(bool))
                {
                    argumentValues[i] = args.HasFlag(parameters[i].Name);
                    continue;
                }

                IValuedArgument arg = args.Get(parameters[i].Name);
                if (!arg.Exists())
                    arg = args.Get(i);
                if (parameters[i].ParameterType == typeof(string))
                {
                    argumentValues[i] = arg.Exists() ? arg.AsString() : null;
                    continue;
                }
                if (parameters[i].ParameterType == typeof(int))
                {
                    argumentValues[i] = arg.Exists() ? arg.AsInt() : 0;
                    continue;
                }

                argumentValues[i] = _provider.GetRequiredService(parameters[i].ParameterType);
            }
            return method.Invoke(instance, argumentValues);
        }
    }
}
