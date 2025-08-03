namespace StoneFruit.SpecTests.Support;

[Verb("multi word verb")]
public class MultiWordVerbHandler : IHandler
{
    public void Execute(IArguments arguments, HandlerContext context)
        => context.Output.WriteLine("multi word verb invoked");
}
