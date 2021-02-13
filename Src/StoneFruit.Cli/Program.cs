using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using StoneFruit.Containers.Autofac;
using StoneFruit.Containers.Lamar;
using StoneFruit.Containers.Microsoft;
using StoneFruit.Containers.StructureMap;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Cli
{
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

    internal static class Program
    {
        private static void Main(string[] args)
        {
            //var engine = NoneMain();
            //var engine = StructureMapMain();
            //var engine = LamarMain();
            //var engine = MicrosoftMain();
            var engine = AutofacMain();
            Environment.ExitCode = engine.RunWithCommandLineArguments();

            Console.ReadKey();
        }

        private static void Build(IEngineBuilder builder)
        {
            builder
                .SetupHandlers(h => h
                    .UsePublicMethodsAsHandlers(new MyObject(), getGroup: s => "public-methods")
                    .Add("testf", (c, d) => d.Output.WriteLine("F"), description: "do F things", usage: "testf ...", group: "delegates")
                    .AddScript("testg", new[] { "echo test", "echo g" }, group: "scripts")
                    .AddScript("testh", new[] { "echo [0]", "echo ['a']" }, group: "scripts")
                    .AddScript("testi", new[]
                    {
                        "echo 1",
                        "echo 2",
                        "echo 3",
                        "echo 4"
                    }, group: "scripts")
                //.AddAlias("testf", "testf-alias")
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
                });
        }

        private static Engine MicrosoftMain()
        {
            IServiceProvider provider = null;
            var services = new ServiceCollection();
            services.SetupEngine<MyEnvironment>(Build, () => provider);
            provider = services.BuildServiceProvider();
            return provider.GetService<Engine>();
        }

        private static Engine NoneMain()
        {
            var builder = new EngineBuilder();
            Build(builder);
            return builder.Build();
        }

        private static Engine StructureMapMain()
        {
            var container = new StructureMap.Container();
            container.SetupEngine<MyEnvironment>(Build);

            return container.GetInstance<Engine>();
        }

        private static Engine LamarMain()
        {
            var serviceCollection = new ServiceRegistry()
                .SetupEngine<MyEnvironment>(Build);

            var container = new Container(serviceCollection);
            return container.GetService<Engine>();
        }

        private static Engine AutofacMain()
        {
            var containerBuilder = new Autofac.ContainerBuilder();
            containerBuilder.SetupEngine<MyEnvironment>(Build);
            Autofac.IContainer container = containerBuilder.Build();
            return container.Resolve<Engine>();
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
        public IResult<object> Create(string name)
            => Result.Success(new MyEnvironment(name));

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

        public Task TestE(IOutput output, CancellationToken token)
        {
            output.WriteLine("E");
            return Task.CompletedTask;
        }
    }
}
