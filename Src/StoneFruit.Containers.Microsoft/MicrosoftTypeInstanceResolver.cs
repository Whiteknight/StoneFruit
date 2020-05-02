using System;
using StoneFruit.Execution;

namespace StoneFruit.Containers.Microsoft
{
    public class MicrosoftTypeInstanceResolver
    {
        private readonly Func<IServiceProvider> _getProvider;

        public MicrosoftTypeInstanceResolver(Func<IServiceProvider> getProvider)
        {
            _getProvider = getProvider;
        }

        public object Resolve(Type handlerType, Command command, CommandDispatcher dispatcher)
        {
            return _getProvider().GetService(handlerType);
        }
    }
}