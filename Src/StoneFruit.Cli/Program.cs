using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.StructureMap;

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
                .UseStructureMapContainerSource()
                //.UseCommands(typeof(HelpCommand), typeof(ExitCommand))
                .UseEnvironmentFactory(new MyEnvironmentFactory())
                .SetupEvents(e =>
                {
                    e.EngineStartInteractive.Add("help");
                    e.EngineStopInteractive.Add("echo 'goodbye'");
                    e.EngineError.Add("echo 'you dun goofed'");
                })
                .Build();
            Environment.ExitCode = engine.RunWithCommandLineArguments();
            //Console.ReadKey();
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

        public IReadOnlyCollection<string> ValidEnvironments => new[] { "TEST1", "TEST2", "TEST3" };
    }

    public class MyEnvironment
    {
        public string Name { get; }

        public MyEnvironment(string name)
        {
            Name = name;
        }
    }
}
