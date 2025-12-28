using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace StoneFruit.Cli;

internal static class Program
{
    private static void Main(string[] args)
    {
        var builder = StoneFruitApplicationBuilder.Create();
        builder.Services.AddPerEnvironment<MyEnvironment>((p, env) => new MyEnvironment(env));
        builder.Services.AddHandlerArgumentType<TestArgsK>();
        builder.SetupHandlers(h => h
            // Scan for handlers using reflection. Looks for any public, non-abstract,
            // implementations of IHandlerBase in the specified assemblies
            //.ScanHandlersFromEntryAssembly()
            .ScanHandlersFromCurrentAssembly("entry")
            //.ScanHandlersFromAssemblyContaining<MyFirstHandler>()

            // Create an object instance and treat each of it's public methods as handlers
            // Do it two ways, so we can see the effect of a Section
            .AddSection("grouped", s => s
                .UsePublicInstanceMethodsAsHandlers(new MyObject("grouped"))
            )
            .UsePublicInstanceMethodsAsHandlers(new MyObject("ungrouped"))

            // Set up a verb which executes a callback delegate
            // Also set up a script as an alias of this
            // Also setup a multi-word verb using a callback delegate.
            .Add("testf", (a, hc) => hc.Output.WriteLine("F"), description: "do F things", usage: "testf ...", group: "delegates")
            .AddScript("testf-alias", ["testf"], group: "delegates")
            .Add(new[] { "test", "j" }, (args, hc) => hc.Output.WriteLine("J"), description: "do J things", usage: "test j")

            // Create a few scripts, showing usage
            .AddSection("scripts", s => s
                .AddScript("testg", ["echo test", "echo g"])
                .AddScript("testh", ["echo [0]", "echo ['a']"])
                .AddScript("testi", [
                    "echo 1",
                    "echo 2",
                    "echo 3",
                    "echo 4"
                ])
            )
        );
        builder.SetupEnvironments(e => e
            .SetEnvironments(["Local", "Testing", "Production"])
            .OnEnvironmentChanged(name => Console.WriteLine($"OBSERVER Environment changed to {name}"))
        );
        builder.SetupEvents(e =>
        {
            //e.EngineStartInteractive.Clear();
            //e.EngineStopInteractive.Add("echo 'goodbye'");
            //e.EngineError.Add("echo 'you dun goofed'");
            //e.EngineError.Add("b");
        });
        builder.SetupSettings(s =>
        {
            //s.MaxInputlessCommands = 3;
            s.MaxExecuteTimeout = TimeSpan.FromSeconds(5);
        });
        var engine = builder.Build();
        engine.RunWithCommandLineArguments();
    }
}

public class MyFirstHandler : IHandler
{
    public void Execute(IArguments arguments, HandlerContext context)
    {
        context.State.Metadata.Add("test", this);
        context.Output.WriteLine("Starting the job...");
        // .. Do work here ..
        context.Output.WriteLine("Done.");
    }
}

public class TestAHandler : IHandler
{
    private readonly MyEnvironment _environment;

    public TestAHandler(MyEnvironment environment)
    {
        _environment = environment;
    }

    public void Execute(IArguments arguments, HandlerContext context)
    {
        _environment.InvokedTimes++;
        context.Output.WriteLine($"TESTA: {_environment.Name} invoked {_environment.InvokedTimes} times");
    }
}

[Verb("b")]
public class TestBHandler : IHandler
{
    public static string Description => "Throw an exception";

    public void Execute(IArguments arguments, HandlerContext context)
    {
        throw new Exception("TESTB");
    }
}

public class TestCHandler : IAsyncHandler
{
    public async Task ExecuteAsync(IArguments arguments, HandlerContext context, CancellationToken cancellation)
    {
        await Task.Run(() =>
        {
            while (!cancellation.IsCancellationRequested)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }
        });
        Console.WriteLine("Cancelled!");
        return;
    }
}

public class FormatHandler : IHandler
{
    public void Execute(IArguments arguments, HandlerContext context)
    {
        context.Output.WriteLineFormatted("{{color red}}test{{color darkyellow}}test{{color yellow}}test{{color green}}test{{color cyan}}test{{color blue}}test{{color magenta}}test{{color default}}test");
        context.Output.WriteLineFormatted("{{color white,red}}test{{color white,darkyellow}}test{{color white,yellow}}test{{color white,green}}test{{color white,cyan}}test{{color white,blue}}test{{color white,magenta}}test{{color}}test");
        var fmt = arguments.Consume(0).AsString();
        if (!string.IsNullOrEmpty(fmt))
        {
            context.Output.WriteLineFormatted(fmt, new
            {
                Value1 = "Value1",
                Value2 = new
                {
                    Value3 = "Value3"
                }
            });
        }
    }
}

public class MyEnvironment
{
    public string Name { get; }

    public int InvokedTimes { get; set; }

    public MyEnvironment(string name)
    {
        Name = name;
        InvokedTimes = 0;
    }
}

public class MyObject
{
    private readonly string _tag;

    public MyObject(string tag)
    {
        _tag = tag;
    }

    [Group("public-methods")]
    [Description("do D things")]
    [Usage("test d <name>")]
    public void TestD(IOutput output, string name)
    {
        output.WriteLine($"{_tag} {name}: D");
    }

    [Group("public-methods")]
    [Description("do E things")]
    [Usage("test e")]
    public Task TestE(IOutput output, CancellationToken token)
    {
        output.WriteLine($"{_tag} E");
        return Task.CompletedTask;
    }
}

public class TestArgsK
{
    [ArgumentIndex(0)]
    [Required]
    public string Arg1 { get; set; }

    [ArgumentIndex(1)]
    public string Arg2 { get; set; }

    [ArgumentIndex(2)]
    public string Arg3 { get; set; }
}

public class TestKHandler : IHandler
{
    private readonly TestArgsK _args;

    public TestKHandler(TestArgsK args)
    {
        _args = args;
    }

    public void Execute(IArguments arguments, HandlerContext context)
    {
        context.Output.WriteLine($"Arg1: {_args.Arg1}");
        context.Output.WriteLine($"Arg2: {_args.Arg2}");
        context.Output.WriteLine($"Arg3: {_args.Arg3}");
    }
}
