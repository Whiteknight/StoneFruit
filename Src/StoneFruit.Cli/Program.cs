using System;
using System.Collections.Generic;
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
                })
                .Build();
            Environment.ExitCode = engine.Run(args);
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

    public class TestACommand : ICommandVerb
    {
        private readonly TestArgsA _args;

        public TestACommand(CommandParser parser)
        {
            _args = parser.ParseArguments("x y z").MapTo<TestArgsA>();
        }

        public void Execute()
        {
            Console.WriteLine("TESTA");
        }
    }

    public class TestBCommand : ICommandVerb
    {
        public void Execute()
        {
            Console.WriteLine("TESTB");
        }
    }

    public class TestCCommand : ICommandVerb
    {
        public void Execute()
        {
            Console.WriteLine("TESTC");
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
