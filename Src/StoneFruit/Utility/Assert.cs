using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace StoneFruit.Utility;

public static class Assert
{
    /// <summary>
    /// Throws an exception if the argument value is null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg"></param>
    /// <param name="name"></param>
    [return: NotNull]
    public static T NotNull<T>([NotNull] T arg, [CallerArgumentExpression(nameof(arg))] string? name = null)
        => arg ?? throw new ArgumentNullException(name);

    /// <summary>
    /// Throws an exception if the argument string value is null or empty.
    /// </summary>
    /// <param name="arg"></param>
    /// <param name="name"></param>
    [return: NotNull]
    public static string NotNullOrEmpty([NotNull] string arg, [CallerArgumentExpression(nameof(arg))] string? name = null)
        => !string.IsNullOrEmpty(arg)
            ? arg
            : throw new ArgumentNullException(name);

    [return: NotNull]
    public static T[] NotNullOrEmpty<T>([NotNull] T[] array, [CallerArgumentExpression(nameof(array))] string? name = null)
        => array == null || array.Length == 0
            ? throw new ArgumentException("Array may not be null or empty", name)
            : array;
}
