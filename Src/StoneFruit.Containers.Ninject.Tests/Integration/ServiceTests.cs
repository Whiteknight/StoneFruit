using System.Collections.Generic;
using FluentAssertions;
using Ninject;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Containers.Ninject.Tests.Integration
{
    public class ServiceTests
    {
        private class MyEnvironment
        {
            public string Name { get; }

            public MyEnvironment(string name)
            {
                Name = name;
            }
        }

        private class MyEnvironmentFactory : IEnvironmentFactory
        {
            public object Create(string name) => new MyEnvironment(name);

            public IReadOnlyCollection<string> ValidEnvironments => new[] { "A", "B", "C" };
        }

        private class MyService
        {
            private readonly MyEnvironment _env;

            public MyService(MyEnvironment env)
            {
                _env = env;
            }

            public string GetEnvironmentName() => _env.Name;
        }

        [Verb("test")]
        private class TestHandler : IHandler
        {
            private readonly MyService _service;
            private readonly IOutput _output;

            public TestHandler(MyService service, IOutput output)
            {
                _service = service;
                _output = output;
            }

            public void Execute()
            {
                _output.WriteLine(_service.GetEnvironmentName());
            }
        }

        [Test]
        public void Test()
        {
            var kernel = new StandardKernel();
            kernel.Bind<MyService>().ToSelf().InTransientScope();
            kernel.Bind<IHandlerBase>().To<TestHandler>().InTransientScope();

            var output = new TestOutput();

            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseNinjectHandlerSource(kernel))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseFactory(new MyEnvironmentFactory()))
                .SetupEvents(e =>
                {
                    e.EnvironmentChanged.Clear();
                })
                .Build();

            kernel
                .Bind<MyEnvironment>()
                .ToMethod(c => (MyEnvironment) engine.Environments.Current)
                .InTransientScope();
            
            engine.RunHeadless("A test");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("A");
        }
    }
}