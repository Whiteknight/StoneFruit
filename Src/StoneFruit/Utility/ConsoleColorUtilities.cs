using System;

namespace StoneFruit.Utility
{
    public static class ConsoleColorUtilities
    {
        public static byte ColorsToByte(ConsoleColor foreground, ConsoleColor background) 
            => (byte)(((byte)foreground << 4) | (byte)background);

        public static ConsoleColor GetForeground(byte b) => (ConsoleColor)((b & 0xF0) >> 4);

        public static ConsoleColor GetBackground(byte b) => (ConsoleColor)(b & 0x0F);
    }
}
