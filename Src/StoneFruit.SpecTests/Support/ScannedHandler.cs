namespace StoneFruit.SpecTests.Support;
public class ScannedHandler : IHandler
{
    public void Execute(IArguments arguments, HandlerContext context)
        => context.Output.WriteLine("Scanned");
}
