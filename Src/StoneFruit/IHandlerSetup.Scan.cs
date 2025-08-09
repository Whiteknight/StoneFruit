using System.Reflection;

namespace StoneFruit;

public static class IHandlerSetupScanExtensions
{
    public static IHandlerSetup ScanHandlersFromEntryAssembly(this IHandlerSetup setup, string? prefix = null)
        => setup.ScanAssemblyForHandlers(Assembly.GetEntryAssembly(), prefix);

    public static IHandlerSetup ScanHandlersFromCurrentAssembly(this IHandlerSetup setup, string? prefix = null)
        => setup.ScanAssemblyForHandlers(Assembly.GetCallingAssembly(), prefix);

    public static IHandlerSetup ScanHandlersFromAssemblyContaining<T>(this IHandlerSetup setup, string? prefix = null)
        => setup.ScanAssemblyForHandlers(typeof(T).Assembly, prefix);
}
