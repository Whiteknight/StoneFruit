using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using NUnit.Framework;
using StoneFruit.Execution.Arguments;

namespace StoneFruit.Tests.Execution.Arguments;

public class SyntheticCommandArgumentsTests
{
    [Test]
    public void Empty_Test()
    {
        var target = SyntheticArguments.Empty();
        target.GetAllArguments().Count().Should().Be(0);
    }

    [Test]
    public void Single_Test()
    {
        var target = SyntheticArguments.From("test");
        target.GetAllArguments().Count().Should().Be(1);
        target.Shift().AsString().Should().Be("test");
    }

    [Test]
    public void Get_Index_TooHigh()
    {
        var target = SyntheticArguments.Empty();
        target.Get(0).Should().BeOfType<MissingArgument>();
    }

    [Test]
    public void Get_Index_AlreadyConsumed()
    {
        var target = SyntheticArguments.From("test");
        target.Get(0).Should().NotBeOfType<MissingArgument>();
        target.Get(0).MarkConsumed();
        target.Get(0).Should().BeOfType<MissingArgument>();
    }

    [Test]
    public void Get_Named_Consumed()
    {
        var target = new SyntheticArguments(new[]
        {
            new NamedArgument("a", "1"),
            new NamedArgument("a", "2")
        });
        var result = target.Get("a");
        result.Value.Should().Be("1");
        result.MarkConsumed();

        result = target.Get("a");
        result.Value.Should().Be("2");
        result.MarkConsumed();

        result = target.Get("a");
        result.Should().BeOfType<MissingArgument>();
    }

    [Test]
    public void Shift_Test()
    {
        var target = new SyntheticArguments(new[]
        {
            new PositionalArgument("a"),
            new PositionalArgument("b")
        });
        target.Shift().Value.Should().Be("a");
        target.Shift().Value.Should().Be("b");
        target.Shift().Should().BeOfType<MissingArgument>();
    }

    [Test]
    public void From_TupleList()
    {
        var target = SyntheticArguments.From(
            ("a", "1"),
            ("b", "2")
        );
        target.Get("a").AsString().Should().Be("1");
        target.Get("b").AsString().Should().Be("2");
    }

    [Test]
    public void From_Dictionary()
    {
        var target = SyntheticArguments.From(
            new Dictionary<string, string>
            {
                { "a", "1" },
                { "b", "2" }
            }
        );
        target.Get("a").AsString().Should().Be("1");
        target.Get("b").AsString().Should().Be("2");
    }

    [Test]
    public void GetAll_Named_Test()
    {
        var target = new SyntheticArguments(new[]
        {
            new NamedArgument("a", "1"),
            new NamedArgument("a", "2")
        });
        var result = target.GetAllNamed("a").Cast<INamedArgument>().ToList();
        result.Count.Should().Be(2);
        result[0].Value.Should().Be("1");
        result[1].Value.Should().Be("2");
    }

    [Test]
    public void GetAll_Named_Empty()
    {
        var target = new SyntheticArguments(new[]
        {
            new NamedArgument("a", "1"),
            new NamedArgument("a", "2")
        });
        var result = target.GetAllNamed("XXX").ToList();
        result.Count.Should().Be(0);
    }

    private class TestArgs1
    {
        [ArgumentIndex(0)]
        public string A { get; set; }

        public string B { get; set; }

        public bool C { get; set; }

        public bool D { get; set; }
    }

    [Test]
    public void MapTo_Test()
    {
        var target = new SyntheticArguments(new IArgument[]
        {
            new PositionalArgument("test1"),
            new NamedArgument("b", "test2"),
            new FlagArgument("c")
        });
        var result = target.MapTo<TestArgs1>();
        result.A.Should().Be("test1");
        result.B.Should().Be("test2");
        result.C.Should().BeTrue();
        result.D.Should().BeFalse();
    }

    [Test]
    public void MapOnTo_Test()
    {
        var target = new SyntheticArguments(new IArgument[]
        {
            new PositionalArgument("test1"),
            new NamedArgument("b", "test2"),
            new FlagArgument("c")
        });
        var result = new TestArgs1();
        target.MapOnto(result);
        result.A.Should().Be("test1");
        result.B.Should().Be("test2");
        result.C.Should().BeTrue();
        result.D.Should().BeFalse();
    }

    [Test]
    public void VerifyAllAreConsumed_NotConsumed()
    {
        var target = new SyntheticArguments(new IArgument[]
        {
            new PositionalArgument("test1"),
            new NamedArgument("b", "test2"),
            new FlagArgument("c")
        });
        Action act = () => target.VerifyAllAreConsumed();
        act.Should().Throw<StoneFruit.Execution.Arguments.ArgumentParseException>();
    }

    [Test]
    public void VerifyAllAreConsumed_Consumed()
    {
        var target = new SyntheticArguments(new IArgument[]
        {
            new PositionalArgument("test1"),
            new NamedArgument("b", "test2"),
            new FlagArgument("c")
        });
        target.Consume(0);
        target.Consume("b");
        target.ConsumeFlag("c");
        Action act = () => target.VerifyAllAreConsumed();
        act.Should().NotThrow();
    }
}
