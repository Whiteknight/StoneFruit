using Autofac;
using FluentAssertions;
using NUnit.Framework;
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
            containerBuilder.SetupEngine(b => b
                .SetupEnvironments(e => e.UseInstance(new TestEnvironment("Single")))
                .SetupHandlers(h => h.Scan())
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            var container = containerBuilder.Build();
            var engine = container.Resolve<Engine>();
            engine.RunHeadless("test-environment");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("Single");
        }
    }
}
