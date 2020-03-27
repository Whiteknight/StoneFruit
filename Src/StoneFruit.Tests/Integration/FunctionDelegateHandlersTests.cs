using FluentAssertions;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class FunctionDelegateHandlersTests
    {
        [Test]
        public void Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.Add("test", (c, d) => d.Output.WriteLine("TEST")))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test");
            output.Lines[0].Should().Be("TEST");
        }
    }
}