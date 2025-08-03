using StoneFruit.Execution.Help;

namespace StoneFruit.Execution;

[Verb(Name)]
[Verb("quit", Hide = true)]
public class ExitHandler : IHandler
{
    public const string Name = "exit";

    public static string Group => HelpHandler.BuiltinsGroup;

    public static string Description => "Exits the application";

    public void Execute(IArguments arguments, HandlerContext context)
    {
        var exitCode = arguments.Shift().As(ExitCode.Parse);
        context.State.SignalExit(exitCode);
    }
}
