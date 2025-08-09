using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution.Handlers;

namespace StoneFruit;

public static class IHandlerSetupInstanceMethodExtensions
{
    /// <summary>
    /// Use the public methods of an instance object as handlers.
    /// </summary>
    /// <param name="setup"></param>
    /// <param name="instance"></param>
    /// <param name="group"></param>
    /// <returns></returns>
    public static IHandlerSetup UsePublicInstanceMethodsAsHandlers(this IHandlerSetup setup, object instance, string? group = null)
        => setup.AddSource(provider =>
            new InstanceMethodHandlerSource(
                instance,
                provider.GetRequiredService<IHandlerMethodInvoker>(),
                provider.GetRequiredService<IVerbExtractor>(),
                group
            )
        );
}
