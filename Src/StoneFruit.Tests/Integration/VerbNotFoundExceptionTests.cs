using FluentAssertions;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class VerbNotFoundExceptionTests
    {
        [Test]
        public void Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("does-not-exist");
            output.Lines[0].Should().StartWith("Verb does-not-exist not found.");
        }
    }
}