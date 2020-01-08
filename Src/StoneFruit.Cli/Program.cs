using System;
using System.Collections.Generic;
using StoneFruit.StructureMap;

namespace StoneFruit.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new EngineBuilder()
                .UseStructureMapContainerSource()
                //.UseCommands(typeof(HelpCommand), typeof(ExitCommand))
                .UseEnvironmentFactory(new MyEnvironmentFactory())
                .Build();
            engine.Run(args);
        }
    }

    public class TestACommand : ICommandVerb
    {
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
