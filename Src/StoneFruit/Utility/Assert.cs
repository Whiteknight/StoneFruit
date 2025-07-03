using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace StoneFruit.Utility;

public static class Assert
{
    /// <summary>
    /// Throws an exception if the argument value is null.
    /// </summary>
    /// <param name="arg"></param>
    /// <param name="name"></param>
    public static T NotNull<T>([NotNull] T arg, [CallerArgumentExpression(nameof(arg))] string? name = null)
        => arg ?? throw new ArgumentNullException(name);

    /// <summary>
    /// Throws an exception if the argument string value is null or empty.
    /// </summary>
    /// <param name="arg"></param>
    /// <param name="name"></param>
    public static string NotNullOrEmpty([NotNull] string arg, [CallerArgumentExpression(nameof(arg))] string? name = null)
    {
        if (string.IsNullOrEmpty(arg))
            throw new ArgumentNullException(name);
        return arg;
    }
}
