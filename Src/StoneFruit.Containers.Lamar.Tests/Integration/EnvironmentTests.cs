using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Containers.Lamar.Tests.Helpers;

namespace StoneFruit.Containers.Lamar.Tests.Integration
{
    public class EnvironmentTests
    {
        [Test]
        public void Instance_Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(LamarEnvironmentTestHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseInstance(new LamarTestEnvironment("Single")))
                .Build();
            engine.RunHeadless("test-environment");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("Single");
        }
    }
}
