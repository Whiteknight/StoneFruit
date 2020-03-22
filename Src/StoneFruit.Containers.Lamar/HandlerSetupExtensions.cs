using Lamar;

namespace StoneFruit.Containers.Lamar
{
    public static class HandlerSetupExtensions
    {
        public static IHandlerSetup UseLamarHandlerSource<TEnvironment>(this IHandlerSetup handlers)
            where TEnvironment : class
            => handlers.AddSource(new LamarHandlerSource<TEnvironment>());

        public static IHandlerSetup UseLamarHandlerSource<TEnvironment>(this IHandlerSetup handlers, IContainer container)
            where TEnvironment : class
            => handlers.AddSource(new LamarHandlerSource<TEnvironment>(container));
    }
}