using FluentAssertions;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
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

        [Test]
        public void Help_Prefix_Test()
        {
            var output = new TestOutput();
            var services = new ServiceRegistry();
            services.SetupEngine(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupHandlers(h => h
                    .AddScript(new[] { "prefix", "a" }, new[] { "echo a" })
                    .AddScript(new[] { "prefix", "b" }, new[] { "echo b" })
                    .AddScript(new[] { "prefix", "c" }, new[] { "echo c" })
                )
            );
            var container = new Container(services);
            var engine = container.GetService<Engine>();

            // There are no handlers "prefix", so VerbNotFound event will call "help -startswith prefix"
            // This should show the three "prefix" commands but not "test-help"
            engine.RunHeadless("prefix");
            output.Lines.Should().Contain("prefix a");
            output.Lines.Should().Contain("prefix b");
            output.Lines.Should().Contain("prefix c");
            output.Lines.Should().NotContain("test-help");
        }
    }
}
