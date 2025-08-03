namespace StoneFruit.SpecTests.Support;

public class ThrowExceptionHandler : IHandler
{
    public void Execute(IArguments arguments, HandlerContext context)
    {
        throw new Exception("Exception thrown");
    }
}
