using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Handlers;

namespace StoneFruit;

public static class InstanceMethodsExtensions
{
    /// <summary>
    /// Use the public methods of an instance object as handlers.
    /// </summary>
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
                group
            )
        );

    public static IHandlerSetup UsePublicInstanceMethodsAsHandlers<T>(this IHandlerSetup setup, string? group = null)
        where T : class
    {
        setup.Services.AddScoped<T>();
        setup.AddSource(provider =>
            new InstanceMethodHandlerSource<T>(
                () => provider.GetRequiredService<T>(),
                provider.GetRequiredService<IHandlerMethodInvoker>(),
                provider.GetRequiredService<IVerbExtractor>(),
                group
            )
        );
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
                section.Name
            )
        );
        return section;
    }

    public static HandlerSectionSetup UsePublicInstanceMethodsAsHandlers<T>(this HandlerSectionSetup section)
        where T : class
    {
        section.Services.AddScoped<T>();
        section.Handlers.AddSource(provider =>
            new InstanceMethodHandlerSource<T>(
                () => provider.GetRequiredService<T>(),
                provider.GetRequiredService<IHandlerMethodInvoker>(),
                new PrefixingVerbExtractor(section.Name, provider.GetRequiredService<IVerbExtractor>()),
                section.Name
            )
        );
        return section;
    }
}
