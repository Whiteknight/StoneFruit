using StoneFruit.Execution.Arguments;

namespace StoneFruit.SpecTests.Support;

public class PositionalMappedArguments
{
    [ArgumentIndex(0)]
    public string? Arg1 { get; set; }

    [ArgumentIndex(1)]
    public string? Arg2 { get; set; }

    [ArgumentIndex(2)]
    public string? Arg3 { get; set; }
}

public class PositionalMappingHandler : IHandler
{
    private readonly IOutput _output;
    private readonly PositionalMappedArguments _args;

    public PositionalMappingHandler(IArguments args, IOutput output)
    {
        _output = output;
        _args = args.MapTo<PositionalMappedArguments>();
    }

    public void Execute()
    {
        _output.WriteLine(_args.Arg1 ?? string.Empty);
        _output.WriteLine(_args.Arg2 ?? string.Empty);
        _output.WriteLine(_args.Arg3 ?? string.Empty);
    }
}
