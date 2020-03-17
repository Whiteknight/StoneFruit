using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Handlers;
using StoneFruit.Tests.Helpers;

namespace StoneFruit.Tests.Integration
{
    public class HeadlessTests
    {
        [Test]
        public void StartAndStopEvents_Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(EchoHandler)))
                .UseOutput(output)
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

        [Verb("test-help")]
        public class TestHelpHandler : IHandler
        {
            private readonly IOutput _output;

            public TestHelpHandler(IOutput output)
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
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(EchoHandler), typeof(TestHelpHandler)))
                .UseOutput(output)
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