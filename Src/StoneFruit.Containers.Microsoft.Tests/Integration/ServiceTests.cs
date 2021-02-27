using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Containers.Microsoft.Tests.Integration
{
    public class MyEnvironment
    {
        public string Name { get; }

        public MyEnvironment(string name)
        {
            Name = name;
        }
    }

    public class MyEnvironmentFactory : IEnvironmentFactory
    {
        public IResult<object> Create(string name) => Result.Success<object>(new MyEnvironment(name));

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
    public class MicrosoftServiceTestHandler : IHandler
    {
        private readonly MyService _service;
        private readonly IOutput _output;

        public MicrosoftServiceTestHandler(MyService service, IOutput output)
        {
            _service = service;
            _output = output;
        }

        public void Execute()
        {
            _output.WriteLine(_service.GetEnvironmentName());
        }
    }

    public class ServiceTests
    {
        [Test]
        public void Test()
        {
            var output = new TestOutput();
            var services = new ServiceCollection();
            IServiceProvider provider = null;
            services.SetupEngine<MyEnvironment>(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseFactory(new MyEnvironmentFactory()))
                .SetupEvents(e =>
                {
                    e.EnvironmentChanged.Clear();
                }),
                () => provider
            );
            services.AddTransient<MyService>();
            provider = services.BuildServiceProvider();
            var engine = provider.GetService<Engine>();

            engine.RunHeadless("A test-service");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("A");
        }
    }
}
