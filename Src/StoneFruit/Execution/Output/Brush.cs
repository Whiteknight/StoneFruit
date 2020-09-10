using StoneFruit.Utility;
using System;
using System.Globalization;
using System.Linq;

namespace StoneFruit.Execution.Output
{
    /// <summary>
    /// Value to represent a foreground/background color combination for writing on the
    /// windows terminal.
    /// </summary>
    public struct Brush : IEquatable<Brush>
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

        public static Brush Current
            => new Brush(Console.ForegroundColor, Console.BackgroundColor);

        public static implicit operator Brush(ConsoleColor color)
            => new Brush(color, Console.BackgroundColor);

        public readonly void Set()
        {
            Console.ForegroundColor = Foreground;
            Console.BackgroundColor = Background;
        }

        public override readonly string ToString() => ToString("N");

        public readonly string ToString(string fmt)
        {
            if (fmt == "N")
                return $"{Foreground} on {Background}";
            if (fmt == "C")
                return $"{Foreground},{Background}";
            if (fmt == "B")
                return ByteValue.ToString("X");
            throw new Exception("Unknown ToString format " + fmt);
        }

        public byte ByteValue { get; }

        public ConsoleColor Foreground { get; }

        public ConsoleColor Background { get; }

        public readonly Brush Swap() => new Brush(Background, Foreground);

        public readonly Brush Invert() => new Brush(Foreground.Invert(), Background.Invert());

        public override bool Equals(object obj)
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

        public static Brush Parse(string s)
        {
            if (TryParse(s, out var palette))
                return palette;
            throw new Exception("Value not in a correct format");
        }

        public static bool TryParse(string s, out Brush palette)
        {
            // If length==2 it's a hex byte. Parse that out.
            if (s.Length == 2)
            {
                var byteValue = byte.Parse(s, NumberStyles.HexNumber);
                palette = new Brush(byteValue);
                return true;
            }

            // If length==4 it might be a hex byte with "0x" prefix
            if (s.Length == 4 && s.StartsWith("0x"))
            {
                var byteValue = byte.Parse(s.Substring(2), NumberStyles.HexNumber);
                palette = new Brush(byteValue);
                return true;
            }

            string[] parts = null;

            // Format "ColorName on ColorName" split
            if (s.Contains(" on "))
                parts = s.Split(new[] { " on " }, StringSplitOptions.None).Select(x => x.Trim()).ToArray();

            // Format "ColorName,ColorName" split
            else if (s.Contains(","))
                parts = s.Split(new[] { "," }, StringSplitOptions.None).Select(x => x.Trim()).ToArray();

            // Otherwise it's nothing we can deal with. Bail out.
            if (parts == null || parts.Length != 2)
            {
                palette = new Brush();
                return false;
            }

            try
            {
                var foreground = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), parts[0]);
                var background = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), parts[1]);
                palette = new Brush(foreground, background);
                return true;
            }
            catch
            {
                palette = new Brush();
                return false;
            }
        }
    }
}
