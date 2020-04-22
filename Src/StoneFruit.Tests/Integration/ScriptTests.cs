using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Handlers;
using TestUtilities;

namespace StoneFruit.Tests.Integration
{
    public class ScriptTests
    {
        [Test]
        public void AddScript_Literals()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h
                    .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                    .AddScript("test", new[] { "argument-display a b=c -d" })
                )
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test x y z");
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("0: a");
            output.Lines[1].Should().Be("'b': c");
            output.Lines[2].Should().Be("flag: d");
        }

        [Test]
        public void AddScript_FetchPositional()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h
                    .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                    .AddScript("test", new[] { "argument-display [1] [0] ['x'] [3]" })
                )
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test a b x=c");
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("0: b");
            output.Lines[1].Should().Be("1: a");
            output.Lines[2].Should().Be("2: c");
        }

        [Test]
        public void AddScript_FetchAllPositionals()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h
                    .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                    .AddScript("test", new[] { "argument-display [1] [*]" })
                )
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test a b c");
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("0: b");
            output.Lines[1].Should().Be("1: a");
            output.Lines[2].Should().Be("2: c");
        }

        [Test]
        public void AddScript_Named()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h
                    .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                    .AddScript("test", new[] { "argument-display a=['b'] {c} d=[0]" })
                )
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test x b=y c=z");
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("'a': y");
            output.Lines[1].Should().Be("'c': z");
            output.Lines[2].Should().Be("'d': x");
        }

        [Test]
        public void AddScript_AllNamed()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h
                    .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                    .AddScript("test", new[] { "argument-display {b} {*}" })
                )
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test a=x b=y c=z");
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("'b': y");
            output.Lines[1].Should().Be("'a': x");
            output.Lines[2].Should().Be("'c': z");
        }

        [Test]
        public void AddScript_Flags()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h
                    .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                    .AddScript("test", new[] { "argument-display ?x ?y" })
                )
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test -x");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("flag: x");
        }

        [Test]
        public void AddScript_AllFlags()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h
                    .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                    .AddScript("test", new[] { "argument-display ?y -*" })
                )
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test -x -y -z");
            output.Lines.Count.Should().Be(3);
            output.Lines[0].Should().Be("flag: y");
            output.Lines[1].Should().Be("flag: x");
            output.Lines[2].Should().Be("flag: z");
        }

        [Test]
        public void AddScript_3Lines()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h
                    .UseHandlerTypes(typeof(ArgumentDisplayHandler), typeof(EchoHandler))
                    .AddScript("test", new[]
                    {
                        "echo start",
                        "argument-display [0] {b} ?d",
                        "echo stop"
                    })
                )
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test a b=c -d");
            output.Lines.Count.Should().Be(5);
            output.Lines[0].Should().Be("start");
            output.Lines[1].Should().Be("0: a");
            output.Lines[2].Should().Be("'b': c");
            output.Lines[3].Should().Be("flag: d");
            output.Lines[4].Should().Be("stop");
        }

        [Test]
        public void AddScript_Nested()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h
                    .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                    .AddScript("test1", new[] { "argument-display [0]" })
                    .AddScript("test2", new[] { "test1 [0]" })
                )
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test2 a");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("0: a");
        }

        [Test]
        public void AddScript_Nested_ReverseDeclareOrder()
        {
            var output = new TestOutput();
            var engine = new EngineBuilder()
                .SetupHandlers(h => h
                    .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                    .AddScript("test2", new[] { "test1 [0]" })
                    .AddScript("test1", new[] { "argument-display [0]" })
                )
                .SetupOutput(o => o.DoNotUseConsole().Add(output))
                .Build();
            engine.RunHeadless("test2 a");
            output.Lines.Count.Should().Be(1);
            output.Lines[0].Should().Be("0: a");
        }
    }
}
