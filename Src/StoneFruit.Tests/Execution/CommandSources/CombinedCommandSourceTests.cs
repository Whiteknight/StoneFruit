using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution.CommandSources;

namespace StoneFruit.Tests.Execution.CommandSources
{
    public class CombinedCommandSourceTests
    {
        [Test]
        public void GetNextCommand_Test()
        {
            var target = new CombinedCommandSource();
            target.AddSource(new SingleCommandSource("test1"));
            target.AddSource(new SingleCommandSource("test2"));
            target.Start();
            target.GetNextCommand().Should().Be("test1");
            target.GetNextCommand().Should().Be("test2");
            target.GetNextCommand().Should().Be(null);
        }
    }
}