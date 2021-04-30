using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Execution.Handlers
{
    public class DefaultHandlerMethodInvoker : IHandlerMethodInvoker
    {
        public void Invoke(object instance, MethodInfo method, IArguments arguments, CommandDispatcher dispatcher, CancellationToken token)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 0)
            {
                method.Invoke(instance, Array.Empty<object>());
                return;
            }

            var args = new object?[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                args[i] = GetValue(arguments, dispatcher, parameter.ParameterType, parameter.Name ?? "", i, token);
            }

            method.Invoke(instance, args);
        }

        public Task InvokeAsync(object instance, MethodInfo method, IArguments arguments, CommandDispatcher dispatcher, CancellationToken token)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 0)
            {
                var resulta = method.Invoke(instance, Array.Empty<object>());
                return (resulta as Task) ?? Task.CompletedTask;
            }

            var args = new object?[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                args[i] = GetValue(arguments, dispatcher, parameter.ParameterType, parameter.Name ?? "", i, token);
            }

            var resultb = method.Invoke(instance, args);
            return (resultb as Task) ?? Task.CompletedTask;
        }

        private static object? GetValue(IArguments arguments, CommandDispatcher dispatcher, Type type, string name, int index, CancellationToken cancellationToken)
        {
            if (type == typeof(CancellationToken))
                return cancellationToken;
            if (type == typeof(CommandDispatcher))
                return dispatcher;
            if (type == typeof(IEnvironmentCollection))
                return dispatcher.Environments;
            if (type == typeof(EngineState))
                return dispatcher.State;
            if (type == typeof(IOutput))
                return dispatcher.Output;
            if (type == typeof(CommandParser))
                return dispatcher.Parser;
            if (type == typeof(IArguments))
                return arguments;

            var currentEnv = dispatcher.Environments.GetCurrent();
            if (currentEnv.HasValue && type == currentEnv.Value.GetType())
                return currentEnv.Value;

            if (type == typeof(bool))
                return arguments.HasFlag(name);

            IValuedArgument arg = arguments.Get(name);
            if (!arg.Exists())
                arg = arguments.Get(index);
            if (!arg.Exists())
                return null;

            if (type == typeof(string))
                return arg.AsString();
            if (type == typeof(int))
                return arg.AsInt();

            return null;
        }
    }
}
