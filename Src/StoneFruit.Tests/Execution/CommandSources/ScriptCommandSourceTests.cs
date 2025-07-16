using AwesomeAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.CommandSources;
using StoneFruit.Execution.Scripts;

namespace StoneFruit.Tests.Execution.CommandSources;

public class ScriptCommandSourceTests
{
    [Test]
    public void GetNextCommand_Test()
    {
        var script = new EventScript(
            "test1",
            "test2"
        );
        var target = new ScriptCommandSource(script, CommandParser.GetDefault());
        target.GetNextCommand().GetValueOrThrow().Arguments.Get(0).Value.Should().Be("test1");
        target.GetNextCommand().GetValueOrThrow().Arguments.Get(0).Value.Should().Be("test2");
        target.GetNextCommand().IsSuccess.Should().BeFalse();
    }

    [Test]
    public void GetNextCommand_Empty()
    {
        var script = new EventScript();
        var target = new ScriptCommandSource(script, CommandParser.GetDefault());
        target.GetNextCommand().IsSuccess.Should().BeFalse();
    }
}
