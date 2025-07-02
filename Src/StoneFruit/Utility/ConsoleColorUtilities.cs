using System;

namespace StoneFruit.Utility;

public static class ConsoleColorUtilities
{
    /// <summary>
    /// Convert two colors into a single compact byte representation
    /// </summary>
    /// <param name="foreground"></param>
    /// <param name="background"></param>
    /// <returns></returns>
    public static byte ColorsToByte(ConsoleColor foreground, ConsoleColor background)
        => (byte)(((byte)foreground << 4) | (byte)background);

    /// <summary>
    /// Get the foreground color from a compact byte representation
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static ConsoleColor GetForeground(byte b) => (ConsoleColor)((b & 0xF0) >> 4);

    /// <summary>
    /// Get the background color from a compact byte representation
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static ConsoleColor GetBackground(byte b) => (ConsoleColor)(b & 0x0F);
}
