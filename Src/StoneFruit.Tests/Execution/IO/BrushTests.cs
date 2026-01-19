using System;
using System.Drawing;
using AwesomeAssertions;
using NUnit.Framework;
using StoneFruit.Execution.IO;

namespace StoneFruit.Tests.Execution.IO;

public class BrushTests
{
    [Test]
    public void Parse_TwoColors_ReturnsForegroundAndBackground()
    {
        Brush result = Brush.Parse("White,Red");
        (ConsoleColor fg, ConsoleColor bg) = result.GetConsoleColors();
        fg.Should().Be(ConsoleColor.White);
        bg.Should().Be(ConsoleColor.Red);
    }

    [Test]
    public void Parse_IsCaseInsensitive()
    {
        Brush resultLower = Brush.Parse("yellow");
        (ConsoleColor fgLower, ConsoleColor bgLower) = resultLower.GetConsoleColors();
        fgLower.Should().Be(ConsoleColor.Yellow);

        Brush resultMixed = Brush.Parse("DaRkGrEeN,BlAcK");
        (ConsoleColor fgMixed, ConsoleColor bgMixed) = resultMixed.GetConsoleColors();
        fgMixed.Should().Be(ConsoleColor.DarkGreen);
        bgMixed.Should().Be(ConsoleColor.Black);
    }

    [Test]
    public void Parse_Invalid_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Brush.Parse("NotAColor"));
        Assert.Throws<ArgumentException>(() => Brush.Parse("Red,NotAColor"));
    }

    [TestCase(ConsoleColor.Black, 0x000000)]
    [TestCase(ConsoleColor.DarkBlue, 0x000080)]
    [TestCase(ConsoleColor.DarkGreen, 0x008000)]
    [TestCase(ConsoleColor.DarkCyan, 0x008080)]
    [TestCase(ConsoleColor.DarkRed, 0x800000)]
    [TestCase(ConsoleColor.DarkMagenta, 0x800080)]
    [TestCase(ConsoleColor.DarkYellow, 0x808000)]
    [TestCase(ConsoleColor.Gray, 0xC0C0C0)]
    [TestCase(ConsoleColor.DarkGray, 0x808080)]
    [TestCase(ConsoleColor.Blue, 0x0000FF)]
    [TestCase(ConsoleColor.Green, 0x00FF00)]
    [TestCase(ConsoleColor.Cyan, 0x00FFFF)]
    [TestCase(ConsoleColor.Red, 0xFF0000)]
    [TestCase(ConsoleColor.Magenta, 0xFF00FF)]
    [TestCase(ConsoleColor.Yellow, 0xFFFF00)]
    [TestCase(ConsoleColor.White, 0xFFFFFF)]
    public void ToColor_MapsConsoleColorToExpectedRgb(ConsoleColor input, int expectedRgb)
    {
        Color actual = Brush.ToColor(input);
        int actualRgb = actual.ToArgb() & 0xFFFFFF;
        actualRgb.Should().Be(expectedRgb);
    }

    [Test]
    public void ToColor_OutOfRangeValue_ReturnsDefaultGray()
    {
        // Use a value outside the ConsoleColor enum range to exercise the default branch
        var outOfRange = (ConsoleColor)999;
        Color actual = Brush.ToColor(outOfRange);
        int actualRgb = actual.ToArgb() & 0xFFFFFF;
        actualRgb.Should().Be(0xC0C0C0);
    }
}
