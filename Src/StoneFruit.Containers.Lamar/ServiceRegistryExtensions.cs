using System;
using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace StoneFruit.Containers.Lamar
{
    /// <summary>
    /// Register the Engine and all dependencies with the Lamar container. This method will automatically
    /// scan your solution assemblies for Handler types. If you have additional handler types to add which
    /// are not found during the scan, register them with the ServiceRegistry yourself.
    /// </summary>
    public static class ServiceRegistryExtensions
    {
        public static ServiceRegistry SetupEngine<TEnvironment>(this ServiceRegistry services, Action<IEngineBuilder> build)
            where TEnvironment : class
        {
            EngineBuilder.SetupEngineRegistrations(services, build);

            services.Scan(s => s.ScanForHandlers());
            services.AddSingleton<IHandlerSource>(provider => new LamarHandlerSource<TEnvironment>(provider, TypeVerbExtractor.DefaultInstance));
            services.AddTransient<TEnvironment>(provider => provider.GetService<IEnvironmentCollection>().Current as TEnvironment);
            
            return services;
        }
    }
}
