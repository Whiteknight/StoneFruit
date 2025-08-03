namespace StoneFruit.SpecTests.Support;

public class FormatHandler : IHandler
{
    public void Execute(IArguments arguments, HandlerContext context)
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
        var format = arguments.Consume(0).AsString();
        context.Output.WriteLineFormatted(format, value);
    }
}
