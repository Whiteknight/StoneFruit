using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace StoneFruit.Execution.IO;

public sealed class LoggingConfiguration
{
    private readonly IReadOnlyDictionary<LogLevel, Brush> _colors;

    public LoggingConfiguration(LogLevel logLevel, IReadOnlyDictionary<LogLevel, Brush> colors)
    {
        LogLevel = logLevel;
        _colors = colors;
    }

    public LogLevel LogLevel { get; }

    public static Dictionary<LogLevel, Brush> GetColorsLookup()
        => new Dictionary<LogLevel, Brush>
        {
            [LogLevel.Trace] = ConsoleColor.Magenta,
            [LogLevel.Debug] = ConsoleColor.Blue,
            [LogLevel.Information] = ConsoleColor.Green,
            [LogLevel.Warning] = ConsoleColor.Yellow,
            [LogLevel.Error] = ConsoleColor.Red,
            [LogLevel.Critical] = new Brush(ConsoleColor.Black, ConsoleColor.Red),
        };

    public Brush GetColor(LogLevel logLevel)
        => _colors.MaybeGetValue(logLevel).GetValueOrDefault(Brush.Default);
}
