using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Containers.Lamar;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Environments;
using StoneFruit.Execution.Handlers;
using StoneFruit.Execution.Output;

namespace StoneFruit.Cli
{
    public class MyFirstHandler : IHandler
    {
        private readonly IOutput _output;

        public MyFirstHandler(IOutput output)
        {
            _output = output;
        }

        public void Execute()
        {
            _output.WriteLine("Starting the job...");
            // .. Do work here ..
            _output.WriteLine("Done.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Container container = null;

            var serviceCollection = new ServiceRegistry();
            
            serviceCollection.SetupEngine<MyEnvironment>(builder => builder
                .SetupHandlers(h => h
                    .UseLamarHandlerSource<MyEnvironment>()
                    .UsePublicMethodsAsHandlers(new MyObject())
                    .Add("testf", (c, d) => d.Output.WriteLine("F"))
                    .AddScript("testg", new[] { "echo test", "echo g" })
                    .AddScript("testh", new[] { "echo [0]", "echo ['a']" })
                    .AddScript("testi", new[]
                    {
                        "echo 1",
                        "echo 2",
                        "echo 3",
                        "echo 4"
                    })
                )
                .SetupEnvironments(e => e.UseFactory(new MyEnvironmentFactory()))
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
                }));
            
            
            
            
            container = new Container(serviceCollection);

            var engine = container.GetService<Engine>();
            Environment.ExitCode = engine.RunInteractively();
            Console.ReadKey();

            //var engine = new EngineBuilder()
            
            //.BuildTo();
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
            _output.WriteLine($"TESTA: {_environment.Name}");
        }
    }

    [Verb("b")]
    public class TestBHandler : IHandler
    {
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

    public class MyEnvironmentFactory : IEnvironmentFactory
    {
        public object Create(string name)
        {
            return new MyEnvironment(name);
        }

        public IReadOnlyCollection<string> ValidEnvironments => new[] { "Local", "Testing", "Production" };
    }

    public class MyEnvironment
    {
        public string Name { get; }

        public MyEnvironment(string name)
        {
            Name = name;
        }
    }

    public class MyObject
    {
        public void TestD(IOutput output, string name)
        {
            output.WriteLine($"{name}: D");
        }

        public Task TestE(IOutput output)
        {
            output.WriteLine("E");
            return Task.CompletedTask;
        }
    }
}
