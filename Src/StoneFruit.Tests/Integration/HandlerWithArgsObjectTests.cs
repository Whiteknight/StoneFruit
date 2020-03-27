using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Arguments;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class HandlerWithArgsObjectTests
    {
        [Test]
        public void Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test x y z");
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("x");
            output.Lines[1].Should().Be("y");
            output.Lines[2].Should().Be("z");
        }

        public class TestArgs
        {
            [ArgumentIndex(0)]
            public string Arg1 { get; set; }

            [ArgumentIndex(1)]
            public string Arg2 { get; set; }

            [ArgumentIndex(2)]
            public string Arg3 { get; set; }
        }

        public class TestHandler : IHandler
        {
            private readonly IOutput _output;
            private readonly TestArgs _args;

            public TestHandler(CommandArguments args, IOutput output)
            {
                _output = output;
                _args = args.MapTo<TestArgs>();
            }

            public void Execute()
            {
                _output.WriteLine(_args.Arg1);
                _output.WriteLine(_args.Arg2);
                _output.WriteLine(_args.Arg3);
            }
        }
    }
}
