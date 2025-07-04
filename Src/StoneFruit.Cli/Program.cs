using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Cli;

internal static class Program
{
    private static void Main(string[] args)
    {
        var services = new ServiceCollection();
        services.UseStonefruit(b => b
            .SetupHandlers(h => h
                // Scan for handlers using reflection. Looks for any public, non-abstract,
                // implementations of IHandlerBase in the specified assemblies
                .ScanHandlersFromEntryAssembly()
                .ScanHandlersFromCurrentAssembly()
                .ScanHandlersFromAssemblyContaining<MyFirstHandler>()

                // Create an object instance and treat each of it's public methods as handlers
                .UsePublicInstanceMethodsAsHandlers(new MyObject())

                // Set up a verb which executes a callback delegate
                // Also set up a script as an alias of this
                // Also setup a multi-word verb using a callback delegate.
                .Add("testf", (_, d) => d.Output.WriteLine("F"), description: "do F things", usage: "testf ...", group: "delegates")
                .AddScript("testf-alias", ["testf"], group: "delegates")
                .Add(new[] { "test", "j" }, (c, d) => d.Output.WriteLine("J"), description: "do J things", usage: "test j")

                // Create a few scripts, showing usage
                .AddScript("testg", ["echo test", "echo g"], group: "scripts")
                .AddScript("testh", ["echo [0]", "echo ['a']"], group: "scripts")
                .AddScript("testi", [
                    "echo 1",
                    "echo 2",
                    "echo 3",
                    "echo 4"
                ], group: "scripts")
            )
            .SetupEnvironments(e => e
                .SetEnvironments(new[] { "Local", "Testing", "Production" })
                .UseFactory(new MyEnvironmentFactory())
            )
            .SetupEvents(e =>
            {
                //e.EngineStartInteractive.Clear();
                //e.EngineStopInteractive.Add("echo 'goodbye'");
                //e.EngineError.Add("echo 'you dun goofed'");
            })
            .SetupSettings(s =>
            {
                //s.MaxInputlessCommands = 3;
                s.MaxExecuteTimeout = TimeSpan.FromSeconds(5);
            })
        );
        var provider = services.BuildServiceProvider();
        var engine = provider.GetRequiredService<Engine>();
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

public class MyEnvironmentFactory : IEnvironmentFactory<MyEnvironment>
{
    public Maybe<MyEnvironment> Create(string name) => new MyEnvironment(name);
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
    [Group("public-methods")]
    [Description("do D things")]
    [Usage("test d <name>")]
    public void TestD(IOutput output, string name)
    {
        output.WriteLine($"{name}: D");
    }

    [Group("public-methods")]
    [Description("do E things")]
    [Usage("test e")]
    public Task TestE(IOutput output, CancellationToken token)
    {
        output.WriteLine("E");
        return Task.CompletedTask;
    }
}
