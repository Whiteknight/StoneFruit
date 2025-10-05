using System;

namespace StoneFruit.Execution;

/// <summary>
/// Settings object to control the behavior of the engine.
/// </summary>
public class EngineSettings
{
    /// <summary>
    /// Gets or sets the maximum number of commands which can be dispatched by the engine without
    /// some kind of user intervention. Default 20.
    /// </summary>
    public int MaxInputlessCommands { get; set; } = 20;

    /// <summary>
    /// Gets or sets the maximum timeout of a single command before cancellation is requested. The
    /// default is 1 minute.
    /// </summary>
    public TimeSpan MaxExecuteTimeout { get; set; } = TimeSpan.FromMinutes(1);
}
