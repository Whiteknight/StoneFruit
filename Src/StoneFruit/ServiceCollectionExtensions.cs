using System;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using StoneFruit.Execution.Handlers;

namespace StoneFruit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupEngine(this IServiceCollection services, Action<IEngineBuilder> build)
    {
        EngineBuilder.SetupEngineRegistrations(services, build, () => services.Scan(s => s.AddType<IHandlerBase>()));
        services.AddSingleton<IHandlerSource>(provider => new ServiceProviderHandlerSource(services, provider, provider.GetRequiredService<IVerbExtractor>()));
        return services;
    }
}
