using StructureMap;

namespace StoneFruit.Containers.StructureMap
{
    public static class ConfigurationExpressionExtensions
    {
        /// <summary>
        /// Setup scanning for handler types in the solution
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
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