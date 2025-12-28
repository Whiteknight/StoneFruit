using System;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Exceptions;
using StoneFruit.Execution.Handlers;
using StoneFruit.Utility;
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
                ?? ThrowOnFactoryReturnsNull<TInterface>();
            currentEnvironment.CacheInstance(value);
            return value;
        });
    }

    private static TInterface ThrowOnFactoryReturnsNull<TInterface>()
        where TInterface : class
        => throw new ExecutionException($"Cannot resolve object of type {typeof(TInterface).Name}. Factory method returned null.");

    public static IServiceCollection AddHandler<T>(this IServiceCollection services, string? prefix = null)
        where T : class, IHandlerBase
        => NotNull(services)
            .AddScoped<T>()
            .AddSingleton(RegisteredHandler.Create<T>(prefix));

    public static IServiceCollection AddHandler(this IServiceCollection services, Type handlerType, string? prefix = null)
        => NotNull(handlerType).IsHandlerType()
            ? NotNull(services)
                .AddScoped(handlerType)
                .AddSingleton(new RegisteredHandler(handlerType, prefix))
            : throw new EngineBuildException($"Cannot register handler type {handlerType.Name}. It is not derived from {nameof(IHandlerBase)}.");

    public static IServiceCollection AddHandlerArgumentType<TArg>(this IServiceCollection services)
        where TArg : class, new()
        => NotNull(services).AddScoped(p => p.GetRequiredService<IArguments>().MapTo<TArg>());
}
