using StructureMap;

namespace StoneFruit.StructureMap
{
    public static class ContainerExtensions
    {
        public static ConfigurationExpression ScanForCommandVerbs(this ConfigurationExpression config)
        {
            config.Scan(s =>
            {
                s.AssemblyContainingType<ICommandVerbBase>();
                s.AssembliesFromApplicationBaseDirectory();
                s.AddAllTypesOf<ICommandVerbBase>();
                s.WithDefaultConventions();
            });
            return config;
        }
    }
}
