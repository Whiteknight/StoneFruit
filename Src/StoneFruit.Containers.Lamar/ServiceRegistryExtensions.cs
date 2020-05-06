using System;
using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace StoneFruit.Containers.Lamar
{
    public static class ServiceRegistryExtensions
    {
        /// <summary>
        /// Register the Engine and all dependencies with the Lamar container. This method will automatically
        /// scan your solution assemblies for Handler types. If you have additional handler types to add which
        /// are not found during the scan, register them with the ServiceRegistry yourself. This variant is
        /// used when you have an environment object.
        /// </summary>
        /// <typeparam name="TEnvironment"></typeparam>
        /// <param name="services"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public static ServiceRegistry SetupEngine<TEnvironment>(this ServiceRegistry services, Action<IEngineBuilder> build)
            where TEnvironment : class
        {
            SetupEngine(services, build);
            EngineBuilder.SetupExplicitEnvironmentRegistration<TEnvironment>(services);
            return services;
        }

        /// <summary>
        /// Register the Engine and all dependencies with the Lamar container. This method will automatically
        /// scan your solution assemblies for Handler types. If you have additional handler types to add which
        /// are not found during the scan, register them with the ServiceRegistry yourself. This variant is
        /// used when you do not have an environment object.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public static ServiceRegistry SetupEngine(this ServiceRegistry services, Action<IEngineBuilder> build)
        {
            EngineBuilder.SetupEngineRegistrations(services, build);

            services.Scan(s => s.ScanForHandlers());
            services.AddSingleton<IHandlerSource>(provider => new LamarHandlerSource(provider, TypeVerbExtractor.DefaultInstance));

            return services;
        }
    }
}
