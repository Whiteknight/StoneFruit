namespace StoneFruit.Execution.CommandSources
{
    public class NullCommandSource : ICommandSource
    {
        public void Start()
        {
        }

        public string GetNextCommand()
        {
            return null;
        }
    }
}