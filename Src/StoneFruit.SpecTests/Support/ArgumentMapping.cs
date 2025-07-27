namespace StoneFruit.SpecTests.Support;

public class SimpleMappedObject
{
    [ArgumentIndex(0)]
    [ArgumentNamed("first")]
    public string? Arg1 { get; set; }

    [ArgumentIndex(1)]
    [ArgumentNamed("second")]
    public int Arg2 { get; set; }

    [ArgumentIndex(2)]
    [ArgumentNamed("third")]
    public string? Arg3 { get; set; }

    public bool Flag1 { get; set; }
}

public class SimpleMapToObjectHandler : IHandler
{
    private readonly IOutput _output;
    private readonly SimpleMappedObject _args;

    public SimpleMapToObjectHandler(IArguments args, IOutput output)
    {
        _output = output;
        _args = args.MapTo<SimpleMappedObject>();
    }

    public void Execute()
    {
        _output.WriteLine(_args.Arg1 ?? string.Empty);
        _output.WriteLine(_args.Arg2);
        _output.WriteLine(_args.Arg3 ?? string.Empty);
        _output.WriteLine(_args.Flag1);
    }
}

public class ComplexMappedObject
{
    [ArgumentIndex(0)]
    [ArgumentNamed("first")]
    public FileInfo? AsFileInfo { get; set; }

    [ArgumentIndex(1)]
    [ArgumentNamed("second")]
    public DirectoryInfo? AsDirectoryInfo { get; set; }

    [ArgumentIndex(2)]
    [ArgumentNamed("third")]
    public Guid AsGuid { get; set; }
}

public class ComplexMapToObjectHandler : IHandler
{
    private readonly IOutput _output;
    private readonly ComplexMappedObject _args;

    public ComplexMapToObjectHandler(IArguments args, IOutput output)
    {
        _output = output;
        _args = args.MapTo<ComplexMappedObject>();
    }

    public void Execute()
    {
        _output.Write("As FileInfo: ").WriteLine(_args.AsFileInfo != null);
        _output.Write("As DirectoryInfo: ").WriteLine(_args.AsDirectoryInfo != null);
        _output.Write("As Guid: ").WriteLine(_args.AsGuid != Guid.Empty);
    }
}