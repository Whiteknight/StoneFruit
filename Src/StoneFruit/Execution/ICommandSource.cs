namespace StoneFruit.Execution
{
    /// <summary>
    /// Source for commands to be executed
    /// </summary>
    public interface ICommandSource
    {
        /// <summary>
        /// Start the source. Only one source will be started at a time
        /// </summary>
        void Start();

        /// <summary>
        /// Get the next command from the source. If there are no more commands, returns null
        /// </summary>
        /// <returns></returns>
        CommandObjectOrString GetNextCommand();
    }
}