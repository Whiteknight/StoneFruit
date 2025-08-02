using StoneFruit.Execution.Help;

namespace StoneFruit.Execution.Arguments;

[Verb(Name, Hide = true)]
public class ArgumentDisplayHandler : IHandler
{
    public const string Name = "_args";
    private readonly IArguments _args;
    private readonly IOutput _output;

    public ArgumentDisplayHandler(IArguments args, IOutput output)
    {
        _args = args;
        _output = output;
    }

    public static string Group => HelpHandler.BuiltinsGroup;
    public static string Description => "Diagnostic handler to display arguments passed";
    public static string Usage => $$"""
        {{Name}} ...

            Displays all arguments passed, one per line
            Used for unit-testing and diagnosing issues with argument-parsing and scripts
        """;

    public void Execute()
    {
        int index = 0;
        foreach (var p in _args.GetAllPositionals())
        {
            _output.WriteLine($"{index}: {p.Value}");
            index++;
        }

        foreach (var n in _args.GetAllNamed())
            _output.WriteLine($"'{n.Name}': {n.Value}");

        foreach (var f in _args.GetAllFlags())
            _output.WriteLine($"flag: {f.Name}");
    }
}
