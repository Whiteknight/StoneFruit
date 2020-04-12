using System;
using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Handlers;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class HandlerThrowsExceptionTests
    {
        //[Test]
        //public void Test()
        //{
        //    var output = new TestOutput();
        //    var engine = new EngineBuilder()
        //        .SetupHandlers(h => h.UseHandlerTypes(typeof(EchoHandler), typeof(TestHandler)))
        //        .SetupOutput(o => o.DoNotUseConsole().Add(output))
        //        .Build();
        //    engine.RunHeadless("test");
        //    output.Lines[0].Should().Be("TEST");
        //}

        public class TestHandler : IHandler
        {
            public void Execute()
            {
                throw new Exception("TEST");
            }
        }
    }
}