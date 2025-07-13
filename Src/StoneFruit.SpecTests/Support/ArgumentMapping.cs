using StoneFruit.Execution.Arguments;

namespace StoneFruit.SpecTests.Support;

public class ArgumentMappedObject
{
    [ArgumentIndex(0)]
    [ArgumentNamed("first")]
    public string? Arg1 { get; set; }

    [ArgumentIndex(1)]
    [ArgumentNamed("second")]
    public string? Arg2 { get; set; }

    [ArgumentIndex(2)]
    [ArgumentNamed("third")]
    public string? Arg3 { get; set; }

    public bool Flag1 { get; set; }

}

public class ArgumentMappingHandler : IHandler
{
    private readonly IOutput _output;
    private readonly ArgumentMappedObject _args;

    public ArgumentMappingHandler(IArguments args, IOutput output)
    {
        _output = output;
        _args = args.MapTo<ArgumentMappedObject>();
    }

    public void Execute()
    {
        _output.WriteLine(_args.Arg1 ?? string.Empty);
        _output.WriteLine(_args.Arg2 ?? string.Empty);
        _output.WriteLine(_args.Arg3 ?? string.Empty);
        _output.WriteLine(_args.Flag1);
    }
}
