using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution;
using TestUtilities;
using Unity;

namespace StoneFruit.Containers.Unity.Tests.Integration
{
    public class EnvironmentTests
    {
        [Test]
        public void Instance_Test()
        {
            var output = new TestOutput();
            var container = new UnityContainer();
            container.SetupEngine<TestEnvironment>(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseInstance(new TestEnvironment("Single")))
            );
            var engine = container.Resolve<Engine>();
            engine.RunHeadless("test-environment");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("Single");
        }
    }
}
