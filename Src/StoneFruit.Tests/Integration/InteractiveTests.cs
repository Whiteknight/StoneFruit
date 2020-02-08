using FluentAssertions;
using NUnit.Framework;
using StoneFruit.BuiltInVerbs;
using StoneFruit.Tests.Helpers;

namespace StoneFruit.Tests.Integration
{
    public class InteractiveTests
    {
        [Test]
        public void StartAndStopEvents_Test()
        {
            var output = new TestOutput("echo 'test'");
            var engine = new EngineBuilder()
                .UseCommandType(typeof(EchoHandler))
                .UseTerminalOutput(output)
                .SetupEvents(c =>
                {
                    // We have to clear the script here otherwise it will try to set the environment
                    c.EngineStartInteractive.Clear();
                    c.EngineStartInteractive.Add("echo start");
                    c.EngineStopInteractive.Add("echo stop");
                })
                .Build();
            engine.RunInteractively();
            output.Lines.Count.Should().Be(6);
            output.Lines[0].Should().Be("start");
            // TODO: Lines 1, 2, and 3 are prompt which we should be able to control and remove from the
            // script somehow
            output.Lines[4].Should().Be("test");
            output.Lines[5].Should().Be("stop");
        }
    }
}