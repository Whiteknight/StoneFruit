using System;
using System.Collections.Generic;
using StoneFruit.BuiltInVerbs;
using StoneFruit.StructureMap;

namespace StoneFruit.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new EngineBuilder()
                //.UseStructureMapContainerSource()
                //.UseCommands(typeof(HelpCommand), typeof(ExitCommand))
                //.UseEnvironmentFactory(new MyEnvironmentFactory())
                .Build();
            engine.Start(args);
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
