using System;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Handlers;

namespace StoneFruit.Tests.Execution.HandlerSources;

public class NamedInstanceSourceTests
{
    private class TestCommandHandler : IHandler
    {
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }

    [Test]
    public void GetInstance_Test()
    {
        var target = new NamedInstanceHandlerSource();
        target.Add("test", new TestCommandHandler());
        var context = new HandlerContext(SyntheticArguments.From("test"), null, null, null, null, null);
        var result = target.GetInstance(context);
        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().BeOfType<TestCommandHandler>();
    }
}
