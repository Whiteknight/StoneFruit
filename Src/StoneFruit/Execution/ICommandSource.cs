namespace StoneFruit.Execution
{
    /// <summary>
    /// Source for commands to be executed
    /// </summary>
    public interface ICommandSource
    {
        void Start();

        string GetNextCommand();
    }
}