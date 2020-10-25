using Autofac;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution;
using TestUtilities;

namespace StoneFruit.Containers.Autofac.Tests.Integration
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
            var containerBuilder = new ContainerBuilder();
            containerBuilder.SetupEngine<object>(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            var container = containerBuilder.Build();
            var engine = container.Resolve<Engine>();

            engine.RunHeadless("help");
            output.Lines.Should().Contain("test-help");
        }

        [Test]
        public void Help_Usage_Test()
        {
            var output = new TestOutput();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.SetupEngine<object>(b => b
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            var container = containerBuilder.Build();
            var engine = container.Resolve<Engine>();

            engine.RunHeadless("help test-help");
            output.Lines.Should().Contain("test the help handler");
        }
    }
}
