namespace StoneFruit.Execution.IO;

public sealed class NullObjectOutputWriter : IObjectOutputWriter
{
    public void WriteObject<T>(T obj)
    {
        // Do nothing
    }
}
