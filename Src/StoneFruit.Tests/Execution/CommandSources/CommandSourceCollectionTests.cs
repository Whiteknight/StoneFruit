using AwesomeAssertions;
using NUnit.Framework;
using StoneFruit.Execution.CommandSources;

namespace StoneFruit.Tests.Execution.CommandSources;

public class CommandSourceCollectionTests
{
    [Test]
    public void GetNextCommand_Empty()
    {
        var builder = new CommandSourcesBuilder(null!, null!, null!, null!);
        var target = builder.Build();
        target.GetNextCommand().IsSuccess.Should().BeFalse();
    }

    [Test]
    public void GetNextCommand_Test()
    {
        var target = new CommandSourcesBuilder(null!, null!, null!, null!);
        target.AddToEnd(new QueueCommandSource("test1"));
        target.AddToEnd(new QueueCommandSource("test2"));
        var sources = target.Build();
        sources.GetNextCommand().GetValueOrThrow().String.Should().Be("test1");
        sources.GetNextCommand().GetValueOrThrow().String.Should().Be("test2");
        sources.GetNextCommand().IsSuccess.Should().BeFalse();
    }
}
