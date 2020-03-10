using Lamar.Scanning.Conventions;

namespace StoneFruit.Containers.Lamar
{
    public static class AssemblyScannerExtensions
    {
        public static void ScanForCommandVerbs(this IAssemblyScanner scanner)
        {
            scanner.AssemblyContainingType<IHandlerBase>();
            scanner.AssembliesFromApplicationBaseDirectory();
            scanner.AddAllTypesOf<IHandlerBase>();
            scanner.WithDefaultConventions();
        }
    }
}