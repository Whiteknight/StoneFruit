﻿using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Handlers;
using StoneFruit.Tests.Helpers;

namespace StoneFruit.Tests.Integration
{
    public class SanityTests
    {
        [Test]
        public void RunHeadless_Echo()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(EchoHandler)))
                .SetupOutput(o => o.Add(output))
                .Build();
            engine.RunHeadless("echo 'test'");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("test");
        }
    }
}
