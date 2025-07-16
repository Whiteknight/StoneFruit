using AwesomeAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Scripts;

namespace StoneFruit.Tests.Execution;

public class ScriptHandlerSourceTests
{
    [Test]
    public void GetInstance_DoesNotExist()
    {
        var source = new ScriptHandlerSource();
        var context = new HandlerContext(SyntheticArguments.From("X"), null, null, null, null, null);
        var instance = source.GetInstance(context);
        instance.IsSuccess.Should().BeFalse();
    }

    //[Test]
    //public void GetInstance_Test()
    //{
    //    var target = new ScriptHandlerSource();
    //    target.AddScript("test", new[] { "echo 'test'" });
    //    var result = target.GetInstance(SyntheticArguments.From("test"), new CommandDispatcher(null, null, null, null, null));
    //    result.HasValue.Should().BeTrue();
    //}
}
