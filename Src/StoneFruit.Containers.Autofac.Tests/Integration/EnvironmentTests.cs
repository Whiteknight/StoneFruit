using Autofac;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution;
using TestUtilities;

namespace StoneFruit.Containers.Autofac.Tests.Integration
{
    public class EnvironmentTests
    {
        [Test]
        public void Instance_Test()
        {
            var output = new TestOutput();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.SetupEngine<TestEnvironment>(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseInstance(new TestEnvironment("Single")))
            );
            var container = containerBuilder.Build();
            var engine = container.Resolve<Engine>();
            engine.RunHeadless("test-environment");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("Single");
        }
    }
}
