using FluentAssertions;
using NUnit.Framework;
using StoneFruit.Handlers;
using TestUtilities;

namespace StoneFruit.Tests.Integration;

public class ScriptTests
{
    [Test]
    public void AddScript_Literals()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args a b=c -d" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test x y z");
        output.Lines.Count.Should().Be(3);
        output.Lines[0].Should().Be("0: a");
        output.Lines[1].Should().Be("'b': c");
        output.Lines[2].Should().Be("flag: d");
    }

    [Test]
    public void AddScript_Positional()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args [1] [0] ['x'] [3]" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test a b x=c");
        output.Lines.Count.Should().Be(3);
        output.Lines[0].Should().Be("0: b");
        output.Lines[1].Should().Be("1: a");
        output.Lines[2].Should().Be("2: c");
    }

    [Test]
    public void AddScript_PositionalIndexed_RequiredFail()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args [0]!" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test");
        output.Lines[0].Should().StartWith("Required argument");
    }

    [Test]
    public void AddScript_PositionalIndexed_DefaultValue()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args [0:test]" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test");
        output.Lines.Count.Should().Be(1);
        output.Lines[0].Should().Be("0: test");
    }

    [Test]
    public void AddScript_PositionalIndexed_SingleQuotedDefaultValue()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args [0:'test']" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test");
        output.Lines.Count.Should().Be(1);
        output.Lines[0].Should().Be("0: test");
    }

    [Test]
    public void AddScript_PositionalIndexed_DoubleQuotedDefaultValue()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args [0:\"test\"]" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test");
        output.Lines.Count.Should().Be(1);
        output.Lines[0].Should().Be("0: test");
    }

    [Test]
    public void AddScript_PositionalNamed_RequiredFail()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args ['x']!" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test");
        output.Lines[0].Should().StartWith("Required argument");
    }

    [Test]
    public void AddScript_PositionalNamed_DefaultValue()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args ['x':test]" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test");
        output.Lines.Count.Should().Be(1);
        output.Lines[0].Should().Be("0: test");
    }

    [Test]
    public void AddScript_AllPositionals()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args [1] [*]" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
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
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args a=['b'] {c} d=[0]" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test x b=y c=z");
        output.Lines.Count.Should().Be(3);
        output.Lines[0].Should().Be("'a': y");
        output.Lines[1].Should().Be("'c': z");
        output.Lines[2].Should().Be("'d': x");
    }

    [Test]
    public void AddScript_LiteralNameNamedValue_RequiredFail()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                // 'b' is required but not provided.
                .AddScript("test", new[] { "_args a=['b']!" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test");
        output.Lines[0].Should().StartWith("Required argument");
    }

    [Test]
    public void AddScript_LiteralNameNamedValue_DefaultValue()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args a=['b':test]" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test");
        output.Lines.Count.Should().Be(1);
        output.Lines[0].Should().Be("'a': test");
    }

    [Test]
    public void AddScript_LiteralNamePositionalValue_RequiredFail()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args d=[0]!" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test");
        output.Lines[0].Should().StartWith("Required argument");
    }

    [Test]
    public void AddScript_LiteralNamePositionalValue_DefaultValue()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args d=[0:test]" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test");
        output.Lines.Count.Should().Be(1);
        output.Lines[0].Should().Be("'d': test");
    }

    [Test]
    public void AddScript_Named_RequiredFail()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args {c}!" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test");
        output.Lines[0].Should().StartWith("Required argument");
    }

    [Test]
    public void AddScript_Named_DefaultValue()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args {c:test}" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test");
        output.Lines.Count.Should().Be(1);
        output.Lines[0].Should().Be("'c': test");
    }

    [Test]
    public void AddScript_AllNamed()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args {b} {*}" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
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
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args ?x ?y" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test -x");
        output.Lines.Count.Should().Be(1);
        output.Lines[0].Should().Be("flag: x");
    }

    [Test]
    public void AddScript_FlagsRenamed()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args ?x:a ?y:b" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test -x");
        output.Lines.Count.Should().Be(1);
        output.Lines[0].Should().Be("flag: a");
    }

    [Test]
    public void AddScript_AllFlags()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test", new[] { "_args ?y -*" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
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
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler), typeof(EchoHandler))
                .AddScript("test", new[]
                {
                    "echo start",
                    "_args [0] {b} ?d",
                    "echo stop"
                })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
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
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test1", new[] { "_args [0]" })
                .AddScript("test2", new[] { "test1 [0]" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test2 a");
        output.Lines.Count.Should().Be(1);
        output.Lines[0].Should().Be("0: a");
    }

    [Test]
    public void AddScript_Nested_ReverseDeclareOrder()
    {
        var output = new TestOutput();
        var engine = EngineBuilder.Build(b => b
            .SetupHandlers(h => h
                .UseHandlerTypes(typeof(ArgumentDisplayHandler))
                .AddScript("test2", new[] { "test1 [0]" })
                .AddScript("test1", new[] { "_args [0]" })
            )
            .SetupOutput(o => o.DoNotUseConsole().Add(output))
        ); ;
        engine.RunHeadless("test2 a");
        output.Lines.Count.Should().Be(1);
        output.Lines[0].Should().Be("0: a");
    }
}
