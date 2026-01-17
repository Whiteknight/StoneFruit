using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StoneFruit.Execution.Handlers;
using StoneFruit.Execution.IO;

namespace StoneFruit;

/// <summary>
/// Tag type for classes which provide instance methods as handlers. Used for auto-wireup/scanning.
/// </summary>
public interface IInstanceMethodHandlers
{
}

public static class InstanceMethods
{
    public static bool IsInstanceMethodHandlersType(this Type type)
        => type?.IsPublic == true && !type.IsAbstract && typeof(IInstanceMethodHandlers).IsAssignableFrom(type);

    /// <summary>
    /// Use the public methods of an instance object as handlers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="setup"></param>
    /// <param name="instance"></param>
    /// <param name="group"></param>
    /// <returns></returns>
    public static IHandlerSetup UsePublicInstanceMethodsAsHandlers<T>(this IHandlerSetup setup, T instance, string? group = null)
        => setup.AddSource(provider =>
            new InstanceMethodHandlerSource<T>(
                () => instance,
                provider.GetRequiredService<IHandlerMethodInvoker>(),
                provider.GetRequiredService<IVerbExtractor>(),
                provider.GetRequiredService<IObjectOutputWriter>(),
                group
            )
        );

    public static IHandlerSetup UsePublicInstanceMethodsAsHandlers<T>(this IHandlerSetup setup, string? group = null)
        where T : class
    {
        setup.Services.TryAddScoped<T>();
        setup.AddSource(provider =>
            new InstanceMethodHandlerSource<T>(
                () => provider.GetRequiredService<T>(),
                provider.GetRequiredService<IHandlerMethodInvoker>(),
                provider.GetRequiredService<IVerbExtractor>(),
                provider.GetRequiredService<IObjectOutputWriter>(),
                group
            )
        );
        return setup;
    }

    public static IHandlerSetup UsePublicInstanceMethodsAsHandlers(this IHandlerSetup setup, Type type, string? group = null)
    {
        var method = typeof(InstanceMethods)
            .GetMethod(nameof(UsePublicInstanceMethodsAsHandlers), [typeof(IHandlerSetup), typeof(string)])!
            .MakeGenericMethod(type);
        method.Invoke(null, [setup, group]);
        return setup;
    }

    public static HandlerSectionSetup UsePublicInstanceMethodsAsHandlers<T>(this HandlerSectionSetup section, T instance)
        where T : class
    {
        section.Handlers.AddSource(provider =>
            new InstanceMethodHandlerSource<T>(
                () => instance,
                provider.GetRequiredService<IHandlerMethodInvoker>(),
                new PrefixingVerbExtractor(section.Name, provider.GetRequiredService<IVerbExtractor>()),
                provider.GetRequiredService<IObjectOutputWriter>(),
                section.Name
            )
        );
        return section;
    }

    public static HandlerSectionSetup UsePublicInstanceMethodsAsHandlers<T>(this HandlerSectionSetup section)
        where T : class
    {
        section.Services.TryAddScoped<T>();
        section.Handlers.AddSource(provider =>
            new InstanceMethodHandlerSource<T>(
                () => provider.GetRequiredService<T>(),
                provider.GetRequiredService<IHandlerMethodInvoker>(),
                new PrefixingVerbExtractor(section.Name, provider.GetRequiredService<IVerbExtractor>()),
                provider.GetRequiredService<IObjectOutputWriter>(),
                section.Name
            )
        );
        return section;
    }
}
