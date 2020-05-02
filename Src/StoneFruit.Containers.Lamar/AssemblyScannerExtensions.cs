using Lamar.Scanning.Conventions;

namespace StoneFruit.Containers.Lamar
{
    public static class AssemblyScannerExtensions
    {
        /// <summary>
        /// Extension method to help with scanning assemblies in the solution for handler types
        /// </summary>
        /// <param name="scanner"></param>
        public static void ScanForHandlers(this IAssemblyScanner scanner)
        {
            scanner.TheCallingAssembly();
            scanner.AssemblyContainingType<IHandlerBase>();
            scanner.AssembliesFromApplicationBaseDirectory();
            scanner.AddAllTypesOf<IHandlerBase>();
            //scanner.WithDefaultConventions();
        }
    }
}