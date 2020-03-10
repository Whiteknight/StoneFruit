using StructureMap;

namespace StoneFruit.Containers.StructureMap
{
    public static class EngineBuilderExtensions
    {
        public static IHandlerSetup UseStructureMapHandlerSource(this IHandlerSetup handlers) 
            => handlers.AddSource(new StructureMapHandlerSource());

        public static IHandlerSetup UseStructureMapHandlerSource(this IHandlerSetup handlers, IContainer container) 
            => handlers.AddSource(new StructureMapHandlerSource(container));
    }
}
