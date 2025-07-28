using System;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Execution;
using StoneFruit.Execution.Templating;

namespace StoneFruit.Cli;

internal static class Program
{
    private static void Main(string[] args)
    {
        var builder = StoneFruitApplicationBuilder.Create();
        builder.Services.AddPerEnvironment<MyEnvironment>((p, env) => new MyEnvironment(env));
        builder.SetupHandlers(h => h
            // Scan for handlers using reflection. Looks for any public, non-abstract,
            // implementations of IHandlerBase in the specified assemblies
            .ScanHandlersFromEntryAssembly()
            .ScanHandlersFromCurrentAssembly()
            .ScanHandlersFromAssemblyContaining<MyFirstHandler>()

            // Create an object instance and treat each of it's public methods as handlers
            // Do it two ways, so we can see the effect of a Section
            .AddSection("grouped", s => s
                .UsePublicInstanceMethodsAsHandlers(new MyObject("grouped"))
            )
            .UsePublicInstanceMethodsAsHandlers(new MyObject("ungrouped"))

            // Set up a verb which executes a callback delegate
            // Also set up a script as an alias of this
            // Also setup a multi-word verb using a callback delegate.
            .Add("testf", d => d.Output.WriteLine("F"), description: "do F things", usage: "testf ...", group: "delegates")
            .AddScript("testf-alias", ["testf"], group: "delegates")
            .Add(new[] { "test", "j" }, d => d.Output.WriteLine("J"), description: "do J things", usage: "test j")

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
            .SetEnvironments(new[] { "Local", "Testing", "Production" })
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
    private readonly IOutput _output;
    private readonly EngineState _state;

    public MyFirstHandler(IOutput output, EngineState state)
    {
        _output = output;
        _state = state;
    }

    public void Execute()
    {
        _state.Metadata.Add("test", this);
        _output.WriteLine("Starting the job...");
        // .. Do work here ..
        _output.WriteLine("Done.");
    }
}

public class TestArgsA
{
    [ArgumentIndex(0)]
    public string Arg1 { get; set; }

    [ArgumentIndex(1)]
    public string Arg2 { get; set; }

    [ArgumentIndex(2)]
    public string Arg3 { get; set; }
}

public class TestAHandler : IHandler
{
    private readonly IOutput _output;
    private readonly MyEnvironment _environment;

    public TestAHandler(IOutput output, MyEnvironment environment)
    {
        _output = output;
        _environment = environment;
    }

    public void Execute()
    {
        _environment.InvokedTimes++;
        _output.WriteLine($"TESTA: {_environment.Name} invoked {_environment.InvokedTimes} times");
    }
}

[Verb("b")]
public class TestBHandler : IHandler
{
    public static string Description => "Throw an exception";

    public void Execute()
    {
        throw new Exception("TESTB");
    }
}

public class TestCHandler : IAsyncHandler
{
    public async Task ExecuteAsync(CancellationToken cancellation)
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
    private readonly IOutput _output;
    private readonly IArguments _args;

    public FormatHandler(IOutput output, IArguments args)
    {
        _output = output;
        _args = args;
    }

    public void Execute()
    {
        _output.WriteLineFormatted(_args.Consume(0).AsString(), new
        {
            Value1 = "Value1",
            Value2 = new
            {
                Value3 = "Value3"
            }
        });
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

