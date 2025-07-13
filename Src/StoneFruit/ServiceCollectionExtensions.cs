using System;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Exceptions;
using StoneFruit.Execution.Handlers;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

public static class ServiceCollectionExtensions
{
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

    public static IServiceCollection AddHandler<T>(this IServiceCollection services)
        where T : class, IHandlerBase
        => NotNull(services)
            .AddScoped<T>()
            .AddSingleton(RegisteredHandler.Create<T>());

    public static IServiceCollection AddHandler(this IServiceCollection services, Type handlerType)
    {
        NotNull(services);
        NotNull(handlerType);
        if (!handlerType.IsAssignableTo(typeof(IHandlerBase)))
            throw new EngineBuildException($"Cannot register handler type {handlerType.Name}. It is not derived from IHandlerBase.");
        return services.AddScoped(handlerType)
            .AddSingleton(new RegisteredHandler(handlerType));
    }
}
