﻿using Autofac;
using FluentAssertions;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Containers.Autofac.Tests.Integration
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

        private class MyEnvironmentFactory : IEnvironmentFactory<MyEnvironment>
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
        public class AutofacServiceTestHandler : IHandler
        {
            private readonly MyService _service;
            private readonly IOutput _output;

            public AutofacServiceTestHandler(MyService service, IOutput output)
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
            var containerBuilder = new ContainerBuilder();
            containerBuilder.SetupEngine(b => b
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
            containerBuilder.RegisterType<MyService>().InstancePerLifetimeScope().AsSelf();

            var container = containerBuilder.Build();
            var engine = container.Resolve<Engine>();

            engine.RunHeadless("A test-service");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("A");
        }
    }
}
