using FluentAssertions;
using NUnit.Framework;
using ParserObjects;
using StoneFruit.Execution.Arguments;
using StoneFruit.Execution.Scripts;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class ParsersTests
    {
        [Verb("test")]
        private class TestHandler : IHandler
        {
            private readonly IOutput _output;
            private readonly IArguments _args;

            public TestHandler(IOutput output, IArguments args)
            {
                _output = output;
                _args = args;
            }

            public void Execute()
            {
                _output.WriteLine(_args.Get(0).AsString());
                _output.WriteLine(_args.Get("b").AsString());
                _output.WriteLine(_args.HasFlag("c").ToString());
            }
        }

        [Test]
        public void UseSimplifiedArgumentParser_Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupArguments(a => a.UseSimplifiedArgumentParser())
                .Build();
            engine.RunHeadless("test a b=x -c");
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("a");
            output.Lines[1].Should().Be("x");
            output.Lines[2].Should().Be("True");
        }

        [Test]
        public void UseParser_Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupArguments(a => a.UseParser(new CommandParser(SimplifiedArgumentGrammar.GetParser(), ScriptFormatGrammar.GetParser())))
                .Build();
            engine.RunHeadless("test a b=x -c");
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("a");
            output.Lines[1].Should().Be("x");
            output.Lines[2].Should().Be("True");
        }

        [Test]
        public void UseParser_Null()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupArguments(a => a.UseParser(null))
                .Build();
            engine.RunHeadless("test a b=x -c");
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("a");
            output.Lines[1].Should().Be("x");
            output.Lines[2].Should().Be("True");
        }


        [Test]
        public void UseScriptParser_Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupArguments(a => a.UseScriptParser(ScriptFormatGrammar.GetParser()))
                .Build();
            engine.RunHeadless("test a b=x -c");
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("a");
            output.Lines[1].Should().Be("x");
            output.Lines[2].Should().Be("True");
        }

        [Test]
        public void UseArgumentParser_Null1()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupArguments(a => a.UseArgumentParser((IParser<char, IParsedArgument>)null))
                .Build();
            engine.RunHeadless("test a b=x -c");
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("a");
            output.Lines[1].Should().Be("x");
            output.Lines[2].Should().Be("True");
        }

        [Test]
        public void UsePosixStyleArgumentParser_Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupArguments(a => a.UsePosixStyleArgumentParser())
                .Build();
            engine.RunHeadless("test a --b x -c");
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("a");
            output.Lines[1].Should().Be("x");
            output.Lines[2].Should().Be("True");
        }

        [Test]
        public void UsePowershellArgumentParser_Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupArguments(a => a.UsePowershellStyleArgumentParser())
                .Build();
            engine.RunHeadless("test a -b x -c");
            //output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("a");
            output.Lines[1].Should().Be("x");
            output.Lines[2].Should().Be("True");
        }

        [Test]
        public void UseWindowsCmdArgumentParser_Test()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h.UseHandlerTypes(typeof(TestHandler)))
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .SetupArguments(a => a.UseWindowsCmdArgumentParser())
                .Build();
            engine.RunHeadless("test a /b:x /c");
            //output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("a");
            output.Lines[1].Should().Be("x");
            output.Lines[2].Should().Be("True");
        }
    }
}
