using System;
using System.Drawing;

namespace StoneFruit.Execution.IO;

/// <summary>
/// Value to represent a foreground/background color combination for writing on the
/// windows terminal.
/// </summary>
public readonly struct Brush
    : IEquatable<Brush>
{
    private readonly Color _foreground;
    private readonly Color _background;
    private readonly ConsoleColor _ccForeground;
    private readonly ConsoleColor _ccBackground;

    public Brush(Color foreground, Color background)
    {
        _foreground = foreground;
        _background = background;
        _ccForeground = ToConsoleColor(_foreground);
        _ccBackground = ToConsoleColor(_background);
    }

    public Brush(ConsoleColor foreground, ConsoleColor background)
    {
        _ccForeground = foreground;
        _ccBackground = background;
        _background = ToColor(background);
        _foreground = ToColor(foreground);
    }

    public Brush(ConsoleColor foreground)
        : this(foreground, Console.BackgroundColor)
    {
    }

    public static Brush Default => new Brush(ConsoleColor.Gray, ConsoleColor.Black);

    public static Brush Current => new Brush(Console.ForegroundColor, Console.BackgroundColor);

    public static implicit operator Brush(ConsoleColor color) => new Brush(color, Console.BackgroundColor);

    public static Brush Parse(string name)
    {
        var parts = name.Split(',');
        if (parts.Length == 2)
        {
            var fg = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), parts[0], true);
            var bg = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), parts[1], true);
            return new Brush(fg, bg);
        }

        var foreground = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), name, true);
        return new Brush(foreground, Console.BackgroundColor);
    }

    public readonly Brush Swap() => new Brush(_background, _foreground);

    public (ConsoleColor Foreground, ConsoleColor Background) GetConsoleColors()
        => (_ccForeground, _ccBackground);

    public (Color Foreground, Color Background) GetColors()
        => (_foreground, _background);

    public override bool Equals(object? obj)
        => obj is Brush other && Equals(other);

    public readonly bool Equals(Brush other)
        => _foreground == other._foreground && _background == other._background;

    public override int GetHashCode() => HashCode.Combine(_foreground, _background);

    public static bool operator ==(Brush a, Brush b) => a.Equals(b);

    public static bool operator !=(Brush a, Brush b) => !a.Equals(b);

    public static ConsoleColor ToConsoleColor(Color c)
    {
        int index = (c.R > 150 || c.G > 150 || c.B > 150) ? 8 : 0; // Bright bit
        index |= (c.R > 64) ? 4 : 0; // Red bit
        index |= (c.G > 64) ? 2 : 0; // Green bit
        index |= (c.B > 64) ? 1 : 0; // Blue bit
        return (ConsoleColor)index;
    }

    public static Color ToColor(ConsoleColor c)
    {
        var colorInt = c switch
        {
            ConsoleColor.Black => 0x000000,
            ConsoleColor.DarkBlue => 0x000080,
            ConsoleColor.DarkGreen => 0x008000,
            ConsoleColor.DarkCyan => 0x008080,
            ConsoleColor.DarkRed => 0x800000,
            ConsoleColor.DarkMagenta => 0x800080,
            ConsoleColor.DarkYellow => 0x808000,
            ConsoleColor.Gray => 0xC0C0C0,
            ConsoleColor.DarkGray => 0x808080,
            ConsoleColor.Blue => 0x0000FF,
            ConsoleColor.Green => 0x00FF00,
            ConsoleColor.Cyan => 0x00FFFF,
            ConsoleColor.Red => 0xFF0000,
            ConsoleColor.Magenta => 0xFF00FF,
            ConsoleColor.Yellow => 0xFFFF00,
            ConsoleColor.White => 0xFFFFFF,
            _ => 0xC0C0C0
        };
        return Color.FromArgb(colorInt);
    }
}
