using System;
using StoneFruit.Utility;

namespace StoneFruit.Execution.Output
{
    /// <summary>
    /// Value to represent a foreground/background color combination for writing on the
    /// windows terminal.
    /// </summary>
    public struct Brush
        : IEquatable<Brush>
    {
        public Brush(byte byteValue)
        {
            ByteValue = byteValue;
            Foreground = ConsoleColorUtilities.GetForeground(byteValue);
            Background = ConsoleColorUtilities.GetBackground(byteValue);
        }

        public Brush(ConsoleColor foreground, ConsoleColor background)
        {
            Background = background;
            Foreground = foreground;
            ByteValue = ConsoleColorUtilities.ColorsToByte(foreground, background);
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
                return ((int)Foreground * 397) ^ (int)Background;
            }
        }

        public static bool operator ==(Brush a, Brush b) => a.Equals(b);

        public static bool operator !=(Brush a, Brush b) => !a.Equals(b);
    }
}
