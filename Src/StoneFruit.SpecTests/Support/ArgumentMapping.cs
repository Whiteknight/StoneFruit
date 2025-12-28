using System.ComponentModel.DataAnnotations;

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
    public void Execute(IArguments arguments, HandlerContext context)
    {
        var args = arguments.MapTo<SimpleMappedObject>();
        context.Output.WriteLine(args.Arg1 ?? string.Empty);
        context.Output.WriteLine(args.Arg2);
        context.Output.WriteLine(args.Arg3 ?? string.Empty);
        context.Output.WriteLine(args.Flag1);
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
    public void Execute(IArguments arguments, HandlerContext context)
    {
        var args = arguments.MapTo<ComplexMappedObject>();
        context.Output.Write("As FileInfo: ").WriteLine(args.AsFileInfo != null);
        context.Output.Write("As DirectoryInfo: ").WriteLine(args.AsDirectoryInfo != null);
        context.Output.Write("As Guid: ").WriteLine(args.AsGuid != Guid.Empty);
    }
}

public class TypeMappedArgs
{
    [ArgumentIndex(0)]
    [ArgumentNamed("first")]
    [Required]
    public string? Arg1 { get; set; }

    [ArgumentIndex(1)]
    [ArgumentNamed("second")]
    public int Arg2 { get; set; }

    [ArgumentIndex(2)]
    [ArgumentNamed("third")]
    public string? Arg3 { get; set; }

    public bool Flag1 { get; set; }
}

public class TypeMappedObjectHandler : IHandler
{
    private readonly TypeMappedArgs _args;

    public TypeMappedObjectHandler(TypeMappedArgs args)
    {
        _args = args;
    }

    public void Execute(IArguments arguments, HandlerContext context)
    {
        context.Output.WriteLine(_args.Arg1 ?? string.Empty);
        context.Output.WriteLine(_args.Arg2);
        context.Output.WriteLine(_args.Arg3 ?? string.Empty);
        context.Output.WriteLine(_args.Flag1);
    }
}
