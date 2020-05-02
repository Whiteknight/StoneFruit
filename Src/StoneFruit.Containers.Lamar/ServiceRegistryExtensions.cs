using System;
using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace StoneFruit.Containers.Lamar
{
    public static class ServiceRegistryExtensions
    {
        public static ServiceRegistry SetupEngine<TEnvironment>(this ServiceRegistry services, Action<IEngineBuilder> build)
            where TEnvironment : class
        {
            // TODO: Setup a custom type resolver

            EngineBuilder.SetupEngineRegistrations(services, build);

            services.Scan(s => s.ScanForHandlers());
            services.AddSingleton<IHandlerSource>(provider => new LamarHandlerSource<TEnvironment>(provider, TypeVerbExtractor.DefaultInstance));
            services.AddTransient<TEnvironment>(provider => provider.GetService<IEnvironmentCollection>().Current as TEnvironment);
            
            return services;
        }
    }
}
