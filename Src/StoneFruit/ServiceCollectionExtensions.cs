using System;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Exceptions;
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
            var currentEnvironment = p.GetRequiredService<IEnvironment>();
            var existing = currentEnvironment.GetCached<TInterface>();
            if (existing.IsSuccess)
                return existing.GetValueOrThrow();
            var value = factory(p, currentEnvironment.Name)
                ?? throw new ExecutionException($"Cannot resolve object of type {typeof(TInterface).Name}. Factory method returned null.");
            currentEnvironment.CacheInstance(value);
            return value;
        });
    }
}
