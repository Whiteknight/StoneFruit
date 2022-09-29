using FluentAssertions;
using NUnit.Framework;
using TestUtilities;
using Unity;

namespace StoneFruit.Containers.Unity.Tests.Integration
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
            var container = new UnityContainer();
            container.SetupEngine(b => b
                .SetupHandlers(h => h.Scan())
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            var engine = container.Resolve<Engine>();

            engine.RunHeadless("help");
            output.Lines.Should().Contain("test-help");
        }

        [Test]
        public void Help_Usage_Test()
        {
            var output = new TestOutput();
            var container = new UnityContainer();
            container.SetupEngine(b => b
                .SetupHandlers(h => h.Scan())
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
            );
            var engine = container.Resolve<Engine>();

            engine.RunHeadless("help test-help");
            output.Lines.Should().Contain("test the help handler");
        }
    }
}
