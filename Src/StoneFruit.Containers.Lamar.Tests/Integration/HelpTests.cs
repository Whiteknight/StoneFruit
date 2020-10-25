using FluentAssertions;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using StoneFruit.Execution;
using TestUtilities;

namespace StoneFruit.Containers.Lamar.Tests.Integration
{
    public class HelpTests
    {
        [Verb("test-help")]
        public class TestHandler : IHandler
        {
            public void Execute() => throw new System.NotImplementedException();

            public static string Usage => "test the help handler";
        }

        [Test]
        public void Help_Test()
        {
            var output = new TestOutput();
            var services = new ServiceRegistry();
            services.SetupEngine(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            var container = new Container(services);
            var engine = container.GetService<Engine>();

            engine.RunHeadless("help");
            output.Lines.Should().Contain("test-help");
        }

        [Test]
        public void Help_Usage_Test()
        {
            var output = new TestOutput();
            var services = new ServiceRegistry();
            services.SetupEngine(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            var container = new Container(services);
            var engine = container.GetService<Engine>();

            engine.RunHeadless("help test-help");
            output.Lines.Should().Contain("test the help handler");
        }
    }
}
