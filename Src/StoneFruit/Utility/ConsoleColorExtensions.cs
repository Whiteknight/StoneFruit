using System;

namespace StoneFruit.Utility
{
    public static class ConsoleColorExtensions
    {
        public static ConsoleColor MakeDark(this ConsoleColor c) => (ConsoleColor)(((int)c) & 0x07);

        public static ConsoleColor MakeBright(this ConsoleColor c) => (ConsoleColor)(((int)c) | 0x08);

        public static bool IsGrayscale(this ConsoleColor cc) 
            => cc == ConsoleColor.Black || cc == ConsoleColor.Gray || cc == ConsoleColor.DarkGray || cc == ConsoleColor.White;

        public static ConsoleColor Invert(this ConsoleColor cc) => (ConsoleColor)(~((byte)cc) & 0x0F);
    }
}
