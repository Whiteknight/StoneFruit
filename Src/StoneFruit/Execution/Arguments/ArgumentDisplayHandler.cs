using StoneFruit.Execution.Help;

namespace StoneFruit.Execution.Arguments;

[Verb(Name, Hide = true)]
public class ArgumentDisplayHandler : IHandler
{
    public const string Name = "_args";

    public static string Group => HelpHandler.BuiltinsGroup;
    public static string Description => "Diagnostic handler to display arguments passed";
    public static string Usage => $$"""
        {{Name}} ...

            Displays all arguments passed, one per line
            Used for unit-testing and diagnosing issues with argument-parsing and scripts
        """;

    public void Execute(IArguments arguments, HandlerContext context)
    {
        var output = context.Output;
        int index = 0;
        foreach (var p in arguments.GetAllPositionals())
        {
            output.WriteLine($"{index}: {p.Value}");
            index++;
        }

        foreach (var n in arguments.GetAllNamed())
            output.WriteLine($"'{n.Name}': {n.Value}");

        foreach (var f in arguments.GetAllFlags())
            output.WriteLine($"flag: {f.Name}");
    }
}
