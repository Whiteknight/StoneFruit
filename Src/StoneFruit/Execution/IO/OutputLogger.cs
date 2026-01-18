using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace StoneFruit.Execution.IO;

// Default logging implementation which wraps IOutput.
// We want this to be a thin wrapper around IOutput and not try to get too clever with too many
// features. The user can provide their own ILogger implementation if they want something more
// detailed.
public sealed class OutputLogger<T> : ILogger<T>
{
    private readonly IOutput _output;
    private readonly LoggingConfiguration _configuration;
    private int _stateIndex;
    private Dictionary<int, object>? _state;

    public OutputLogger(IOutput output, LoggingConfiguration configuration)
    {
        _output = output;
        _configuration = configuration;
        _stateIndex = 1;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        _state ??= [];
        var currentIndex = _stateIndex;
        _stateIndex++;
        _state.Add(currentIndex, state);
        return new StateToken(_state, currentIndex);
    }

    public bool IsEnabled(LogLevel logLevel) => _configuration.LogLevel <= logLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;
        var color = _configuration.GetColor(logLevel);
        var formatted = formatter(state, exception);
        _output.WriteLine(formatted, color);
    }

    private sealed record StateToken(Dictionary<int, object> State, int Index) : IDisposable
    {
        public void Dispose() => State.Remove(Index);
    }
}
