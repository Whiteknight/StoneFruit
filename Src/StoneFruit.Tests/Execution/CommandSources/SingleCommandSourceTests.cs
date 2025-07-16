using AwesomeAssertions;
using NUnit.Framework;
using StoneFruit.Execution.CommandSources;

namespace StoneFruit.Tests.Execution.CommandSources;

public class SingleCommandSourceTests
{
    [Test]
    public void GetNextCommand_Test()
    {
        var target = new QueueCommandSource("test");
        target.GetNextCommand().GetValueOrThrow().String.Should().Be("test");
        target.GetNextCommand().IsSuccess.Should().BeFalse();
    }
}
