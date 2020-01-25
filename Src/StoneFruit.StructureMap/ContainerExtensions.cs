using StructureMap;

namespace StoneFruit.StructureMap
{
    public static class ContainerExtensions
    {
        public static ConfigurationExpression ScanForCommandVerbs(this ConfigurationExpression config)
        {
            config.Scan(s =>
            {
                s.AssemblyContainingType<ICommandHandlerBase>();
                s.AssembliesFromApplicationBaseDirectory();
                s.AddAllTypesOf<ICommandHandlerBase>();
                s.WithDefaultConventions();
            });
            return config;
        }
    }
}
