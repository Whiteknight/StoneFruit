using StructureMap;

namespace StoneFruit.StructureMap
{
    public static class EngineBuilderExtensions
    {
        public static EngineBuilder UseStructureMapContainerSource(this EngineBuilder builder)
        {
            return builder.UseCommandSource(new StructureMapCommandSource());
        }

        public static EngineBuilder UseStructureMapContainerSource(this EngineBuilder builder, IContainer container)
        {
            return builder.UseCommandSource(new StructureMapCommandSource(container));
        }
    }
}
