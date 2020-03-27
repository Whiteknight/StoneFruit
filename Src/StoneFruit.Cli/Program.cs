using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Containers.Lamar;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            //var x = SimplifiedArgumentGrammar.GetParser().ParseArguments(args).MapTo<TestArgsA>();
            //var y = WindowsCmdArgumentGrammar.GetParser().ParseArguments(args).MapTo<TestArgsA>();
            //var z = PosixStyleArgumentGrammar.GetParser().ParseArguments(args).MapTo<TestArgsA>();
            //var w = PowershellStyleArgumentGrammar.GetParser().ParseArguments(args).MapTo<TestArgsA>();

            var engine = new EngineBuilder()
                .SetupHandlers(h => h
                    .UseLamarHandlerSource<object>()
                    //.UseNinjectHandlerSource()
                    //.UseStructureMapHandlerSource()
                    //.UseCommands(typeof(HelpCommand), typeof(ExitCommand))
                    //.UsePublicMethodsAsHandlers(new MyObject())
                    .Add("testf", (c, d) => d.Output.WriteLine("F"))
                    .AddScript("testg", new [] { "echo test", "echo g" })
                    .AddScript("testh", new[] { "echo [0]", "echo ['a']" })
                )
                .SetupEnvironments(e => e.UseFactory(new MyEnvironmentFactory()))
                .SetupEvents(e =>
                {
                    //e.EngineStartInteractive.Add("help");
                    //e.EngineStopInteractive.Add("echo 'goodbye'");
                    //e.EngineError.Add("echo 'you dun goofed'");
                })
                .Build();
            Environment.ExitCode = engine.RunWithCommandLineArguments();
            Console.ReadKey();
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

        public TestAHandler(CommandParser parser)
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
        public Task ExecuteAsync(CancellationToken cancellation)
        {
            Console.WriteLine("TESTC");
            return Task.CompletedTask;
        }
    }

    public class MyEnvironmentFactory : IEnvironmentFactory
    {
        public object Create(string name)
        {
            return new MyEnvironment(name);
        }

        public IReadOnlyCollection<string> ValidEnvironments => new[] { "TEST1" };
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
