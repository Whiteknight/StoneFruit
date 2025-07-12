namespace StoneFruit.SpecTests.Support;

public class ThrowExceptionHandler : IHandler
{
    public void Execute()
    {
        throw new Exception("Exception thrown");
    }
}
