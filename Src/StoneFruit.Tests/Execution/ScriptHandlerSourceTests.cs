using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution;
using StoneFruit.Execution.Scripts;

namespace StoneFruit.Tests.Execution
{
    public class ScriptHandlerSourceTests
    {
        [Test]
        public void GetInstance_DoesNotExist()
        {
            var source = new ScriptHandlerSource();
            var instance = source.GetInstance(new Command("X", null), null);
            instance.Should().BeNull();
        }

        [Test]
        public void GetInstance_Test()
        {
            var target = new ScriptHandlerSource();
            target.AddScript("test", new[] { "echo 'test'" });
            var result = target.GetInstance(new Command("test", null), new CommandDispatcher(null, null, null, null, null));
            result.Should().NotBeNull();
        }
    }
}