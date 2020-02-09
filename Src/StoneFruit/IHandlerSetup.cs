using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoneFruit.Execution;
using StoneFruit.Execution.HandlerSources;

namespace StoneFruit
{
    public interface IHandlerSetup
    {
        IHandlerSetup AddSource(IHandlerSource source);
        IHandlerSetup Add(string verb, Action<Command, CommandDispatcher> handle, string description = null, string usage = null);
        IHandlerSetup AddAsync(string verb, Func<Command, CommandDispatcher, Task> handleAsync, string description = null, string usage = null);
    }

    public static class HandlerSetupExtensions
    {
        public static IHandlerSetup UsePublicMethodsAsHandlers(this IHandlerSetup handlers, object instance)
        {
            return handlers.AddSource(new InstanceMethodHandlerSource(instance, null, null));
        }

        public static IHandlerSetup UseHandlerTypes(this IHandlerSetup handlers, IEnumerable<Type> commandTypes)
        {
            return handlers.AddSource(new TypeListConstructSource(commandTypes ?? Enumerable.Empty<Type>()));
        }

        public static IHandlerSetup UseHandlerTypes(this IHandlerSetup handlers, params Type[] commandTypes)
        {
            return handlers.AddSource(new TypeListConstructSource(commandTypes ?? Enumerable.Empty<Type>()));
        }
    }
}