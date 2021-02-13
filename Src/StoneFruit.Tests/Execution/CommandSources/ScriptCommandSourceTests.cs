using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.CommandSources;

namespace StoneFruit.Tests.Execution.CommandSources
{
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
            target.GetNextCommand().Value.Arguments.Get(0).Value.Should().Be("test1");
            target.GetNextCommand().Value.Arguments.Get(0).Value.Should().Be("test2");
            target.GetNextCommand().HasValue.Should().BeFalse();
        }

        [Test]
        public void GetNextCommand_Empty()
        {
            var script = new EventScript();
            var target = new ScriptCommandSource(script, CommandParser.GetDefault());
            target.GetNextCommand().HasValue.Should().BeFalse();
        }
    }
}
