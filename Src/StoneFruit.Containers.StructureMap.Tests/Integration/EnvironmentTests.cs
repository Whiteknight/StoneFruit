using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution;
using StructureMap;
using TestUtilities;

namespace StoneFruit.Containers.StructureMap.Tests.Integration
{
    public class EnvironmentTests
    {
        [Test]
        public void Instance_Test()
        {
            var output = new TestOutput();
            var container = new Container();
            container.SetupEngine<TestEnvironment>(builder => builder
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseInstance(new TestEnvironment("Single")))
            );
            var engine = container.GetInstance<Engine>();
            engine.RunHeadless("test-environment");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("Single");
        }
    }
}
