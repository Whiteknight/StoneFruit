using StoneFruit.Execution.Help;

namespace StoneFruit.Execution;

[Verb(Name)]
[Verb("quit", Hide = true)]
public class ExitHandler : IHandler
{
    public const string Name = "exit";

    private readonly EngineState _state;
    private readonly IArguments _args;

    public ExitHandler(EngineState state, IArguments args)
    {
        _state = state;
        _args = args;
    }

    public static string Group => HelpHandler.BuiltinsGroup;

    public static string Description => "Exits the application";

    public void Execute()
    {
        var exitCode = _args.Shift().As(ExitCode.Parse);
        _state.SignalExit(exitCode);
    }
}
