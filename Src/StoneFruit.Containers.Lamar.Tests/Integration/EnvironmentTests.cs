using FluentAssertions;
using Lamar;
using NUnit.Framework;
using StoneFruit.Execution;
using TestUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace StoneFruit.Containers.Lamar.Tests.Integration
{
    public class EnvironmentTests
    {
        [Test]
        public void Instance_Test()
        {
            var output = new TestOutput();
            var services = new ServiceRegistry();
            services.SetupEngine<TestEnvironment>(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseInstance(new TestEnvironment("Single")))
            );
            var container = new Container(services);
            var engine = container.GetService<Engine>();
            engine.RunHeadless("test-environment");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("Single");
        }
    }
}
