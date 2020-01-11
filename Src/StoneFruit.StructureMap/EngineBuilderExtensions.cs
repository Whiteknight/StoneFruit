using StructureMap;

namespace StoneFruit.StructureMap
{
    public static class EngineBuilderExtensions
    {
        public static EngineBuilder UseStructureMapContainerSource(this EngineBuilder builder) 
            => builder.UseCommandSource(new StructureMapCommandSource());

        public static EngineBuilder UseStructureMapContainerSource(this EngineBuilder builder, IContainer container) 
            => builder.UseCommandSource(new StructureMapCommandSource(container));
    }
}
