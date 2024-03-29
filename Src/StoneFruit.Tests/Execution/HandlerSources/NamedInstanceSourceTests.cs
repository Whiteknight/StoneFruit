﻿using System;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Handlers;

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
            var result = target.GetInstance(SyntheticArguments.From("test"), null);
            result.HasValue.Should().BeTrue();
            result.Value.Should().BeOfType<TestCommandHandler>();
        }
    }
}
