using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace StoneFruit.Execution.IO;

public sealed class OutputLogger<T> : ILogger<T>
{
    private readonly IOutput _output;
    private int _stateIndex;
    private Dictionary<int, object>? _state;

    public OutputLogger(IOutput output)
    {
        _output = output;
        _stateIndex = 1;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        if (_state == null)
            _state = [];
        var currentIndex = _stateIndex;
        _stateIndex++;
        _state.Add(currentIndex, state);
        return new StateToken(_state, currentIndex);
    }

    // TODO: Need configuration during Engine build-up
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        // TODO: How do we use _state here?
        var (_, color) = Decode(logLevel);
        var formatted = formatter(state, exception);
        _output.Color(color).WriteLine(formatted);
    }

    private static (string Abbreviation, Brush Color) Decode(LogLevel logLevel)
        => logLevel switch
        {
            LogLevel.Trace => ("trce", ConsoleColor.Magenta),
            LogLevel.Debug => ("debg", ConsoleColor.Blue),
            LogLevel.Information => ("info", ConsoleColor.Green),
            LogLevel.Warning => ("warn", ConsoleColor.Yellow),
            LogLevel.Error => ("errr", ConsoleColor.Red),
            LogLevel.Critical => ("crit", new Brush(ConsoleColor.Black, ConsoleColor.Red)),
            _ => ("none", Brush.Default)
        };

    private sealed record StateToken(Dictionary<int, object> State, int Index) : IDisposable
    {
        public void Dispose() => State.Remove(Index);
    }
}
