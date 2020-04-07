using System;

namespace StoneFruit.Execution
{
    public class EngineSettings
    {
        /// <summary>
        /// The maximum number of commands which can be dispatched by the engine without
        /// some kind of user intervention. Default 20.
        /// </summary>
        public int MaxInputlessCommands { get; set; } = 20;

        /// <summary>
        /// The maximum timeout of a single command before cancellation is requested
        /// </summary>
        public TimeSpan MaxExecuteTimeout { get; set; } = TimeSpan.FromMinutes(1);
    }
}