using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution.CommandSources;

namespace StoneFruit.Tests.Execution.CommandSources
{
    public class SingleCommandSourceTests
    {
        [Test]
        public void GetNextCommand_Test()
        {
            var target = new QueueCommandSource("test");
            target.GetNextCommand().Value.String.Should().Be("test");
            target.GetNextCommand().HasValue.Should().BeFalse();
        }
    }
}
