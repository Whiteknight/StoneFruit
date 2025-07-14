using System;
using StoneFruit.Utility;

namespace StoneFruit.Execution.IO;

/// <summary>
/// Value to represent a foreground/background color combination for writing on the
/// windows terminal.
/// </summary>
public readonly struct Brush
    : IEquatable<Brush>
{
    public Brush(byte byteValue)
    {
        ByteValue = byteValue;
        Foreground = GetForeground(byteValue);
        Background = GetBackground(byteValue);
    }

    public Brush(ConsoleColor foreground, ConsoleColor background)
    {
        Background = background;
        Foreground = foreground;
        ByteValue = ColorsToByte(foreground, background);
    }

    public Brush(ConsoleColor foreground)
        : this(foreground, Console.BackgroundColor)
    {
    }

    public static Brush Default => new Brush(ConsoleColor.Gray, ConsoleColor.Black);

    public static Brush Current => new Brush(Console.ForegroundColor, Console.BackgroundColor);

    public static implicit operator Brush(ConsoleColor color) => new Brush(color, Console.BackgroundColor);

    public readonly void Set()
    {
        Console.ForegroundColor = Foreground;
        Console.BackgroundColor = Background;
    }

    public byte ByteValue { get; }

    public ConsoleColor Foreground { get; }

    public ConsoleColor Background { get; }

    public readonly Brush Swap() => new Brush(Background, Foreground);

    public readonly Brush Invert() => new Brush(Foreground.Invert(), Background.Invert());

    public override bool Equals(object? obj)
        => obj is Brush other && Equals(other);

    public readonly bool Equals(Brush other)
        => Foreground == other.Foreground && Background == other.Background;

    public override int GetHashCode()
    {
        unchecked
        {
            return (int)Foreground * 397 ^ (int)Background;
        }
    }

    public static bool operator ==(Brush a, Brush b) => a.Equals(b);

    public static bool operator !=(Brush a, Brush b) => !a.Equals(b);

    private static byte ColorsToByte(ConsoleColor foreground, ConsoleColor background)
        => (byte)(((byte)foreground << 4) | (byte)background);

    private static ConsoleColor GetForeground(byte b) => (ConsoleColor)((b & 0xF0) >> 4);

    private static ConsoleColor GetBackground(byte b) => (ConsoleColor)(b & 0x0F);
}
