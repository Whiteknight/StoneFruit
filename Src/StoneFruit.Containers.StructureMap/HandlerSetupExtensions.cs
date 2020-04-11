using StoneFruit.Execution;
using StoneFruit.Utility;
using StructureMap;

namespace StoneFruit.Containers.StructureMap
{
    public static class HandlerSetupExtensions
    {
        public static IHandlerSetup UseStructureMapHandlerSource(this IHandlerSetup handlers, IContainer container = null, ITypeVerbExtractor verbExtractor = null)
        {
            Assert.ArgumentNotNull(handlers, nameof(handlers));
            var source = new StructureMapHandlerSource(container, verbExtractor);
            return handlers.AddSource(source);
        }
    }
}
