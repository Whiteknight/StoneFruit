namespace StoneFruit.SpecTests.Support;

public class FormatHandler : IHandler
{
    private readonly IOutput _output;
    private readonly IArguments _args;

    public FormatHandler(IOutput output, IArguments args)
    {
        _output = output;
        _args = args;
    }

    public void Execute()
    {
        var value = new
        {
            Prop1 = "prop1",
            Prop2 = new
            {
                Prop2_1 = "prop2_1",
                Prop2_2 = "prop2_2"
            }
        };
        var format = _args.Consume(0).AsString();
        _output.WriteLineFormatted(format, value);
    }
}
