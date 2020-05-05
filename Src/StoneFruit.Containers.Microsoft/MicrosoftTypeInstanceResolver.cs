using System;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution;

namespace StoneFruit.Containers.Microsoft
{
    /// <summary>
    /// Use the Microsoft DI container to resolve handler instances
    /// </summary>
    public class MicrosoftTypeInstanceResolver
    {
        private readonly Func<IServiceProvider> _getProvider;

        public MicrosoftTypeInstanceResolver(Func<IServiceProvider> getProvider)
        {
            _getProvider = getProvider;
        }

        public object Resolve(Type handlerType, Command command, CommandDispatcher dispatcher)
        {
            using var scope = _getProvider().CreateScope();
            return scope.ServiceProvider.GetService(handlerType);
        }
    }
}