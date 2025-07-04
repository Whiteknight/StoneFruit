using System;
using Microsoft.Extensions.DependencyInjection;

namespace StoneFruit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseStonefruit(this IServiceCollection services, Action<IEngineBuilder> build)
    {
        EngineBuilder.SetupEngineRegistrations(services, build);
        return services;
    }
}
