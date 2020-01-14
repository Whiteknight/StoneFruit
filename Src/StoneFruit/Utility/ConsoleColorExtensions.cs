using System;
using System.Collections.Generic;
using System.Text;

namespace StoneFruit.Utility
{
    public static class ConsoleColorExtensions
    {
        public static ConsoleColor MakeDark(this ConsoleColor c)
        {
            return (ConsoleColor)(((int)c) & 0x07);
        }

        public static ConsoleColor MakeBright(this ConsoleColor c)
        {
            return (ConsoleColor)(((int)c) | 0x08);
        }

        public static bool IsGrayscale(this ConsoleColor cc)
        {
            return cc == ConsoleColor.Black || cc == ConsoleColor.Gray || cc == ConsoleColor.DarkGray || cc == ConsoleColor.White;
        }

        public static ConsoleColor Invert(this ConsoleColor cc)
        {
            return (ConsoleColor)(~((byte)cc) & 0x0F);
        }
    }
}
