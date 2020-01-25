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
            var target = new SingleCommandSource("test");
            target.Start();
            target.GetNextCommand().Should().Be("test");
            target.GetNextCommand().Should().Be(null);
        }
    }
}
