using FluentAssertions;
using NUnit.Framework;
using StoneFruit.BuiltInVerbs;
using StoneFruit.Tests.Helpers;

namespace StoneFruit.Tests.Integration
{
    public class HeadlessTests
    {
        [Test]
        public void StartAndStopEvents_Test()
        {
            var output = new TestTerminalOutput();
            var engine = new EngineBuilder()
                .UseCommandType(typeof(EchoHandler))
                .UseTerminalOutput(output)
                .SetupEvents(c =>
                {
                    c.EngineStartHeadless.Add("echo start");
                    c.EngineStopHeadless.Add("echo stop");
                })
                .Build();
            engine.RunHeadless("echo 'test'");
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("start");
            output.Lines[1].Should().Be("test");
            output.Lines[2].Should().Be("stop");
        }

        [CommandName("test-help")]
        public class TestHelpHandler : ICommandHandler
        {
            private readonly ITerminalOutput _output;

            public TestHelpHandler(ITerminalOutput output)
            {
                _output = output;
            }

            public void Execute()
            {
                _output.WriteLine("helped");
            }
        }

        [Test]
        public void HeadlessHelp_Test()
        {
            var output = new TestTerminalOutput();
            var engine = new EngineBuilder()
                .UseCommandType(typeof(TestHelpHandler))
                .UseTerminalOutput(output)
                .SetupEvents(c =>
                {
                    c.HeadlessHelp.Clear();
                    c.HeadlessHelp.Add("test-help");
                })
                .Build();
            engine.RunHeadless("help");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("helped");
        }
    }
}