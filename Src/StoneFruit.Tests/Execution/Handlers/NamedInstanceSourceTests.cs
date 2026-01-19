using System;

namespace StoneFruit.Tests.Execution.HandlerSources;

public class NamedInstanceSourceTests
{
    private class TestCommandHandler : IHandler
    {
        public void Execute(IArguments arguments, HandlerContext context)
        {
            throw new NotImplementedException();
        }
    }

    //[Test]
    //public void GetInstance_Test()
    //{
    //    var target = new NamedInstanceHandlerSource();
    //    target.Add("test", new TestCommandHandler());
    //    var context = new HandlerContext(SyntheticArguments.Empty("test"), null, null, null, null, null);
    //    var result = target.GetInstance(context);
    //    result.IsSuccess.Should().BeTrue();
    //    result.GetValueOrThrow().Should().BeOfType<TestCommandHandler>();
    //}
}
