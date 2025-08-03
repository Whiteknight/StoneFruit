namespace StoneFruit.SpecTests.Support;
public class ScannedHandler : IHandler
{
    private readonly IOutput _output;

    public ScannedHandler(IOutput output)
    {
        _output = output;
    }

    public void Execute() => _output.WriteLine("Scanned");
}
