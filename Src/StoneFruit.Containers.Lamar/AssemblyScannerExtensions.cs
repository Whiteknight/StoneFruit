using Lamar.Scanning.Conventions;

namespace StoneFruit.Containers.Lamar
{
    public static class AssemblyScannerExtensions
    {
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