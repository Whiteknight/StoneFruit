using System;
using System.Threading.Tasks;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Containers.Lamar;

namespace StoneFruit.Utilities
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceRegistry()
                .SetupEngine(Build);

            var container = new Container(serviceCollection);
            var engine = container.GetService<Engine>();
            Environment.ExitCode = await engine.RunWithCommandLineArgumentsAsync();
        }

        private static void Build(IEngineBuilder builder)
        {
            builder
                .SetupEnvironments(e => e.UseInstance(new UtilitiesEnvironment()))
                ;
        }
    }
}
