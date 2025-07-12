namespace StoneFruit.SpecTests.Support;

[Verb("multi word verb")]
public class MultiWordVerbHandler : IHandler
{
    private readonly IOutput _output;

    public MultiWordVerbHandler(IOutput output)
    {
        _output = output;
    }

    public void Execute() => _output.WriteLine("multi word verb invoked");
}
