using System;
using System.Collections.Generic;
using System.Text;

namespace StoneFruit.Utility
{
    public static class ConsoleColorUtilities
    {
        public static byte ColorsToByte(ConsoleColor foreground, ConsoleColor background)
        {
            return (byte)(((byte)foreground << 4) | (byte)background);
        }

        public static ConsoleColor GetForeground(byte b)
        {
            return (ConsoleColor)((b & 0xF0) >> 4);
        }

        public static ConsoleColor GetBackground(byte b)
        {
            return (ConsoleColor)(b & 0x0F);
        }
    }
}
