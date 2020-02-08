using StructureMap;

namespace StoneFruit.StructureMap
{
    public static class ContainerExtensions
    {
        public static ConfigurationExpression ScanForCommandVerbs(this ConfigurationExpression config)
        {
            config.Scan(s =>
            {
                s.AssemblyContainingType<IHandlerBase>();
                s.AssembliesFromApplicationBaseDirectory();
                s.AddAllTypesOf<IHandlerBase>();
                s.WithDefaultConventions();
            });
            return config;
        }
    }
}
