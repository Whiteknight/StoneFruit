using System.Collections.Generic;
using FluentAssertions;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using StoneFruit.Execution;
using TestUtilities;

namespace StoneFruit.Containers.Lamar.Tests.Integration
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

        private class MyEnvironmentFactory : IEnvironmentFactory
        {
            public IResult<object> Create(string name) => Result.Success(new MyEnvironment(name));

            public IReadOnlyCollection<string> ValidEnvironments => new[] { "A", "B", "C" };
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
        public class LamarServiceTestHandler : IHandler
        {
            private readonly MyService _service;
            private readonly IOutput _output;

            public LamarServiceTestHandler(MyService service, IOutput output)
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
            var services = new ServiceRegistry();
            services.SetupEngine<MyEnvironment>(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseFactory(new MyEnvironmentFactory()))
                .SetupEvents(e =>
                {
                    e.EnvironmentChanged.Clear();
                })
            );
            services.AddTransient<MyService>();
            var container = new Container(services);
            var engine = container.GetService<Engine>();

            engine.RunHeadless("A test-service");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("A");
        }
    }
}
