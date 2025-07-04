using System;

namespace StoneFruit.Utility;

public static class ConsoleColorExtensions
{
    public static ConsoleColor Invert(this ConsoleColor cc) => (ConsoleColor)(~((byte)cc) & 0x0F);
}
