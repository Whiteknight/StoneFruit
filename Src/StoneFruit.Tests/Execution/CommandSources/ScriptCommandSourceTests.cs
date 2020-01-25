using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution;
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
            var target = new ScriptCommandSource(script);
            target.Start();
            target.GetNextCommand().Should().Be("test1");
            target.GetNextCommand().Should().Be("test2");
            target.GetNextCommand().Should().Be(null);
        }

        [Test]
        public void GetNextCommand_Empty()
        {
            var script = new EventScript();
            var target = new ScriptCommandSource(script);
            target.Start();
            target.GetNextCommand().Should().Be(null);
        }
    }
}