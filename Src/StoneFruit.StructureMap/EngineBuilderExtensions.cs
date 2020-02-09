using StructureMap;

namespace StoneFruit.StructureMap
{
    public static class EngineBuilderExtensions
    {
        public static EngineBuilder UseStructureMapHandlerSource(this EngineBuilder builder) 
            => builder.UseCommandSource(new StructureMapHandlerSource());

        public static EngineBuilder UseStructureMapHandlerSource(this EngineBuilder builder, IContainer container) 
            => builder.UseCommandSource(new StructureMapHandlerSource(container));
    }
}
