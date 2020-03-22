using Ninject;

namespace StoneFruit.Containers.Ninject
{
    public static class HandlerSetupExtensions
    {
        public static IHandlerSetup UseNinjectHandlerSource(this IHandlerSetup handlers)
            => handlers.AddSource(new NinjectHandlerSource());

        public static IHandlerSetup UseNinjectHandlerSource(this IHandlerSetup handlers, IKernel container)
            => handlers.AddSource(new NinjectHandlerSource(container));
    }
}
