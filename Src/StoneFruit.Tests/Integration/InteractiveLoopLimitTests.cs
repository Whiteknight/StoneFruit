using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Handlers;
using TestUtilities;

namespace StoneFruit.Tests.Integration;

public class InteractiveLoopLimitTests
{
    [Test]
    public void ScriptCutOff_n()
    {
        // Script with 4 commands, loop limit of 2. User enters "n" at the prompt to
        // continue. The runloop terminates after executing 2 commands
        var output = new TestOutput("test", "n");
        var engine = EngineBuilder.Build(b => b
             .SetupOutput(o => o.DoNotUseConsole().Add(output))
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(EchoHandler))
                .AddScript("test", new[] {
                    "echo 1",
                    "echo 2",
                    "echo 3",
                    "echo 4"
                })
            )
            .SetupSettings(s =>
            {
                s.MaxInputlessCommands = 2;
            })
            .SetupEvents(c =>
            {
                c.EngineStartInteractive.Clear();
                //c.EngineStopInteractive.Clear();
            })
        );
        engine.RunInteractively();
        output.Lines.Count.Should().Be(2);
        output.Lines[0].Should().Be("1");
        output.Lines[1].Should().Be("2");
    }

    [Test]
    public void ScriptCutOff_y()
    {
        // Script with 4 commands, loop limit of 2. User enters "y" at the prompt to
        // continue. The loop continues and executes all commands
        var output = new TestOutput("test", "y");
        var engine = EngineBuilder.Build(b => b
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(EchoHandler))
                .AddScript("test", new[] {
                    "echo 1",
                    "echo 2",
                    "echo 3",
                    "echo 4"
                })
            )
            .SetupSettings(s =>
            {
                s.MaxInputlessCommands = 2;
            })
            .SetupEvents(c =>
            {
                c.EngineStartInteractive.Clear();
                //c.EngineStopInteractive.Clear();
            })
        );
        engine.RunInteractively();
        output.Lines.Count.Should().Be(4);
        output.Lines[0].Should().Be("1");
        output.Lines[1].Should().Be("2");
        output.Lines[2].Should().Be("3");
        output.Lines[3].Should().Be("4");
    }
}
