using FluentAssertions;
using Ninject;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Containers.Ninject.Tests.Integration
{
    public class EnvironmentTests
    {
        [Test]
        public void Instance_Test()
        {
            var kernel = new StandardKernel();
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseNinjectHandlerSource(kernel))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseInstance(new TestEnvironment("Single")))
                .Build();
            engine.RunHeadless("test-environment");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("Single");
        }
    }
}
