using System;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Environments;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseStonefruit(this IServiceCollection services, Action<IEngineBuilder> build)
    {
        EngineBuilder.SetupEngineRegistrations(services, build);
        return services;
    }

    public static IServiceCollection AddPerEnvironment<TInterface>(this IServiceCollection services, Func<IServiceProvider, string, TInterface> factory)
        where TInterface : class
    {
        NotNull(factory);
        return NotNull(services).AddScoped(p =>
        {
            var environments = p.GetRequiredService<IEnvironmentCollection>() as EnvironmentCollection
                ?? throw new InvalidOperationException("Could not get EnvironmentCollection");
            var existing = environments.GetCached<TInterface>();
            if (existing.IsSuccess)
                return existing.GetValueOrThrow();
            var value = factory(p, environments.GetCurrentName().GetValueOrThrow())
                ?? throw new InvalidOperationException($"Cannot resolve object of type {typeof(TInterface).Name}. Factory method returned null.");
            environments.CacheInstance(value);
            return value;
        });
    }
}
