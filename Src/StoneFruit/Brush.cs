using System;
using System.Globalization;
using StoneFruit.Utility;

namespace StoneFruit
{
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
        public static Brush Current => new Brush(System.Console.ForegroundColor, System.Console.BackgroundColor);

        public static implicit operator Brush(ConsoleColor color) => new Brush(color, System.Console.BackgroundColor);

        public readonly void Set()
        {
            System.Console.ForegroundColor = Foreground;
            System.Console.BackgroundColor = Background;
        }

        public override readonly string ToString()
        {
            return Foreground + " on " + Background;
        }

        public readonly string ToString(string fmt)
        {
            if (fmt == "N")
                return ToString();
            if (fmt == "C")
                return Foreground + "," + Background;
            if (fmt == "B")
                return ByteValue.ToString("X");
            throw new Exception("Unknown ToString format " + fmt);
        }

        public byte ByteValue { get; }

        public ConsoleColor Foreground { get; }
        public ConsoleColor Background { get; }

        public readonly Brush Swap() => new Brush(Background, Foreground);

        public readonly Brush Invert() => new Brush(Foreground.Invert(), Background.Invert());

        public readonly bool Equals(Brush other) => other.ByteValue == ByteValue;

        public static Brush Parse(string s)
        {
            if (TryParse(s, out var palette))
                return palette;
            throw new Exception("Value not in a correct format");
        }

        // TODO: Use proper parsers
        public static bool TryParse(string s, out Brush palette)
        {
            string[] parts;
            if (s.Length == 2)
            {
                byte byteValue = Byte.Parse(s, NumberStyles.HexNumber);
                palette = new Brush(byteValue);
                return true;
            }
            if (s.Length == 4 && s.StartsWith("0x"))
            {
                byte byteValue = Byte.Parse(s.Substring(2), NumberStyles.HexNumber);
                palette = new Brush(byteValue);
                return true;
            }
            if (s.Contains(" on "))
                parts = s.Split(new string[] { " on " }, StringSplitOptions.None);
            else if (s.Contains(","))
                parts = s.Split(new string[] { "," }, StringSplitOptions.None);
            else
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
