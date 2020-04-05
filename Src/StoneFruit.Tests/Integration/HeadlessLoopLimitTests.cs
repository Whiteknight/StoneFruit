using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Handlers;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class HeadlessLoopLimitTests
    {
        [Test]
        public void CutOffScript_Test()
        {
            // Script with 4 commands. Loop limit of 2. Engine executes 2 commands and
            // then terminates with an error message
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupHandlers(h => h
                    .UseHandlerTypes(typeof(EchoHandler), typeof(ExitHandler))
                    .AddScript("test", new[] {
                        "echo 1",
                        "echo 2",
                        "echo 3",
                        "echo 4"
                    })
                )
                .SetupSettings(s => {
                    s.MaxInputlessCommands = 2;
                })
                .Build();
            engine.RunHeadless("test");
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("1");
            output.Lines[1].Should().Be("2");
            output.Lines[2].Should().StartWith("Maximum");
        }

        [Test]
        public void ScriptInLimit_Test()
        {
            // Script with 4 commands. Loop limit of 4. Engine executes all 4 commands
            // without issue
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupHandlers(h => h
                    .UseHandlerTypes(typeof(EchoHandler), typeof(ExitHandler))
                    .AddScript("test", new[] {
                        "echo 1",
                        "echo 2",
                        "echo 3",
                        "echo 4"
                    })
                )
                .SetupSettings(s => {
                    s.MaxInputlessCommands = 4;
                })
                .Build();
            engine.RunHeadless("test");
            output.Lines.Count.Should().Be(4);
            output.Lines[0].Should().Be("1");
            output.Lines[1].Should().Be("2");
            output.Lines[2].Should().Be("3");
            output.Lines[3].Should().Be("4");

        }
    }
}
