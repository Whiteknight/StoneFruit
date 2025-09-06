using System.Linq;
using System.Reflection;
using StoneFruit;
using StoneFruit.Execution.Help;

namespace StoneFruit.Execution;

[Verb(Name, Hide = true)]
public class ShowHandler : IHandler
{
    public const string Name = "_show";
    private readonly EngineSettings _settings;

    public ShowHandler(EngineSettings settings)
    {
        _settings = settings;
    }

    public static string Description => "Show internal values and settings";

    public static string Group => HelpHandler.BuiltinsGroup;

    public void Execute(IArguments arguments, HandlerContext context)
    {
        var command = arguments.Get(0).Require("Must provide thing to show").AsString();
        switch (command)
        {
            case "exitcodes":
                ShowExitCodes(context.Output);
                break;
            case "settings":
                ShowSettings(context.Output, _settings);
                break;
            case "version":
                ShowStonefruitVersion(context.Output);
                break;
        }
    }

    private static void ShowStonefruitVersion(IOutput output)
    {
        output.WriteLine($"StoneFruit Version={typeof(Engine).Assembly.GetName().Version}");
    }

    private static void ShowExitCodes(IOutput output)
    {
        var constants = typeof(ExitCode.Constants)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .ToList();
        foreach (var c in constants)
            output.WriteLine($"{c.Name} : {c.GetValue(null)}");
    }

    private static void ShowSettings(IOutput output, EngineSettings settings)
    {
        var props = typeof(EngineSettings)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToList();
        foreach (var p in props)
            output.WriteLine($"{p.Name} : {p.GetValue(settings)}");
    }
}
