using System;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Exceptions;
using StoneFruit.Execution.Handlers;
using StoneFruit.Utility;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

public static class ServiceCollectionExtensions
{
    // TODO: Want a transient version of this method with no caching.
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

    public static IServiceCollection AddHandler<T>(this IServiceCollection services, string? prefix = null)
        where T : class, IHandlerBase
        => NotNull(services)
            .AddScoped<T>()
            .AddSingleton(RegisteredHandler.Create<T>(prefix));

    public static IServiceCollection AddHandler(this IServiceCollection services, Type handlerType, string? prefix = null)
    {
        NotNull(services);
        NotNull(handlerType);
        if (!handlerType.IsHandlerType())
            throw new EngineBuildException($"Cannot register handler type {handlerType.Name}. It is not derived from IHandlerBase.");
        return services.AddScoped(handlerType)
            .AddSingleton(new RegisteredHandler(handlerType, prefix));
    }
}
