namespace StoneFruit.Execution
{
    /// <summary>
    /// Type of engine execution.
    /// </summary>
    public enum EngineRunMode
    {
        /// <summary>
        /// The engine is not running
        /// </summary>
        Idle,

        /// <summary>
        /// The engine is running headless, without prompting the user for input. The engine will
        /// exit when all queued commands have been executed and the queue is empty.
        /// </summary>
        Headless,

        /// <summary>
        /// The engine is running in interactive mode. The user is prompted for input when the
        /// command queue is empty. The engine only exits when the user uses the 'exit' command.
        /// </summary>
        Interactive
    }
}
