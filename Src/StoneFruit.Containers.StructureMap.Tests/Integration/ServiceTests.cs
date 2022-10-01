using FluentAssertions;
using NUnit.Framework;
using StructureMap;
using TestUtilities;

namespace StoneFruit.Containers.StructureMap.Tests.Integration
{
    public class ServiceTests
    {
        public class MyEnvironment
        {
            public string Name { get; }

            public MyEnvironment(string name)
            {
                Name = name;
            }
        }

        public class MyEnvironmentFactory : IEnvironmentFactory<MyEnvironment>
        {
            public IResult<MyEnvironment> Create(string name) => Result.Success(new MyEnvironment(name));
        }

        public class MyService
        {
            private readonly MyEnvironment _env;

            public MyService(MyEnvironment env)
            {
                _env = env;
            }

            public string GetEnvironmentName() => _env.Name;
        }

        [Verb("test-service")]
        public class TestHandler : IHandler
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
            var output = new TestOutput();
            var container = new Container();
            container.SetupEngine(builder => builder
                .SetupHandlers(h => h.Scan())
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e
                    .SetEnvironments(new[] { "A", "B", "C" })
                    .UseFactory(new MyEnvironmentFactory())
                )
                .SetupEvents(e =>
                {
                    e.EnvironmentChanged.Clear();
                })
            );

            var engine = container.GetInstance<Engine>();

            engine.RunHeadless("A test-service");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("A");
        }
    }
}
