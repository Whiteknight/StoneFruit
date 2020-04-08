namespace StoneFruit.Execution
{
    /// <summary>
    /// Source for commands to be executed
    /// </summary>
    public interface ICommandSource
    {
        /// <summary>
        /// Get the next command from the source. If there are no more commands, returns null
        /// </summary>
        /// <returns></returns>
        CommandOrString GetNextCommand();
    }
}