using System;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Exceptions;
using StoneFruit.Execution.Handlers;
using StoneFruit.Utility;
using static StoneFruit.Utility.Assert;

namespace StoneFruit;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a type with the DI container whose lifetime is related to the current StoneFruit
    /// environment. When the current environment is changed, these values are cleared and
    /// recreated.
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <param name="services"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Add a handler type. Handler types must implement IHandler or IAsyncHandler.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public static IServiceCollection AddHandler<T>(this IServiceCollection services, string? prefix = null)
        where T : class, IHandlerBase
        => NotNull(services)
            .AddScoped<T>()
            .AddSingleton(RegisteredHandler.Create<T>(prefix));

    /// <summary>
    /// Add a handler type. Handler types must implement IHandler or IAsyncHandler.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="handlerType"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    /// <exception cref="EngineBuildException">The provided handler type does not derive from
    /// IHandler or IAsyncHandler.</exception>
    public static IServiceCollection AddHandler(this IServiceCollection services, Type handlerType, string? prefix = null)
        => NotNull(handlerType).IsHandlerType()
            ? NotNull(services)
                .AddScoped(handlerType)
                .AddSingleton(new RegisteredHandler(handlerType, prefix))
            : throw new EngineBuildException($"Cannot register handler type {handlerType.Name}. It is not derived from {nameof(IHandlerBase)}.");

    /// <summary>
    /// Argument types are mapped from the IArguments instance to be injected into handlers.
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddArgumentType<TArg>(this IServiceCollection services)
        where TArg : class, new()
        => NotNull(services).AddScoped(p => p.GetRequiredService<IArguments>().MapTo<TArg>());

    private static TInterface ThrowOnFactoryReturnsNull<TInterface>()
        where TInterface : class
        => throw new ExecutionException($"Cannot resolve object of type {typeof(TInterface).Name}. Factory method returned null.");
}
