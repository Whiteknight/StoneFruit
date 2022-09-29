using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Containers.Microsoft.Tests.Integration
{
    public class EnvironmentTests
    {
        [Test]
        public void Instance_Test()
        {
            var output = new TestOutput();
            var services = new ServiceCollection();
            IServiceProvider provider = null;
            services.SetupEngine<TestEnvironment>(b => b
                .SetupHandlers(h => h.Scan())
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEnvironments(e => e.UseInstance(new TestEnvironment("Single"))),
                () => provider
            );
            provider = services.BuildServiceProvider();
            var engine = provider.GetService<Engine>();
            engine.RunHeadless("test-environment");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("Single");
        }
    }
}
