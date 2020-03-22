using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Handlers;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class InteractiveTests
    {
        [Test]
        public void StartAndStopEvents_Test()
        {
            var output = new TestOutput("echo 'test'");
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(EchoHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupEvents(c =>
                {
                    // We have to clear the script here otherwise it will try to set the environment
                    c.EngineStartInteractive.Clear();
                    c.EngineStartInteractive.Add("echo start");
                    c.EngineStopInteractive.Add("echo stop");
                })
                .Build();
            engine.RunInteractively();
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("start");
            output.Lines[1].Should().Be("test");
            output.Lines[2].Should().Be("stop");
        }
    }
}