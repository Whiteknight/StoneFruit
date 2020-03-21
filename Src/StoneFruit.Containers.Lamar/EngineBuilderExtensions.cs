using Lamar;

namespace StoneFruit.Containers.Lamar
{
    public static class EngineBuilderExtensions
    {
        public static IHandlerSetup UseLamarHandlerSource(this IHandlerSetup handlers)
            => handlers.AddSource(new LamarHandlerSource());

        public static IHandlerSetup UseLamarHandlerSource(this IHandlerSetup handlers, IContainer container)
            => handlers.AddSource(new LamarHandlerSource(container));
    }
}