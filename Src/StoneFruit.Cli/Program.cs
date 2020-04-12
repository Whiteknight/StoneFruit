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

    public class EngineAccessor
    {
        public Engine Engine { get; private set; }

        public void SetEngine(Engine engine)
        {
            Engine = engine;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Container container = null;
            Container GetContainer() => container;

            Action<EngineEventCatalog> setupCatalog = e =>
            {
                //e.EngineStartInteractive.Clear();
                //e.EngineStopInteractive.Add("echo 'goodbye'");
                //e.EngineError.Add("echo 'you dun goofed'");
            };

            Action<EngineSettings> setupSettings = s =>
            {
                //s.MaxInputlessCommands = 3;
                s.MaxExecuteTimeout = TimeSpan.FromSeconds(5);
            };

            var engineAccessor = new EngineAccessor();
            var serviceCollection = new ServiceRegistry();
            serviceCollection.AddSingleton(engineAccessor);
            serviceCollection.Scan(scanner =>
            {
                scanner.ScanForHandlers();
            });
            serviceCollection.SetupInjectableServices<MyEnvironment>();
            serviceCollection.AddSingleton<IHandlerSource>(provider => new LamarHandlerSource<MyEnvironment>(provider, TypeVerbExtractor.DefaultInstance));
            serviceCollection.AddSingleton<IHandlerSource>(HandlerSource.GetBuiltinHandlerSource());
            serviceCollection.AddSingleton<IOutput>(new ConsoleOutput());
            serviceCollection.AddSingleton(provider =>
            {
                var catalog = new EngineEventCatalog();
                setupCatalog?.Invoke(catalog);
                return catalog;
            });
            serviceCollection.AddSingleton(provider =>
            {
                var settings = new EngineSettings();
                setupSettings?.Invoke(settings);
                return settings;
            });
            serviceCollection.AddSingleton<AliasMap>();
            serviceCollection.AddSingleton<IHandlers, HandlerSourceCollection>();
            serviceCollection.AddSingleton<IEnvironmentCollection>(new FactoryEnvironmentCollection(new MyEnvironmentFactory()));
            serviceCollection.AddSingleton<ICommandParser>(CommandParser.GetDefault());
            serviceCollection.AddTransient(provider => provider.GetService<EngineAccessor>().Engine.GetCurrentState());
            serviceCollection.AddTransient(provider => provider.GetService<EngineAccessor>().Engine.GetCurrentDispatcher());
            serviceCollection.AddSingleton(provider =>
            {
                var accessor = provider.GetService<EngineAccessor>();
                var handlers = provider.GetService<HandlerSourceCollection>();
                var environments = provider.GetService<IEnvironmentCollection>();
                var parser = provider.GetService<ICommandParser>();
                var output = provider.GetService<IOutput>();
                var engineCatalog = provider.GetService<EngineEventCatalog>();
                var engineSettings = provider.GetService<EngineSettings>();
                var e = new Engine(handlers, environments, parser, output, engineCatalog, engineSettings);
                accessor.SetEngine(e);
                return e;
            });
            container = new Container(serviceCollection);

            var engine = container.GetService<Engine>();
            Environment.ExitCode = engine.RunInteractively();
            Console.ReadKey();

            //var engine = new EngineBuilder()
            //.SetupHandlers(h => h
            //.UseLamarHandlerSource<object>()
            //.UseNinjectHandlerSource()
            //.UseStructureMapHandlerSource()
            //.UseCommands(typeof(HelpCommand), typeof(ExitCommand))
            //.UsePublicMethodsAsHandlers(new MyObject())
            //.Add("testf", (c, d) => d.Output.WriteLine("F"))
            //.AddScript("testg", new [] { "echo test", "echo g" })
            //.AddScript("testh", new[] { "echo [0]", "echo ['a']" })
            //.AddScript("testi", new [] {
            //    "echo 1",
            //    "echo 2",
            //    "echo 3",
            //    "echo 4"
            //})
            //)
            //.SetupEnvironments(e => e.UseFactory(new MyEnvironmentFactory()))
            //.SetupEvents()
            //.SetupSettings(s => {
            //    //s.MaxInputlessCommands = 3;
            //    s.MaxExecuteTimeout = TimeSpan.FromSeconds(5);
            //})
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
        private readonly TestArgsA _args;

        public TestAHandler(ICommandParser parser)
        {
            _args = parser.ParseArguments("x y z").MapTo<TestArgsA>();
        }

        public void Execute()
        {
            Console.WriteLine("TESTA");
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
