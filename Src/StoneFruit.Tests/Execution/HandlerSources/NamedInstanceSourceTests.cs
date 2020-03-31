using System;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution;
using StoneFruit.Execution.HandlerSources;

namespace StoneFruit.Tests.Execution.HandlerSources
{
    public class NamedInstanceSourceTests
    {
        private class TestCommandHandler : IHandler
        {
            public TestCommandHandler()
            {
            }

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
            var result = target.GetInstance(Command.Create("test", null), null);
            result.Should().BeOfType<TestCommandHandler>();
        }
    }
}