using StructureMap;
using StructureMap.Graph;

namespace StoneFruit.StructureMap
{
    public static class ContainerExtensions
    {
        public static ConfigurationExpression ScanForCommandVerbs(this ConfigurationExpression config)
        {
            config.Scan(s =>
            {
                s.ExcludeNamespace(nameof(StoneFruit.BuiltInVerbs.Hidden));
                s.AssemblyContainingType<ICommandVerb>();
                s.AssembliesFromApplicationBaseDirectory();
                s.AddAllTypesOf<ICommandVerb>();
                s.WithDefaultConventions();
            });
            return config;
        }
    }
}
