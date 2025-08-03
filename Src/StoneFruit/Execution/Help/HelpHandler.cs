using System.Linq;

namespace StoneFruit.Execution.Help;

[Verb(Name)]
public class HelpHandler : IHandler
{
    public const string Name = "help";
    public const string FlagShowAll = "showall";
    public const string FlagStartsWith = "startswith";
    public const string BuiltinsGroup = "__STONEFRUIT_BUILTIN";

    private readonly IHandlers _commands;

    public HelpHandler(IHandlers commands)
    {
        _commands = commands;
    }

    public static string Group => BuiltinsGroup;

    public static string Description => "List all commands or get detailed information for a single command";

    public static string Usage => $$"""
        {{Name}} [ -{{FlagShowAll}} ]
            Get overview information for all available verbs. 
            IHandler classes can implement this static property:

                public static string {{nameof(Description)}} { get; }

            Otherwise IHandler classes or instance methods can use the [DescriptionAttribute(...)]

            The help command by default hides some internal commands which are not necessary for normal
            user interaction. To see all commands including hidden commands use the -{{FlagShowAll}} flag.

        {{Name}} <verb>
            Get detailed help information for the given verb. 
            IHandler classes can implement this static property:

                public static string {{nameof(Usage)}} { get; }

            Otherwise IHandler classes or instance methods can use the [UsageAttribute(...)]

        {{Name}} -{{FlagStartsWith}} <word>
            Get overview information for all handlers whose verbs start with the given word. The verb
            Command class should provide a description using the mechanisms mentioned above.
        """;

    public void Execute(IArguments arguments, HandlerContext context)
    {
        var verb = arguments.GetAllPositionals().Select(p => p.AsString()).ToArray();
        var showAll = arguments.HasFlag(FlagShowAll);
        if (verb.Length == 0)
        {
            new HelpOverviewDisplay(context.Output, _commands).DisplayOverview(showAll);
            return;
        }

        if (arguments.HasFlag(FlagStartsWith))
        {
            new HelpOverviewDisplay(context.Output, _commands).DisplayOverviewStartingWith(verb[0], showAll);
            return;
        }

        Verb.TryCreate(verb)
            .OnSuccess(v => new HelpDetailsDisplay(context.Output, _commands).ShowTestDetail(v));
    }
}
