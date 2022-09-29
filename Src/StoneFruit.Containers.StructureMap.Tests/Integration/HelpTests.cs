using FluentAssertions;
using NUnit.Framework;
using StructureMap;
using TestUtilities;

namespace StoneFruit.Containers.StructureMap.Tests.Integration
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
            var container = new Container();
            container.SetupEngine<object>(builder => builder
                .SetupHandlers(h => h.Scan())
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );

            var engine = container.GetInstance<Engine>();

            engine.RunHeadless("help");
            output.Lines.Should().Contain("test-help");
        }

        [Test]
        public void Help_Usage_Test()
        {
            var output = new TestOutput();
            var container = new Container();
            container.SetupEngine<object>(builder => builder
                .SetupHandlers(h => h.Scan())
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );

            var engine = container.GetInstance<Engine>();

            engine.RunHeadless("help test-help");
            output.Lines.Should().Contain("test the help handler");
        }
    }
}
