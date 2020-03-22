namespace StoneFruit.Execution.CommandSources
{
    /// <summary>
    /// Null-Object implementation of ICommandSource. Returns no commands
    /// </summary>
    public class NullCommandSource : ICommandSource
    {
        public void Start()
        {
        }

        public CommandObjectOrString GetNextCommand()
        {
            return null;
        }
    }
}