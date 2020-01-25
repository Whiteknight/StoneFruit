namespace StoneFruit.Execution
{
    public interface ICommandSource
    {
        void Start();

        string GetNextCommand();
    }
}