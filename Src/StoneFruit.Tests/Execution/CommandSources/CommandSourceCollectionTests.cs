using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution.CommandSources;

namespace StoneFruit.Tests.Execution.CommandSources
{
    public class CommandSourceCollectionTests
    {
        [Test]
        public void GetNextCommand_Empty()
        {
            var target = new CommandSourceCollection();
            target.GetNextCommand().Should().Be(null);
        }

        [Test]
        public void GetNextCommand_Test()
        {
            var target = new CommandSourceCollection();
            target.AddToEnd(new QueueCommandSource("test1"));
            target.AddToEnd(new QueueCommandSource("test2"));
            target.GetNextCommand().String.Should().Be("test1");
            target.GetNextCommand().String.Should().Be("test2");
            target.GetNextCommand().Should().Be(null);
        }
    }
}