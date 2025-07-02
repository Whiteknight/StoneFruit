using System;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Handlers;

namespace StoneFruit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseStonefruit(this IServiceCollection services, Action<IEngineBuilder> build)
    {
        EngineBuilder.SetupEngineRegistrations(services, build);
        services.AddSingleton<IHandlerSource>(provider => new ServiceProviderHandlerSource(services, provider, provider.GetRequiredService<IVerbExtractor>()));
        return services;
    }
}
