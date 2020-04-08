namespace StoneFruit.Execution.CommandSources
{
    /// <summary>
    /// Null-Object implementation of ICommandSource. Returns no commands
    /// </summary>
    public class NullCommandSource : ICommandSource
    {
        public CommandOrString GetNextCommand() => null;
    }
}