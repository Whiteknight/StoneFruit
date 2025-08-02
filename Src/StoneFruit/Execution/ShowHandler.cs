using System.Linq;
using System.Reflection;
using StoneFruit;
using StoneFruit.Execution.Help;

namespace StoneFruit.Execution;

[Verb(Name, Hide = true)]
public class ShowHandler : IHandler
{
    public const string Name = "_show";
    private readonly IArguments _args;
    private readonly IOutput _output;
    private readonly EngineSettings _settings;

    public ShowHandler(IArguments args, IOutput output, EngineSettings settings)
    {
        _args = args;
        _output = output;
        _settings = settings;
    }

    public static string Description => "Show internal values and settings";

    public static string Group => HelpHandler.BuiltinsGroup;

    public void Execute()
    {
        var command = _args.Get(0).Require("Must provide thing to show").AsString();
        switch (command)
        {
            // TODO: Would like to include some kind of library version output here too.
            case "exitcodes":
                ShowExitCodes();
                break;
            case "settings":
                ShowSettings();
                break;
        }
    }

    private void ShowExitCodes()
    {
        var constants = typeof(ExitCode.Constants)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .ToList();
        foreach (var c in constants)
            _output.WriteLine($"{c.Name} : {c.GetValue(null)}");
    }

    private void ShowSettings()
    {
        var props = typeof(EngineSettings)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToList();
        foreach (var p in props)
            _output.WriteLine($"{p.Name} : {p.GetValue(_settings)}");
    }
}
