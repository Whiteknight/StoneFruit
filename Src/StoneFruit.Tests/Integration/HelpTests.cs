using FluentAssertions;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class HelpTests
    {
        [Test]
        public void Help_Builtins()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("help");
            output.Lines.Should().Contain("exit");
            output.Lines.Should().Contain("help");
            output.Lines.Should().NotContain("echo");
        }

        [Test]
        public void Help_Builtins_ShowAll()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("help -showall");
            output.Lines.Should().Contain("exit");
            output.Lines.Should().Contain("help");
            output.Lines.Should().Contain("echo");
        }

        [Test]
        public void Help_Alias()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.AddAlias("help", "help-alias"))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("help");
            output.Lines.Should().Contain("help");
            output.Lines.Should().Contain("help-alias");
        }
    }
}
