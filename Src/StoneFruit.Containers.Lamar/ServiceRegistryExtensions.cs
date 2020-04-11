using System;
using System.Collections.Generic;
using System.Text;
using Lamar;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Containers.Lamar
{
    public static class ServiceRegistryExtensions
    {
        public static void SetupInjectableServices(this ServiceRegistry registry)
            => SetupInjectableServices<object>(registry);

        public static void SetupInjectableServices<TEnvironment>(this ServiceRegistry registry) 
            where TEnvironment : class
        {
            registry.Injectable<CommandDispatcher>();
            registry.Injectable<IEnvironmentCollection>();
            registry.Injectable<EngineState>();
            registry.Injectable<IOutput>();
            registry.Injectable<ICommandParser>();
            registry.Injectable<IHandlerSource>();
            registry.Injectable<Command>();
            registry.Injectable<IArguments>();
            if (typeof(TEnvironment) != typeof(object))
                registry.Injectable<TEnvironment>();
        }
    }
}
