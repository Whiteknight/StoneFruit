using System;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Containers.Lamar;

namespace StoneFruit.Utilities
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var serviceCollection = new ServiceRegistry()
                .SetupEngine(Build);

            var container = new Container(serviceCollection);
            var engine = container.GetService<Engine>();
            Environment.ExitCode = engine.RunWithCommandLineArguments();
        }

        private static void Build(IEngineBuilder builder)
        {
            builder
                .SetupEnvironments(e => e.UseInstance(new UtilitiesEnvironment()))
                ;
        }
    }
}
