using FluentAssertions;
using NUnit.Framework;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class ScriptVerbsTests
    {
        [Verb("test1")]
        public class Test1Handler : IHandler
        {
            private readonly IOutput _output;

            public Test1Handler(IOutput output)
            {
                _output = output;
            }

            public void Execute()
            {
                _output.WriteLine("invoked");
            }
        }

        [Test]
        public void Script_PositionalBecomesSingleVerb()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h
                    .UseHandlerTypes(typeof(Test1Handler))
                    .AddScript("test", new[] { "[0]" })
                )
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test test1");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("invoked");
        }

        [Verb("test2 abc")]
        public class Test2Handler : IHandler
        {
            private readonly IOutput _output;

            public Test2Handler(IOutput output)
            {
                _output = output;
            }

            public void Execute()
            {
                _output.WriteLine("invoked");
            }
        }

        [Test]
        public void Script_PositionalBecomesMultiWordVerb()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h
                    .UseHandlerTypes(typeof(Test2Handler))
                    .AddScript("test", new[] { "test2 [0]" })
                )
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test abc");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("invoked");
        }
    }
}
