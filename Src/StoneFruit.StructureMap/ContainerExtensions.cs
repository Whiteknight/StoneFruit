using StructureMap;

namespace StoneFruit.StructureMap
{
    public static class ContainerExtensions
    {
        public static ConfigurationExpression ScanForCommandVerbs(this ConfigurationExpression config)
        {
            config.Scan(s =>
            {
                s.AssemblyContainingType<ICommandVerb>();
                s.AssembliesFromApplicationBaseDirectory();
                s.AddAllTypesOf<ICommandVerb>();
                s.WithDefaultConventions();
            });
            return config;
        }
    }
}
